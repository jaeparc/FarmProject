using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS
{
    public List<Node> NNV = new List<Node>();
    public List<Node> NV = new List<Node>();
    public List<Node> Noeuds = new List<Node>();

    public BFS(Vector3Int topLeft, Vector3Int bottomRight, GridLayout Grille){
        for(int i = topLeft.x; i <= bottomRight.x; i++){
            for(int j = topLeft.y; j <= bottomRight.y; j++){
                Ray ray = new Ray(Grille.CellToWorld(new Vector3Int(i,j,0))+new Vector3(0.5f,1,0.5f),Vector3.down);
                if(Physics.Raycast(ray,out RaycastHit hit)){
                    if(hit.transform.CompareTag("Walkable"))
                        Noeuds.Add(new Node(new Vector3Int(i,j,0)));
                    else if (hit.transform.CompareTag("Unwalkable")){
                        Node NodeToAdd = new Node(new Vector3Int(i,j,0));
                        NodeToAdd.unit = hit.transform.gameObject;
                        Noeuds.Add(NodeToAdd);
                        hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.white;
                        hit.transform.position += new Vector3(0,0.5f,0);
                    }
                }
            }
        }
        addNeighbours();
    }

    public void addNeighbours(){
        for(int k = 0; k < Noeuds.Count; k++){
            for(int l = 0; l < Noeuds.Count; l++){
                if((Noeuds[l].coord.x == Noeuds[k].coord.x-1 && Noeuds[l].coord.y == Noeuds[k].coord.y ||
                Noeuds[l].coord.x == Noeuds[k].coord.x+1 && Noeuds[l].coord.y == Noeuds[k].coord.y ||
                Noeuds[l].coord.x == Noeuds[k].coord.x && Noeuds[l].coord.y == Noeuds[k].coord.y+1 ||
                Noeuds[l].coord.x == Noeuds[k].coord.x && Noeuds[l].coord.y == Noeuds[k].coord.y-1) && 
                !Noeuds[k].voisins.Contains(Noeuds[l]))
                    Noeuds[k].voisins.Add(Noeuds[l]);
            }
        }
    }

    public void startBfs(Node startingNode, int range){
        NNV.Clear();
        NV.Clear();
        for(int i = 0; i < Noeuds.Count; i++){
            Noeuds[i].previousNode = null;
            Noeuds[i].Cout = 0;
        }
        NNV.Add(startingNode);
        while(NNV.Count > 0){
            for(int i = 0; i < NNV[0].voisins.Count; i++){
                if(!NNV.Contains(NNV[0].voisins[i]) && !NV.Contains(NNV[0].voisins[i]) && NNV[0].Cout+1 <= range && !NNV[0].IsOccupied){
                    NNV.Add(NNV[0].voisins[i]);
                    NNV[0].voisins[i].Cout = NNV[0].Cout+1;
                    NNV[0].voisins[i].previousNode = NNV[0];
                }
            }
            NV.Add(NNV[0]);
            NNV.Remove(NNV[0]);
        }
    }
}
