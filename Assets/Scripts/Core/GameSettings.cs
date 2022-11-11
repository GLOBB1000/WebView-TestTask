using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public List<int> countOfSpawnedObjects;
    public List<int> cellSizes;
}
