using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class playerMovementScript : MonoBehaviour
{
    [HideInInspector] public GridLayout Grille;
    [HideInInspector] public bool targetActive;
    private Color baseColorIndicator;
    private GameObject lastIndicator = null;
    Vector3Int nextCell;
    bool movementMode = false;
    int indexPath = 1;
    bool doneBFS = false;
    public BFS BreathFirstSearch;
    public List<Node> path = new List<Node>();
    [SerializeField] float speedToNextCell;
    [Header("--------------OBJETS A REMPLIR--------------")]
    [SerializeField] GameObject indicator;
    [SerializeField] Transform spawnPoint;
    public actionChoiceUI_script bubblesActionChoice;

    // Start is called before the first frame update
    void Start()
    {
        InitVar();
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        interactionIndicators();
        Moving();
    }

    void InitVar(){
        BreathFirstSearch = new BFS(new Vector3Int(-4,-4,0), new Vector3Int(3,3,0), Grille);
        baseColorIndicator = indicator.GetComponent<SpriteRenderer>().color;
        for(int i = 0; i < GameObject.FindGameObjectsWithTag("Enemy").Length; i++){
            GameObject.FindGameObjectsWithTag("Enemy")[i].GetComponent<zombieScript>().unwalkableCell();
        }
    }

    void Spawn(){
        Vector3Int cellPosition = Grille.WorldToCell(spawnPoint.position);
        transform.position = new Vector3(Grille.CellToWorld(cellPosition).x+0.5f,Grille.CellToWorld(cellPosition).y,Grille.CellToWorld(cellPosition).z+0.5f);
    }

    void Moving(){
        float realSpeed = speedToNextCell*Time.deltaTime;
        if(targetActive){
            Vector3 worldNextCell = Grille.CellToWorld(nextCell)+new Vector3(0.5f,0,0.5f);
            if(worldNextCell.x < transform.position.x){
                transform.GetChild(0).localScale = new Vector3(-Mathf.Abs(transform.GetChild(0).localScale.x),transform.GetChild(0).localScale.y,transform.GetChild(0).localScale.z);
                transform.GetChild(1).localScale = new Vector3(-Mathf.Abs(transform.GetChild(1).localScale.x),transform.GetChild(1).localScale.y,transform.GetChild(1).localScale.z);
            }else if(worldNextCell.x > transform.position.x){
                transform.GetChild(0).localScale = new Vector3(Mathf.Abs(transform.GetChild(0).localScale.x),transform.GetChild(0).localScale.y,transform.GetChild(0).localScale.z);
                transform.GetChild(1).localScale = new Vector3(Mathf.Abs(transform.GetChild(1).localScale.x),transform.GetChild(1).localScale.y,transform.GetChild(1).localScale.z);
            }
            transform.position = Vector3.MoveTowards(transform.position,worldNextCell,realSpeed);
            if(Vector3.Distance(transform.position, worldNextCell) < 0.1f && path.Count-indexPath >= 0 || indexPath == 1){
                GetComponent<Animator>().SetBool("moving",true);
                nextCell = path[path.Count-indexPath].coord;
                indexPath++;
            } else if(Vector3.Distance(transform.position, worldNextCell) < 0.1f && path.Count-indexPath < 0){
                targetActive = false;
                indexPath = 1;
                doneBFS = false;
                changeMovementMode(false);
                GetComponent<Animator>().SetBool("moving",false);
            }
        }
        if(!doneBFS && movementMode){
            setBFSorigin(transform.position);
            doneBFS = true;
        }
    }

    void changeMovementMode(bool setMovementActive){
        if(movementMode && !targetActive && !setMovementActive){
            movementMode = false;
            destroyIndicators();
            bubblesActionChoice.changeActivatedMode(0);
        }else if(!movementMode && setMovementActive){
            bubblesActionChoice.changeActivatedMode(1);
            movementMode = true;
            doneBFS = false;
        }
    }

    void interactionIndicators(){
        Vector3 screenPos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hitInfo; 

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Indicator")))
        {
            if(hitInfo.transform.CompareTag("Indicator") && !targetActive){
                if(hitInfo.transform.gameObject != lastIndicator){
                    if(lastIndicator == null)
                        lastIndicator = hitInfo.transform.gameObject;
                    if(lastIndicator != null){
                        changeColorPathIndicator(lastIndicator.transform.gameObject,baseColorIndicator);
                        lastIndicator = hitInfo.transform.gameObject;
                        changeColorPathIndicator(hitInfo.transform.gameObject,Color.red);
                        GetComponent<attackScript>().HUD.displayAPcost(path.Count-1);
                    }
                }
                if(Input.GetMouseButtonDown(0)){
                    changeColorPathIndicator(hitInfo.transform.gameObject,Color.green);
                    GetComponent<attackScript>().actualActionPoint -= path.Count-1;
                    GetComponent<attackScript>().HUD.refreshAP();
                    targetActive = true;
                }
            }else if(lastIndicator !=null){
                changeColorPathIndicator(lastIndicator.transform.gameObject,baseColorIndicator);
                lastIndicator = null;
                GetComponent<attackScript>().HUD.hideAPcost();
            }
        }
    }

    void setBFSorigin(Vector3 position){
        Vector3Int originCell = Grille.WorldToCell(position);
        destroyIndicators();
        for(int i = 0; i < BreathFirstSearch.Noeuds.Count; i++){
            if(BreathFirstSearch.Noeuds[i].coord == originCell){
                displayBFSNV(BreathFirstSearch.Noeuds[i]);
                string debug = "";
                for(int j = 0; j < BreathFirstSearch.Noeuds[i].voisins.Count; j++){
                    debug += BreathFirstSearch.Noeuds[i].voisins[j].coord;
                }
                Debug.Log(debug);
                break;
            }
        }
    }

    public void destroyIndicators(){
        for(int i = 0; i < BreathFirstSearch.NV.Count; i++){
            if(BreathFirstSearch.NV[i].indicator != null){
                Destroy(BreathFirstSearch.NV[i].indicator);
                BreathFirstSearch.NV[i].indicator = null;
            }
        }
    }

    void changeColorPathIndicator(GameObject indicatorOrigin, Color colorToSet){
        Node previousNode = null;
        path.Clear();
        for(int i = 0; i < BreathFirstSearch.Noeuds.Count; i++){
            if(BreathFirstSearch.Noeuds[i].coord == Grille.WorldToCell(indicatorOrigin.transform.position-new Vector3(0.5f,0,0.5f))){
                BreathFirstSearch.Noeuds[i].indicator.GetComponent<SpriteRenderer>().color = colorToSet;
                previousNode = BreathFirstSearch.Noeuds[i];
                path.Add(previousNode);
            }
        }
        while(previousNode != null){
            previousNode.indicator.GetComponent<SpriteRenderer>().color = colorToSet;
            previousNode = previousNode.previousNode;
            if(previousNode != null)
                path.Add(previousNode);
        }
    }

    void displayBFSNV(Node nodeOrigin){
        BreathFirstSearch.startBfs(nodeOrigin, Mathf.RoundToInt(GetComponent<attackScript>().actualActionPoint));
        for(int i = 0; i < BreathFirstSearch.NV.Count; i++){
            if((BreathFirstSearch.NV[i].indicator == null || BreathFirstSearch.NV[i] != nodeOrigin) && !BreathFirstSearch.NV[i].IsOccupied)
                BreathFirstSearch.NV[i].indicator = Instantiate(indicator,Grille.CellToWorld(BreathFirstSearch.NV[i].coord)+new Vector3(0.5f,0.05f,0.5f),Quaternion.Euler(90,0,0));
        }
    }
}
