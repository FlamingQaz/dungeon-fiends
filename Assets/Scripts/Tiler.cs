using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TilePlacement;


public class Tiler : MonoBehaviour
{

        public TilePlacement tilePlacement;

        private void Start()
        {
            List<Vector3Int> positions = new List<Vector3Int>();
            positions.Add(new Vector3Int(0, 0, 0));
            positions.Add(new Vector3Int(1, 0, 0));
            positions.Add(new Vector3Int(2, 0, 0));
            positions.Add(new Vector3Int(3, 0, 0));

            tilePlacement.PlaceTiles(positions);
        }
    }



