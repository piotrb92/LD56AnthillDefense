using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    public Tilemap fogTilemap;
    public TileBase fogTile;

    private void Start()
    {
        fogTilemap.gameObject.SetActive(true);
    }

    public void RevealTilesAroundUnit(Vector3 pos, float revealRadius)
    {
        Vector3Int unitTilePosition = fogTilemap.WorldToCell(pos);

        for (int x = -Mathf.FloorToInt(revealRadius); x <= Mathf.FloorToInt(revealRadius); x++)
        {
            for (int y = -Mathf.FloorToInt(revealRadius); y <= Mathf.FloorToInt(revealRadius); y++)
            {
                Vector3Int tilePos = new Vector3Int(unitTilePosition.x + x, unitTilePosition.y + y, unitTilePosition.z);

                float distance = Vector3.Distance(fogTilemap.CellToWorld(tilePos), pos);
                if(distance <= revealRadius)
                {
                    if(fogTilemap.HasTile(tilePos))
                    {
                        fogTilemap.SetTile(tilePos, null);
                    }
                }
            }
        }
    }


}
