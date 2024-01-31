using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDscript : MonoBehaviour
{
    [SerializeField] Image LeftHealthBar;
    [SerializeField] Image RightHealthBar;
    [SerializeField] GameObject Bullets;
    [SerializeField] GameObject Batteries;
    public GameObject NextButton;
    [SerializeField] TextMeshProUGUI CostAP;
    GameObject Player;

    void Start(){
        InitVar();
    }

    void InitVar(){
        Player = GameObject.FindWithTag("Player");
    }

    public void refreshAmmo(){
        for(int i = 0; i < Bullets.transform.childCount; i++){
            if(i < Player.GetComponent<attackScript>().ammoLeft)
                Bullets.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.white;
            else
                Bullets.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.black;
        }
    }

    public void refreshAP(){
        for(int i = 0; i < Batteries.transform.childCount; i++){
            if(i < Player.GetComponent<playerMovementScript>().bubblesActionChoice.actualActionPoint)
                Batteries.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.white;
            else
                Batteries.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.black;
        }
    }

    public void displayAPcost(int cost){
        CostAP.gameObject.SetActive(true);
        CostAP.text = "Cost : -"+cost+"AP";
    }

    public void hideAPcost(){
        CostAP.gameObject.SetActive(false);
    }

    public void refreshHealth(){
        float coefHP =Player.GetComponent<attackScript>().HPleft/Player.GetComponent<attackScript>().HPmax;
        //Debug.Log(coefHP);
        if(coefHP > 1-(1/9)){
            RightHealthBar.fillAmount = 1;
            LeftHealthBar.fillAmount = 1;
        }else if(coefHP <= 1f-(1f/9f) && coefHP > 1f-(2f/9f)){
            RightHealthBar.fillAmount = 1f-(1f/9f);
            LeftHealthBar.fillAmount = 1f-(1f/9f);
        }else if(coefHP <= 1f-(2f/9f) && coefHP > 1f-(3f/9f)){
            RightHealthBar.fillAmount = 1f-(2f/9f);
            LeftHealthBar.fillAmount = 1f-(2f/9f);
        }else if(coefHP <= 1f-(3f/9f) && coefHP > 1f-(4f/9f)){
            RightHealthBar.fillAmount = 1f-(3f/9f);
            LeftHealthBar.fillAmount = 1f-(3f/9f);
        }else if(coefHP <= 1f-(4f/9f) && coefHP > 1f-(5f/9f)){
            RightHealthBar.fillAmount = 1f-(4f/9f);
            LeftHealthBar.fillAmount = 1f-(4f/9f);
        }else if(coefHP <= 1f-(5f/9f) && coefHP > 1f-(6f/9f)){
            RightHealthBar.fillAmount = 1f-(5f/9f);
            LeftHealthBar.fillAmount = 1f-(5f/9f);
        }else if(coefHP <= 1f-(6f/9f) && coefHP > 1f-(7f/9f)){
            RightHealthBar.fillAmount = 0.351f;
            LeftHealthBar.fillAmount = 0.351f;
        }else if(coefHP <= 1f-(7f/9f) && coefHP > 1f-(8f/9f)){
            RightHealthBar.fillAmount = 0.243f;
            LeftHealthBar.fillAmount = 0.243f;
        }else if(coefHP <= 1f-(8f/9f) && coefHP > 0f){
            RightHealthBar.fillAmount = 0.13f;
            LeftHealthBar.fillAmount = 0.13f;
        } else if(coefHP <= 0f){
            RightHealthBar.fillAmount = 0;
            LeftHealthBar.fillAmount = 0;
        }
    }
}
