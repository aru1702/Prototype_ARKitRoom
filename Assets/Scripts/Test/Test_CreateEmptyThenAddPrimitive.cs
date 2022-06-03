using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_CreateEmptyThenAddPrimitive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int a = 0;

        GameObject gameObject;
        if (a == 1) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube); }
        else { gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere); }
        gameObject.name = "Cube1";
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
