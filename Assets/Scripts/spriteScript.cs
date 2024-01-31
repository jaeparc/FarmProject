using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteScript : MonoBehaviour
{
    void Update()
    {
        faceCamera();
    }

    void faceCamera(){
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        //transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
    }
}
