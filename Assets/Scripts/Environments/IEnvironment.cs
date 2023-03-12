using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IEnvironment
{
    IEnumerator Attack(Player player);
}