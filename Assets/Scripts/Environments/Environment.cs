using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Environment : ScriptableObject, IEnvironment
{
    public Tilemap Base;
    public Tilemap Input;
    public Transform SpecialObject;
    public Color Color;
    public int Ammos;

    public abstract IEnumerator Attack(Player player);
}