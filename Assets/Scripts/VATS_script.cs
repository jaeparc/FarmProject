using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VATS_script : MonoBehaviour
{
    private zombieScript zombieAimed;
    private GameObject player;

    [Header("       Ammo")]
    public GameObject[] bullet;

    [Header("       Probabilités")]
    [SerializeField] TextMeshProUGUI headProba;
    [SerializeField] TextMeshProUGUI torsoProba;
    [SerializeField] TextMeshProUGUI armsProba;
    [SerializeField] TextMeshProUGUI legsProba;

    [Header("       HP")]
    [SerializeField] TextMeshProUGUI headHP;
    [SerializeField] TextMeshProUGUI torsoHP;
    [SerializeField] TextMeshProUGUI armsHP;
    [SerializeField] TextMeshProUGUI legsHP;
    

    public void openInterface(){
        player = GameObject.FindWithTag("Player");
        zombieAimed = player.GetComponent<attackScript>().enemyAimed.GetComponent<zombieScript>();
        setProbaText();
        setHPtext();
        updateAmmo();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void setProbaText(){
        attackScript playerAttackScript = player.GetComponent<attackScript>();

        headProba.text = "Head : "+Mathf.RoundToInt(playerAttackScript.getProba()*zombieAimed.getProbaPart("head"))+"%";
        torsoProba.text = "Torso : "+Mathf.RoundToInt(playerAttackScript.getProba()*zombieAimed.getProbaPart("torso"))+"%";
        armsProba.text = "Arms : "+Mathf.RoundToInt(playerAttackScript.getProba()*zombieAimed.getProbaPart("arms"))+"%";
        legsProba.text = "Legs : "+Mathf.RoundToInt(playerAttackScript.getProba()*zombieAimed.getProbaPart("legs"))+"%";
    }

    public void setHPtext(){
        headHP.text = "Head : "+Mathf.RoundToInt(zombieAimed.headHP)+"HP";
        torsoHP.text = "Torso : "+Mathf.RoundToInt(zombieAimed.torsoHP)+"HP";
        armsHP.text = "Arms : "+Mathf.RoundToInt(zombieAimed.armsHP)+"HP";
        legsHP.text = "Legs : "+Mathf.RoundToInt(zombieAimed.legsHP)+"HP";
    }

    public void closeInterface(){
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void updateAmmo(){
        for(int i = 0; i < bullet.Length; i++){
            if(i < player.GetComponent<attackScript>().ammoLeft)
                bullet[i].SetActive(true);
            else
                bullet[i].SetActive(false);
        }
    }
}
