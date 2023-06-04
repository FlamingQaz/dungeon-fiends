using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    public int numPlayers = 1;
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Ran1");
        Invoke("SpawnPlayers",0.1f); 
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        Debug.Log("Ran2");
        Instantiate(Player, new Vector3(0, 0, -1), Quaternion.identity);
    }

  
}
