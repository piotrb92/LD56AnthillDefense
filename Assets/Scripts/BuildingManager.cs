using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public Building[] buildings;

    public bool isBuilding;
    public GameObject currentGhost;
    public int currentBuildingIndex;

    private void Update()
    {
        if(currentGhost!= null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            currentGhost.transform.position = mousePos;
        }
    }

    public void StartBuilding()
    {
        isBuilding = true;
        currentGhost = Instantiate(buildings[currentBuildingIndex].ghostPrefab, Vector3.zero, Quaternion.identity);
    }

    public void SetCurrentBuilding(int index)
    {
        CancelBuilding();
        currentBuildingIndex = index;
        if (CheckRequirements())
        {
            StartBuilding();
        }
    }

    public void ConfirmPlacement()
    {
        Building building = buildings[currentBuildingIndex];
        GameObject b = Instantiate(buildings[currentBuildingIndex].completedPrefab, currentGhost.transform.position, currentGhost.transform.rotation);
        GameManager.instance.anthill.wood -= building.reqSticks;
        GameManager.instance.anthill.stone -= building.reqStones;
        GameManager.instance.anthill.web -= building.reqWeb;
        Destroy(currentGhost);
        isBuilding = false;
    }

    public void CancelBuilding()
    {
        isBuilding = false;
        if(currentGhost != null)
        {
            Destroy(currentGhost);
            currentGhost = null;
        }
    }

    public bool CheckRequirements()
    {
        Building b = buildings[currentBuildingIndex];
        if (b.reqSticks > GameManager.instance.anthill.wood) return false;
        if (b.reqStones > GameManager.instance.anthill.stone) return false;
        if (b.reqWeb > GameManager.instance.anthill.web) return false;
        return true;
    }
}
