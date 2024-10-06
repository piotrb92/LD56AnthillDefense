using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building
{
    public string BuildingName;
    public int reqSticks, reqStones, reqWeb;
    public GameObject ghostPrefab, completedPrefab;
}
