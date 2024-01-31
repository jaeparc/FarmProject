using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class attackScript : MonoBehaviour
{
    [SerializeField] int valueAttack;
    [SerializeField] int rangeOfAttack;
    [SerializeField] int costOfAttack;
    [SerializeField] int costOfReload;
    public int ammoMax;
    public float HPmax;
    public VATS_script VATS;
    public HUDscript HUD;
    [HideInInspector] public GameObject enemyAimed;
    [HideInInspector] public int ammoLeft;
    public float HPleft;
    private actionChoiceUI_script UIplayer;

    void Start(){
        InitVar();
    }

    void InitVar(){
        ammoLeft = ammoMax;
        HPleft = HPmax;
        UIplayer = GetComponent<playerMovementScript>().bubblesActionChoice;
    }

    public void aim(GameObject enemyToAttack){
        enemyAimed = enemyToAttack;
        VATS.openInterface();
        UIplayer.changeActivatedMode(0);
    }

    public void attack(string part){
        float actionPoint = UIplayer.actualActionPoint;
        if(ammoLeft > 0 && actionPoint - costOfAttack >= 0){
            float probaHit = getProba()*enemyAimed.GetComponent<zombieScript>().getProbaPart(part);
            float coefDamage;
            if(UnityEngine.Random.Range(0f,100f) < probaHit)
                coefDamage = 1f;
            else
                coefDamage = 0f;
            enemyAimed.GetComponent<zombieScript>().getAttackedIsDying(part,valueAttack*coefDamage);
            if(enemyAimed.transform.position.x < transform.position.x){
                transform.GetChild(0).localScale = new Vector3(-Mathf.Abs(transform.GetChild(0).localScale.x),transform.GetChild(0).localScale.y,transform.GetChild(0).localScale.z);
                transform.GetChild(1).localScale = new Vector3(-Mathf.Abs(transform.GetChild(1).localScale.x),transform.GetChild(1).localScale.y,transform.GetChild(1).localScale.z);
            }else if(enemyAimed.transform.position.x > transform.position.x){
                transform.GetChild(0).localScale = new Vector3(Mathf.Abs(transform.GetChild(0).localScale.x),transform.GetChild(0).localScale.y,transform.GetChild(0).localScale.z);
                transform.GetChild(1).localScale = new Vector3(Mathf.Abs(transform.GetChild(1).localScale.x),transform.GetChild(1).localScale.y,transform.GetChild(1).localScale.z);
            }
            ammoLeft--;
            UIplayer.actualActionPoint-=costOfAttack;
            HUD.refreshAmmo();
            HUD.refreshAP();
            GetComponent<Animator>().SetTrigger("shoot");
        } else if(ammoLeft <= 0){
            UIplayer.messageAnimation("NO AMMO");
        } else if(actionPoint - costOfAttack < 0)
            UIplayer.messageAnimation("NOT ENOUGH AP");
        VATS.closeInterface();
    }

    public float getProba(){
        Vector3Int cellPosPlayer = GetComponent<playerMovementScript>().Grille.WorldToCell(transform.position);
        Vector3Int cellPosEnemy = GetComponent<playerMovementScript>().Grille.WorldToCell(enemyAimed.transform.position);
        Vector2 startPoint = new Vector2(cellPosPlayer.x,cellPosPlayer.y);
        Vector2 endPoint = new Vector2(cellPosEnemy.x,cellPosEnemy.y);

        Vector2 directionVector = endPoint - startPoint;

        int xDirection = Mathf.RoundToInt(directionVector.x);
        int yDirection = Mathf.RoundToInt(directionVector.y);

        int distanceToEnemy = 0;

        int deplaMax = Mathf.Max(Mathf.Abs(xDirection), Mathf.Abs(yDirection));
        for (int i = 0; i <= deplaMax; i++)
        {
            float k = i;
            float m = deplaMax;
            float t = k / m;
            Vector2 currentPosition = Vector2.Lerp(startPoint, endPoint, t);

            int xCurrent = Mathf.RoundToInt(currentPosition.x);
            int yCurrent = Mathf.RoundToInt(currentPosition.y);

            BFS BreathFirstSearch = GetComponent<playerMovementScript>().BreathFirstSearch;
            for(int j = 0; j < BreathFirstSearch.Noeuds.Count; j++){
                if(BreathFirstSearch.Noeuds[j].coord == new Vector3Int(xCurrent,yCurrent,0) && BreathFirstSearch.Noeuds[j].IsOccupied && new Vector3Int(xCurrent,yCurrent,0) != cellPosEnemy){
                    Debug.Log("SIGHT BLOCKED");
                    return 0;
                }
            }
            distanceToEnemy++;
        }
        float dividende = rangeOfAttack-(distanceToEnemy-2f);
        float diviseur = rangeOfAttack/2f;
        float coefProba = dividende/diviseur;
        if(coefProba > 1)
            coefProba = 1f;
        else if (coefProba < 0f)
            coefProba = 0f;
        return coefProba*100f;
    }

    public void reload(){
        float actionPoint = UIplayer.actualActionPoint;
        if(ammoLeft < ammoMax && actionPoint - costOfReload >= 0){
            GetComponent<Animator>().SetTrigger("reload");
            UIplayer.actualActionPoint-=costOfReload;
            HUD.refreshAP();
        }else if(ammoLeft >= ammoMax)
            UIplayer.messageAnimation("MAX AMMO");
        else if(actionPoint - costOfReload < 0)
            UIplayer.messageAnimation("NOT ENOUGH AP");
        UIplayer.changeActivatedMode(3);
    }

    public void endReloadAnimation(){
        ammoLeft++;
        HUD.refreshAmmo();
        if(ammoLeft < ammoMax)
            UIplayer.messageAnimation("+1 AMMO");
        else
            UIplayer.messageAnimation("MAX AMMO");
    }

    public void endMessageAnimation(){
        UIplayer.changeActivatedMode(0);
    }

    public void GetAttacked(float damagesTaken){
        UIplayer.hit.text = "-"+damagesTaken;
        GetComponent<Animator>().SetTrigger("hit");
        HPleft-=damagesTaken;
        HUD.refreshHealth();
    }

    public void endHitAnimation(){
        GameObject.FindWithTag("TurnController").GetComponent<turnBasedController>().nextZombieTurn();
    }
}
