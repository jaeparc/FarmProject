using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class actionChoiceUI_script : MonoBehaviour
{
    public int activatedMode = 0; //0 pour aucun; 1 pour deplacement; 2 pour tir; 3 pour rechargement;
    [SerializeField] GameObject[] classesChoix;
    public GameObject boutonRetour;
    public TextMeshProUGUI message;
    public TextMeshProUGUI hit;
    GameObject boutonsChoix;

    void Start(){
        InitVar();
    }

    void InitVar(){
        GameObject Player = GameObject.FindWithTag("Player");

        switch(Player.GetComponent<attackScript>().Stats.className){
            case "Hunter":
                for(int i = 0; i < classesChoix.Length; i++){
                    if(classesChoix[i].name.Contains("Chasseur") || classesChoix[i].name.Contains("Hunter"))
                        boutonsChoix = classesChoix[i];
                    else
                        classesChoix[i].SetActive(false);
                }
                break;
            case "Scavenger":
                for(int i = 0; i < classesChoix.Length; i++){
                    if(classesChoix[i].name.Contains("Pillard") || classesChoix[i].name.Contains("Scavenger")){
                        boutonsChoix = classesChoix[i];
                    }else
                        classesChoix[i].SetActive(false);
                }
                break;
            case "Marksman":
                for(int i = 0; i < classesChoix.Length; i++){
                    if(classesChoix[i].name.Contains("Sniper") || classesChoix[i].name.Contains("Marksman"))
                        boutonsChoix = classesChoix[i];
                    else
                        classesChoix[i].SetActive(false);
                }
                break;
        }
        boutonsChoix.SetActive(true);
    }

    public void changeActivatedMode(int idMode){
        HUDscript HUD = GameObject.FindWithTag("Player").GetComponent<attackScript>().HUD;
        activatedMode = idMode;
        if(activatedMode == 0){
            //On fait disparaitre le bouton de retour
            boutonRetour.SetActive(false);
            //On fait apparaitre les boutons de choix
            boutonsChoix.SetActive(true);
            HUD.hideAPcost();
            HUD.refreshAmmo();
            HUD.refreshAP();
            HUD.NextButton.SetActive(true);
        } else if(activatedMode == 1 || activatedMode == 2){
            //On fait disparaitre les boutons proposant le choix
            boutonsChoix.SetActive(false);
            //On fait apparaitre le bouton de retour
            boutonRetour.SetActive(true);
        } else if(activatedMode == 3){
            boutonsChoix.SetActive(false);
            boutonRetour.SetActive(false);
            HUD.NextButton.SetActive(false);
        }
    }

    public void messageAnimation(String messageToDraw){
        Animator anim = GameObject.FindWithTag("Player").GetComponent<Animator>();
        switch(messageToDraw){
            case "NO AMMO":
                message.text = messageToDraw;
                message.color = Color.red;
                anim.SetTrigger("message");
                break;
            case "NOT ENOUGH AP":
                message.text = messageToDraw;
                message.color = Color.red;
                anim.SetTrigger("message");
                break;
            case "MAX AMMO":
                message.text = messageToDraw;
                message.color = new Color(0f,159f/255f,33f/255f,0f);
                anim.SetTrigger("message");
                break;
            case "+1 AMMO":
                message.text = messageToDraw;
                message.color = new Color(0f,159f/255f,33f/255f,0f);
                anim.SetTrigger("message");
                break;
        }
    }
}
