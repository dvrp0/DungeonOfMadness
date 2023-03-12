using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Observation
{
    public List<TileBase>[,] Board { get; set; }
    public TileBase Tile { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public Observation(List<TileBase>[,] board, TileBase tile, int x, int y)
    {
        this.Board = board;
        this.Tile = tile;
        this.X = x;
        this.Y = y;
    }
}