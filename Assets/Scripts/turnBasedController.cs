using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnBasedController : MonoBehaviour
{
    private GameObject player;
    private int indexEnemyTurn;
    List<GameObject> enemies = new List<GameObject>();
    bool playersTurn = true;

    void Start()
    {
        InitVar();
    }

    void InitVar(){
        player = GameObject.FindWithTag("Player");
        for(int i = 0; i < GameObject.FindGameObjectsWithTag("Enemy").Length; i++)
            enemies.Add(GameObject.FindGameObjectsWithTag("Enemy")[i]);
    }

    public void nextTurn(){
        actionChoiceUI_script UIplayer = player.GetComponent<playerMovementScript>().bubblesActionChoice;
        attackScript attackPlayer = player.GetComponent<attackScript>();
        playersTurn = !playersTurn;

        if(!playersTurn){
            attackPlayer.VATS.closeInterface();
            UIplayer.changeActivatedMode(3);
            indexEnemyTurn = 0;
            zombieTurn();
        } else {
            attackPlayer.HUD.NextButton.SetActive(true);
            attackPlayer.actualActionPoint = attackPlayer.actionPointMax;
            UIplayer.changeActivatedMode(0);
        }
    }

    public void zombieTurn(){
        bool enemiesStillExist = false;
        foreach(GameObject enemy in enemies){
            if(enemy != null)
                enemiesStillExist = true;
        }
        if(enemiesStillExist){
            enemies[indexEnemyTurn].GetComponent<zombieScript>().actualActionPoint = enemies[indexEnemyTurn].GetComponent<zombieScript>().Stats.actionPointMax;
            enemies[indexEnemyTurn].GetComponent<zombieScript>().turnAction();
        } else 
            endGame();
    }

    public void endGame(){

    }

    public void nextZombieTurn(){
        if(indexEnemyTurn < enemies.Count-1){
            indexEnemyTurn++;
            zombieTurn();
        }else{
            nextTurn();
            Debug.Log("ZOMBIE(S) TURN IS DONE, YOU'RE UP.");
        }
    }
}
