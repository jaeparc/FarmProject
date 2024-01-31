using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class zombieScript : MonoBehaviour
{
    [HideInInspector] public float headHP, armsHP, torsoHP, legsHP;
    [HideInInspector] public float headProba, armsProba, torsoProba, legsProba;
    [HideInInspector] public int actualActionPoint;
    private Vector3Int nextCell;
    private bool targetActive = false;
    private int indexPath = 2;
    public BFS BreathFirstSearch;
    public List<Node> path = new List<Node>();
    private GameObject player;
    private playerMovementScript playerMoveScript;
    private actionChoiceUI_script playerUIscript;
    private attackScript playerAttackScript;
    private GridLayout Grille;

    [Header("--------------OBJETS A REMPLIR--------------")]
    public stats_Zombie Stats;
    [SerializeField] GameObject aimButton;
    [SerializeField] TextMeshProUGUI hitDamagesText;
    // Start is called before the first frame update
    void Start()
    {
        InitVar();
        InitStats();
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        buttonAttack();
        Moving();
    }

    void InitVar(){
        player = GameObject.FindWithTag("Player");
        playerAttackScript = player.GetComponent<attackScript>();
        playerMoveScript = player.GetComponent<playerMovementScript>();
        playerUIscript = playerMoveScript.bubblesActionChoice;
        Grille = playerMoveScript.Grille;
        actualActionPoint = Stats.actionPointMax;
        BreathFirstSearch = new BFS(new Vector3Int(-4,-4,0), new Vector3Int(3,3,0), Grille);
    }

    void InitStats(){
        headHP = Stats.headHP;
        torsoHP = Stats.torsoHP;
        armsHP = Stats.armsHP;
        legsHP = Stats.legsHP;

        headProba = Stats.headProba;
        torsoProba = Stats.torsoProba;
        armsProba = Stats.armsProba;
        legsProba = Stats.legsProba;
    }

    void Spawn(){
        Vector3Int cellPosition = Grille.WorldToCell(transform.position);
        transform.position = new Vector3(Grille.CellToWorld(cellPosition).x+0.5f,Grille.CellToWorld(cellPosition).y,Grille.CellToWorld(cellPosition).z+0.5f);
    }

    void unwalkableCell(){
        Ray ray = new Ray(transform.position+new Vector3(0.1f,1,0.1f),Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Sol")) && hit.transform.CompareTag("Walkable")){
            for(int i = 0; i < playerMoveScript.BreathFirstSearch.Noeuds.Count; i++){
                if(playerMoveScript.BreathFirstSearch.Noeuds[i].coord == Grille.WorldToCell(hit.transform.position))
                    playerMoveScript.BreathFirstSearch.Noeuds[i].unit = gameObject;
            }
        }
    }

    void buttonAttack(){
        if(playerUIscript.activatedMode == 2){
            Vector3 screenPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit hitInfo; 

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity)){
                if(hitInfo.transform.gameObject == gameObject){
                    aimButton.SetActive(true);
                    if(Input.GetMouseButtonUp(0)){
                        playerAttackScript.aim(gameObject);
                    }
                }
                else
                    aimButton.SetActive(false);
            }
        } else if(aimButton.activeSelf)
            aimButton.SetActive(false);
    }

    public void getAttackedIsDying(string part, float damage){
        if(damage <= 0)
            GetComponent<Animator>().SetTrigger("missed");
        else{
            switch(part){
                case "head":
                    headHP-=damage;
                    GetComponent<Animator>().SetTrigger("hit");
                    hitDamagesText.text = "-"+Mathf.RoundToInt(damage);
                    break;
                case "torso":
                    torsoHP-=damage;
                    GetComponent<Animator>().SetTrigger("hit");
                    hitDamagesText.text = "-"+Mathf.RoundToInt(damage);
                    break;
                case "arms":
                    armsHP-=damage;
                    GetComponent<Animator>().SetTrigger("hit");
                    hitDamagesText.text = "-"+Mathf.RoundToInt(damage);
                    break;
                case "legs":
                    legsHP-=damage;
                    GetComponent<Animator>().SetTrigger("hit");
                    hitDamagesText.text = "-"+Mathf.RoundToInt(damage);
                    break;
            }
        }
    }

    public float getProbaPart(string part){
        switch(part){
            case "head":
                return headProba;
            case "torso":
                return torsoProba;
            case "arms":
                return armsProba;
            case "legs":
                return legsProba;
        }
        return 0;
    }

    public void endHitAnimation(){
        if(headHP<=0 || torsoHP<=0){
            playerAttackScript.VATS.closeInterface();
            GetComponent<Animator>().SetTrigger("death");
        }
    }

    public void endDeathAnimation(){
        Destroy(gameObject);
    }

    public void turnAction(){
        setPath();
    }

    void Moving(){
        if(targetActive){
            float realSpeed = Stats.speedToNextCell*Time.deltaTime;
            Vector3 worldNextCell = Grille.CellToWorld(nextCell)+new Vector3(0.5f,0,0.5f);
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position,worldNextCell,realSpeed);
            GetComponent<Animator>().SetBool("moving",true);
            if(worldNextCell.x < transform.position.x){
                transform.GetChild(0).localScale = new Vector3(-Mathf.Abs(transform.GetChild(0).localScale.x),transform.GetChild(0).localScale.y,transform.GetChild(0).localScale.z);
                transform.GetChild(1).localScale = new Vector3(-Mathf.Abs(transform.GetChild(1).localScale.x),transform.GetChild(1).localScale.y,transform.GetChild(1).localScale.z);
            }else if(worldNextCell.x > transform.position.x){
                transform.GetChild(0).localScale = new Vector3(Mathf.Abs(transform.GetChild(0).localScale.x),transform.GetChild(0).localScale.y,transform.GetChild(0).localScale.z);
                transform.GetChild(1).localScale = new Vector3(Mathf.Abs(transform.GetChild(1).localScale.x),transform.GetChild(1).localScale.y,transform.GetChild(1).localScale.z);
            }
            for(int i = 0; i < playerMoveScript.BreathFirstSearch.Noeuds.Count; i++){
                if(playerMoveScript.BreathFirstSearch.Noeuds[i].coord == path[path.Count-1].coord)
                    playerMoveScript.BreathFirstSearch.Noeuds[i].unit = null;
            }
            if(Vector3.Distance(transform.position,worldNextCell) < 0.1f && path.Count-(indexPath+1) >= 0 && actualActionPoint > 1){
                for(int i = 0; i < playerMoveScript.BreathFirstSearch.Noeuds.Count; i++){
                    if(playerMoveScript.BreathFirstSearch.Noeuds[i].coord == nextCell)
                        playerMoveScript.BreathFirstSearch.Noeuds[i].unit = null;
                }
                indexPath++;
                actualActionPoint--;
                nextCell = path[path.Count-indexPath].coord;
            } else if(Vector3.Distance(transform.position,worldNextCell) < 0.1f && path.Count-(indexPath+1) < 0 || Vector3.Distance(transform.position,worldNextCell) < 0.1f && actualActionPoint <= 1){
                indexPath = 2;
                targetActive = false;
                unwalkableCell();
                GetComponent<Animator>().SetBool("moving",false);
                if(path.Count-(indexPath+1) <= 0)
                    GetComponent<Animator>().SetTrigger("attack");
                else
                    GameObject.FindWithTag("TurnController").GetComponent<turnBasedController>().nextZombieTurn();
            }
        }
    }

    void setPath(){
        Node nodeOrigin = new Node(Grille.WorldToCell(transform.position));
        path.Clear();
        for(int i = 0; i < BreathFirstSearch.Noeuds.Count; i++){
            if(BreathFirstSearch.Noeuds[i].coord == Grille.WorldToCell(transform.position))
                nodeOrigin = BreathFirstSearch.Noeuds[i];
        }
        BreathFirstSearch.startBfs(nodeOrigin,10);
        for(int i = 0; i < BreathFirstSearch.Noeuds.Count; i++){
            if(BreathFirstSearch.Noeuds[i].coord == Grille.WorldToCell(player.transform.position)){
                path.Add(BreathFirstSearch.Noeuds[i].previousNode);
            }
        }
        Node previousNode = path[0].previousNode;
        while(previousNode != null){
            path.Add(previousNode);
            previousNode = previousNode.previousNode;
        }
        if(path.Count > 1){
            nextCell = path[path.Count-indexPath].coord;
            targetActive = true;
        } else {
            GetComponent<Animator>().SetTrigger("attack");
        }
    }

    void endAttackAnimation(){
        float coefDamage;
        if(UnityEngine.Random.Range(0f,100f) < Stats.probaHit)
            coefDamage = 1f;
        else
            coefDamage = 0f;
        player.GetComponent<attackScript>().GetAttacked(Stats.damages*coefDamage);
    }


}
