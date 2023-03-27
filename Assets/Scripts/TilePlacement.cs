using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlacement : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] tileset;


    public void PlaceTiles(List<Vector3Int> positions)
    {
        foreach (Vector3Int pos in positions)
        {
            int tileIndex = Random.Range(0, tileset.Length-1);
            tilemap.SetTile(pos, tileset[tileIndex]);
        }
    }
    public void PlaceTiles(List<Vector3Int> positions, int tileIndex)
    {
        foreach (Vector3Int pos in positions)
        {
            
            tilemap.SetTile(pos, tileset[tileIndex]);
        }
    }
}

