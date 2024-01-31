using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int Cout;
    public Vector3Int coord;
    public List<Node> voisins = new List<Node>();
    public Node previousNode;
    public GameObject indicator, unit;
    public bool IsOccupied => unit != null;

    public Node(Vector3Int coordToSet){
        coord = coordToSet;
    }
}
