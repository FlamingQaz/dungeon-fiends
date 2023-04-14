    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlacement : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] tileset;
    public TileBase[] floors;
    public TileBase[] sides;
    public TileBase[] bottoms;
    public TileBase[] tops;



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
    public void PlaceFloor(Vector3Int position, int tileIndex)
    {

            tilemap.SetTile(position, floors[tileIndex]);
       
    }
    public void PlaceFloor(Vector3Int position)
    {
        int tileIndex = Random.Range(0, floors.Length - 1);
        tilemap.SetTile(position, floors[tileIndex]);

    }
    public void PlaceCeiling(Vector3Int position, int tileIndex)
    {

        tilemap.SetTile(position, tops[tileIndex]);

    }
    public void PlaceCeiling(Vector3Int position)
    {
        int tileIndex = Random.Range(0, tops.Length - 1);
        tilemap.SetTile(position, tops[tileIndex]);

    }
    public void PlaceBottom(Vector3Int position, int tileIndex)
    {

        tilemap.SetTile(position, bottoms[tileIndex]);

    }
    public void PlaceSide(Vector3Int position)
    {
        int tileIndex = Random.Range(0, sides.Length - 1);
        tilemap.SetTile(position, sides[tileIndex]);

    }
    public void PlaceSide(Vector3Int position, int tileIndex)
    {
        tilemap.SetTile(position, sides[tileIndex]);

    }



}

