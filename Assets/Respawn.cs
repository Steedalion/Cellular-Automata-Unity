using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Respawn : MonoBehaviour
{
    public MapGenerator mapGenerator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Todo event after new map generated.
            Invoke(nameof(SpawnOnSafeSpot), 3);
        }
    }

    void SpawnOnSafeSpot()
    {
                    gameObject.transform.position = mapGenerator.GetPlaceInMainRoom(1);
                    

    }
}
