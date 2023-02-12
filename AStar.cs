using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AStar
{
    private static Dictionary<Point, Node> nodes;

    private static void CreateNodes()
    {
        nodes = new Dictionary<Point, Node>();

        foreach(TileScript tile in LevelManager.Instance.Tiles.Values)
        {
            nodes.Add(tile.GridPosition, new Node(tile));
        }
    }

    /// <summary>
    /// Genereaza un nou drum cu ajutorul algoritmului A*
    /// </summary>
    /// <param name="start">Inceputul drumului</param>
    public static Stack<Node> GetPath(Point start, Point goal)
    {
        if (nodes == null) //Daca nu avem nodurile atunci este nevoie sa se creeze
        {
            CreateNodes();
        }

        //Creaza o lista deschisa pentru a fi folosita in cadrul algoritmului A*
        HashSet<Node> openList = new HashSet<Node>();

        //Creaza o lista inchisa pentru a fi folosita in cadrul algoritmului A*
        HashSet<Node> closedList = new HashSet<Node>();

        Stack<Node> finalPath = new Stack<Node>();
        
        //Gaseste nodul de start si creeaza o referinta la el numita nodul curent
        Node currentNode = nodes[start];

        //Adauga nodul de start la lista deschisa
        openList.Add(currentNode);

        while(openList.Count > 0)
        {
            //Trece prin toti vecinii
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Point neighbourPos = new Point(currentNode.GridPosition.X - x, currentNode.GridPosition.Y - y);

                    if (LevelManager.Instance.InBounds(neighbourPos) && LevelManager.Instance.Tiles[neighbourPos].WalkAble && neighbourPos != currentNode.GridPosition)
                    {
                        //Initializeaza valoarea costului G cu 0
                        int gCost = 0;

                        if (Math.Abs(x - y) == 1) //Verifica daca trebuie sa avem costul 10
                        {
                            gCost = 10;
                        }
                        else //Daca suntem pe diagonala, costul este 14
                        {
                            if (!ConnectedDiagonally(currentNode, nodes[neighbourPos]))
                            {
                                continue;
                            }
                            gCost = 14;
                        }

                        //Adauga vecinii la lista deschisa
                        Node neighbour = nodes[neighbourPos];

                        if (openList.Contains(neighbour))
                        {
                            if (currentNode.G + gCost < neighbour.G)
                            {
                                neighbour.CalcValues(currentNode, nodes[goal], gCost);
                            }
                        }
                        else if (!closedList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                            neighbour.CalcValues(currentNode, nodes[goal], gCost);
                        }
                    }
                }
            }

            //Muta nodul curent din lista deschisa in lista inchisa
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (openList.Count > 0)
            {
                //Sorteaza lista dupa valoarea lui F si il selecteaza pe primul din lista
                currentNode = openList.OrderBy(n => n.F).First();
            }

            if (currentNode == nodes[goal])
            {
                while (currentNode.GridPosition != start)
                {
                    finalPath.Push(currentNode);
                    currentNode = currentNode.Parent;
                }
                return finalPath;
            }
        }
        return null;
    }

    //Seteaza nodurile conectate diagonal sa nu fie accesibile, astfel incat inamicii sa taie colturile
    private static bool ConnectedDiagonally(Node currentNode, Node neighbour)
    {
        Point direction = neighbour.GridPosition - currentNode.GridPosition;

        Point first = new Point(currentNode.GridPosition.X + direction.X, currentNode.GridPosition.Y);

        Point second = new Point(currentNode.GridPosition.X, currentNode.GridPosition.Y + direction.Y);

        if (LevelManager.Instance.InBounds(first) && !LevelManager.Instance.Tiles[first].WalkAble)
        {
            return false;
        }
        if (LevelManager.Instance.InBounds(second) && !LevelManager.Instance.Tiles[second].WalkAble)
        {
            return false;
        }
        return true;
    }
}
