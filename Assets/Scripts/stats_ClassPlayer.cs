using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "stats_ClassPlayer", menuName = "ScriptableObjects/ClassPlayerStats", order = 1)]
public class stats_ClassPlayer : ScriptableObject
{
    public string className;
    public AnimatorController animator;
    public float HP;
    public int actionPoints;
    [Header("--------------ATTACK--------------")]
    public int damages;
    public int range;
    public int ammo;

}