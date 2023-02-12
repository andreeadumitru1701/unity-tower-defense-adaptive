using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    /// <summary>
    /// Pozitia pe grila a nodurilor
    /// </summary>
    public Point GridPosition { get; private set; }

    /// <summary>
    /// O referinta la placa careia ii apartine nodul
    /// </summary>
    public TileScript TileRef { get; private set; }

    public Vector2 WorldPosition { get; set; }

    /// <summary>
    /// O referinta la parintele nodului
    /// </summary>
    public Node Parent { get; private set; }

    public int G { get; set; }

    public int H { get; set; }

    public int F { get; set; }

    /// <summary>
    /// Constructorul nodului
    /// </summary>
    /// <param name="tileRef">O referinta la tilescript</param>
    public Node(TileScript tileRef)
    {
        this.TileRef = tileRef;
        this.GridPosition = tileRef.GridPosition;
        this.WorldPosition = tileRef.WorldPosition;
    }

    /// <summary>
    /// Calculeaza valorile pentru nod
    /// </summary>
    /// <param name="parent">Parintele nodului</param>
    /// <param name="gScore"></param>
    public void CalcValues(Node parent, Node goal, int gCost)
    {
        this.Parent = parent;
        this.G = parent.G + gCost;
        this.H = ((Math.Abs(GridPosition.X - goal.GridPosition.X))+Math.Abs((goal.GridPosition.Y-GridPosition.Y))) * 10;
        this.F = G + H;
    }

}
