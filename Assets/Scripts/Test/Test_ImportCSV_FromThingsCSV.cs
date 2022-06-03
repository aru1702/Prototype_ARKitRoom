using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ImportCSV_FromThingsCSV : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<Things> thingsList = Import_FromThings.GetThingsList();

        foreach (var thing in thingsList)
        {
            Debug.Log(thing.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
