using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public List<Creature> selectedCreatures = new List<Creature>();
    public float cameraSpeed = 5;
    public float baseRadius = 0.1f;
    public float formationSpacing = 0.3f;
    public LayerMask interactiveLayer;
    public LayerMask groundLayer;
    public Vector2 cameraLimitMin, cameraLimitMax;
    private bool isSelecting = false;
    private Vector2 initialMousePos, currentMousePos;

    private void Update()
    {
        HandleRightMouseClick();
        HandleLeftMouseClick();
        HandleCameraMovement();
        HandleHotKeys();
       if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.TogglePause();
        }
    }

    public void HandleHotKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectAnts(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectAnts(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectAnts(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectAnts(4);
        }
    }
    /// <summary>
    /// 1 = all
    /// 2 = with loot
    /// 3 = without loot
    /// 4 = first available
    /// 5 = none
    /// </summary>
    /// <param name="n"></param>
    public void SelectAnts(int n)
    {
        Debug.Log("Selecting ants, n = " + n.ToString());
        DeselectAnts();
        if(n == 1)
            selectedCreatures.AddRange(GameManager.instance.playersCreatures);
        else if(n == 2)
            selectedCreatures.AddRange(GameManager.instance.playersCreatures.Where(x => x.currentItem != null));
        else if (n == 3)
            selectedCreatures.AddRange(GameManager.instance.playersCreatures.Where(x => x.currentItem == null));
        else if (n == 4)
            selectedCreatures.Add(GameManager.instance.playersCreatures.Where(x => x.currentItem == null).FirstOrDefault());
        else if (n == 5)
        {
            return;
        }
        for (int i = 0; i < selectedCreatures.Count; i++)
        {
            selectedCreatures[i].Select();
        }
    }

    public void DeselectAnts()
    {
        for (int i = 0; i < selectedCreatures.Count; i++)
        {
            selectedCreatures[i].Deselect();
        }
        selectedCreatures.Clear();
    }

    public void HandleCameraMovement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        Vector3 newPos = transform.position + input * (Input.GetKey(KeyCode.LeftShift) ? cameraSpeed * 2 : cameraSpeed) * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, cameraLimitMin.x, cameraLimitMax.x);
        newPos.y = Mathf.Clamp(newPos.y, cameraLimitMin.y, cameraLimitMax.y);
        transform.position = newPos;
    }

    public void HandleRightMouseClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(GameManager.instance.build.isBuilding)
            {
                GameManager.instance.build.CancelBuilding();
            }
            else
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mousePos, interactiveLayer);
                if (hit != null)
                {
                    Debug.Log("collider = " +hit.gameObject.name);
                    if (hit.CompareTag("Item"))
                    {
                        GatherAntsAroundItem(hit.GetComponent<Item>());
                    }
                    else if (hit.CompareTag("Spider"))
                    {
                        SetItemToPickUp(null);
                        float tempBaseRadius = baseRadius;
                        float tempSpacing = formationSpacing;
                        formationSpacing = 0;
                        baseRadius = 0;
                        NavigateToPosition(hit.transform.position);
                        baseRadius = tempBaseRadius;
                        formationSpacing = tempSpacing;
                    }
                    else if (hit.CompareTag("Anthill"))
                    {
                        SetItemToPickUp(null);
                        float tempBaseRadius = baseRadius;
                        baseRadius = 0;
                        NavigateToPosition(hit.transform.position);
                        baseRadius = tempBaseRadius;
                    }
                }
                else
                {
                    RaycastHit2D rayHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, groundLayer);

                    if (rayHit.collider.CompareTag("Ground"))
                    {
                        Debug.Log("navigating to: " + mousePos.ToString());
                        SetItemToPickUp(null);
                        NavigateToPosition(rayHit.point);
                    }
                }
            }
            
        }
    }

    public void SetItemToPickUp(Item item)
    {
        for (int i = 0; i < selectedCreatures.Count; i++)
        {
            selectedCreatures[i].itemToPickup = (item==null?null:item);
        }
    }

    public void NavigateToPosition(Vector3 position)
    {
        int antCount = selectedCreatures.Count;

        int layer = 0;
        int remainingAnts = antCount;
        int placedAnts = 0;


        while (remainingAnts > 0)
        {
            int antsInLayer = Mathf.Min(remainingAnts, GetAntsInLayer(layer));

            float angleIncrement = 360f / antsInLayer;

            for (int i = 0; i < antsInLayer; i++)
            {
                float angle = i * angleIncrement * Mathf.Deg2Rad;

                Vector2 newPosition = (Vector2)position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (baseRadius + layer * formationSpacing);

                selectedCreatures[placedAnts].controller.targetPosition = newPosition;
                placedAnts++;
            }

            remainingAnts -= antsInLayer;
            layer++;
        }
    }

    public void GatherAntsAroundItem(Item item)
    {
        SetItemToPickUp(item);
        float tempFormationSpacing = formationSpacing;
        formationSpacing = 0;
        NavigateToPosition(item.transform.position);
        formationSpacing = tempFormationSpacing;
        
    }

    private int GetAntsInLayer(int layer)
    {
        return (int)(Mathf.Pow(layer + 1, 2) - Mathf.Pow(layer, 2));
    }

    public void HandleLeftMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (GameManager.instance.build.isBuilding)
            {
                GameManager.instance.build.ConfirmPlacement();
            }
            else
            {
                isSelecting = true;
                initialMousePos = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            SelectCreatures();
        }
    }

    public void SelectCreatures()
    {
        Vector2 worldStart = Camera.main.ScreenToWorldPoint(initialMousePos);
        Vector2 worldEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Rect selectionRect = new Rect(
            Mathf.Min(worldStart.x, worldEnd.x),
            Mathf.Min(worldStart.y, worldEnd.y),
            Mathf.Abs(worldEnd.x - worldStart.x),
            Mathf.Abs(worldEnd.y - worldStart.y));

        DeselectAnts();

        Creature[] allCreatures = GameManager.instance.playersCreatures.ToArray();
        for (int i = 0; i < allCreatures.Length; i++)
        {
            if (selectionRect.Contains(allCreatures[i].transform.position))
            {
                selectedCreatures.Add(allCreatures[i]);
                allCreatures[i].Select();
            }
        }
    }

    public void OnGUI()
    {
        if(isSelecting)
        {
            Rect rect = GetScreenRect(initialMousePos, Input.mousePosition);
            DrawSelectionRectangle(rect);
        }
    }

    private Rect GetScreenRect(Vector2 start, Vector2 end)
    {
        return new Rect(
            Mathf.Min(start.x, end.x),
            Mathf.Min(Screen.height - start.y, Screen.height - end.y),
            Mathf.Abs(end.x - start.x),
            Mathf.Abs(start.y - end.y));
    }

    public void DrawSelectionRectangle(Rect rect)
    {
        GUI.color = new Color(0, 1, 0, 0.25f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
}
