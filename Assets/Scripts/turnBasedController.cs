using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnBasedController : MonoBehaviour
{
    private GameObject player;
    private int indexEnemyTurn;
    [SerializeField] List<GameObject> enemies = new List<GameObject>();
    public bool playersTurn = true;

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
        playersTurn = !playersTurn;

        if(!playersTurn){
            player.GetComponent<attackScript>().VATS.closeInterface();
            UIplayer.changeActivatedMode(3);
            indexEnemyTurn = 0;
            zombieTurn();
        } else {
            GameObject.FindWithTag("Player").GetComponent<attackScript>().HUD.NextButton.SetActive(true);
            UIplayer.actualActionPoint = UIplayer.actionPointMax;
            UIplayer.changeActivatedMode(0);
        }
    }

    public void zombieTurn(){
        enemies[indexEnemyTurn].GetComponent<zombieScript>().actualActionPoint = enemies[indexEnemyTurn].GetComponent<zombieScript>().Stats.actionPointMax;
        enemies[indexEnemyTurn].GetComponent<zombieScript>().turnAction();
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
