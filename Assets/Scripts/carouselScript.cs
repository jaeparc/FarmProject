using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class carouselScript : MonoBehaviour
{
    [Header("--------------OBJETS A REMPLIR--------------")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] GameObject[] objectsPrefab;
    [SerializeField] TextMeshProUGUI nameObjects;
    [Header("--------------VALEURS A REMPLIR--------------")]
    [SerializeField] float rayon;
    [SerializeField] float speedMovement;
    [SerializeField] float speedRotation;
    bool moving = false;
    bool positionsSet = false;
    string direction;
    List<float> angles = new List<float>();
    List<Vector3> positions = new List<Vector3>();
    List<GameObject> objectsInstances = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        Controls();
        Move();
        Rotation();
    }

    void Init(){
        Vector3 positionCamera = cameraTransform.position;
        transform.position = positionCamera+new Vector3(0f,-1f,15.5f);
        Vector3 vecteurCamera = positionCamera-transform.position;
        Vector3 posFirstGameObject = new Vector3(transform.position.x+(rayon*Mathf.Cos(0f)),0,transform.position.z+(rayon*Mathf.Sin(0)));
        Vector3 vecteurFirstGameObject = posFirstGameObject-transform.position;

        float produitScalaire = (vecteurCamera.x*vecteurFirstGameObject.x)+(vecteurCamera.z*vecteurFirstGameObject.z);
        float normVecteurCamera = Mathf.Sqrt((vecteurCamera.x*vecteurCamera.x)+(vecteurCamera.z*vecteurCamera.z));
        float normVecteurFirstGameObject = Mathf.Sqrt((vecteurFirstGameObject.x*vecteurFirstGameObject.x)+(vecteurFirstGameObject.z*vecteurFirstGameObject.z));

        float angle = -Mathf.Acos(produitScalaire/(normVecteurCamera*normVecteurFirstGameObject))*(180/Mathf.PI);

        for(int i = 0; i < objectsPrefab.Length; i++){
            float x = transform.position.x+(rayon*Mathf.Cos((angle*Mathf.PI)/180));
            float z = transform.position.z+(rayon*Mathf.Sin((angle*Mathf.PI)/180));
            GameObject instanceToAdd = Instantiate(objectsPrefab[i],new Vector3(x,transform.position.y,z),Quaternion.identity,transform);
            instanceToAdd.name = instanceToAdd.name.Replace("(Clone)", "");
            objectsInstances.Add(instanceToAdd);
            angle += 360f/objectsPrefab.Length;
        }
    }

    void Controls(){
        if(Input.GetKey(KeyCode.RightArrow)){
            moving = true;
            direction = "right";
        }
        else if(Input.GetKey(KeyCode.LeftArrow)){
            moving = true;
            direction = "left";
        }
        else if(Input.GetKeyUp(KeyCode.Return) && !moving){
            Select();
        }
    }

    void Select(){
        GameObject gameObjectSelected = null;
        float closestDistance = Mathf.Infinity;
        foreach(GameObject objects in objectsInstances){
            if(Vector3.Distance(cameraTransform.position,objects.transform.position) < closestDistance){
                gameObjectSelected = objects;
                closestDistance = Vector3.Distance(cameraTransform.position,objects.transform.position);
            }
        }
        switch(gameObjectSelected.name){
            case "Continue":
                transform.parent.gameObject.SetActive(false);
                break;
            case "Quit":
                break;
        }
    }

    void Rotation(){
        if(!moving){
            float realSpeed = speedRotation*Time.deltaTime; 
            GameObject gameObjectToRotate = null;
            float closestDistance = Mathf.Infinity;
            foreach(GameObject objects in objectsInstances){
                if(Vector3.Distance(cameraTransform.position,objects.transform.position) < closestDistance){
                    gameObjectToRotate = objects;
                    closestDistance = Vector3.Distance(cameraTransform.position,objects.transform.position);
                }
            }
            gameObjectToRotate.transform.Rotate(new Vector3(0,realSpeed,0));
            nameObjects.text = gameObjectToRotate.name;
        }
    }

    void Move(){
        if(moving){
            if(!positionsSet){
                Vector3 positionCamera = GameObject.FindWithTag("MainCamera").transform.position;
                Vector3 vecteurCamera = positionCamera-transform.position;
                Vector3 posFirstGameObject = new Vector3(transform.position.x+(rayon*Mathf.Cos(0f)),transform.position.y,transform.position.z+(rayon*Mathf.Sin(0)));
                Vector3 vecteurFirstGameObject = posFirstGameObject-transform.position;

                float produitScalaire = (vecteurCamera.x*vecteurFirstGameObject.x)+(vecteurCamera.z*vecteurFirstGameObject.z);
                float normVecteurCamera = Mathf.Sqrt((vecteurCamera.x*vecteurCamera.x)+(vecteurCamera.z*vecteurCamera.z));
                float normVecteurFirstGameObject = Mathf.Sqrt((vecteurFirstGameObject.x*vecteurFirstGameObject.x)+(vecteurFirstGameObject.z*vecteurFirstGameObject.z));

                float angle = -Mathf.Acos(produitScalaire/(normVecteurCamera*normVecteurFirstGameObject))*(180/Mathf.PI);
                for(int i = 0; i < objectsInstances.Count; i++){
                    positions.Add(objectsInstances[i].transform.position);
                    angles.Add(angle);
                    angle += 360f/objectsInstances.Count;
                }
                positionsSet = true;
            }
            float realSpeedMovement = speedMovement*Time.deltaTime;
            float angleChangement = (360f/objectsInstances.Count)*realSpeedMovement/360f;
            if(direction == "right"){
                angleChangement = -angleChangement;
            }
            for(int i = 0; i < objectsInstances.Count; i++){
                angles[i]+=angleChangement;
                float x = transform.position.x+(rayon*Mathf.Cos((angles[i]*Mathf.PI)/180));
                float z = transform.position.z+(rayon*Mathf.Sin((angles[i]*Mathf.PI)/180));
                objectsInstances[i].transform.position = new Vector3(x,transform.position.y,z);
                Vector3 targetPosition = Vector3.zero;
                if(direction == "left"){
                    if(i+1 >= positions.Count) 
                        targetPosition = positions[0];
                    else
                        targetPosition = positions[i+1];
                } else if(direction == "right"){
                    if(i-1 < 0) 
                        targetPosition = positions[positions.Count-1];
                    else
                        targetPosition = positions[i-1];
                }
                Debug.Log(objectsInstances[i].transform.position+" "+targetPosition);
                //Debug.Log("POSITION:"+objectsInstances[i].transform.position+";TARGET:"+targetPosition+";DISTANCE:"+Vector3.Distance(objectsInstances[i].transform.position,targetPosition));
                if(Vector3.Distance(objectsInstances[i].transform.position,targetPosition) < 0.1f){
                    endMove();
                    break;
                }
            }
        }
    }

    void endMove(){
        for(int i = 0; i < objectsInstances.Count; i++){
            Vector3 targetPosition = Vector3.zero;
            if(direction == "left"){
                if(i+1 >= positions.Count) 
                    targetPosition = positions[0];
                else
                    targetPosition = positions[i+1];
            } else if(direction == "right"){
                if(i-1 < 0) 
                    targetPosition = positions[positions.Count-1];
                else
                    targetPosition = positions[i-1];
            }
            objectsInstances[i].transform.position = targetPosition;
        }
        moving = false;
        positionsSet = false;
        direction = "";
        positions.Clear();
    }
}
