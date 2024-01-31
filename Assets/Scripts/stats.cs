using UnityEngine;

[CreateAssetMenu(fileName = "stats_Zombie", menuName = "ScriptableObjects/Farm_Project", order = 1)]
public class stats_Zombie : ScriptableObject
{
    [Header("--------------HP--------------")]
    public float headHP;
    public float armsHP;
    public float torsoHP;
    public float legsHP;
    [Header("--------------PROBABILITES--------------")]
    public float headProba;
    public float armsProba;
    public float torsoProba;
    public float legsProba;
    [Header("--------------AP--------------")]
    public int actionPointMax;
    [Header("--------------MOVEMENT--------------")]
    public float speedToNextCell;
    [Header("--------------ATTACK--------------")]
    public float damages;
    public float probaHit;
}