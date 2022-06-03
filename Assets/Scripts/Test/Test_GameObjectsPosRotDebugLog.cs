using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_GameObjectsPosRotDebugLog : MonoBehaviour
{
    public GameObject[] gameObjects;

    // Start is called before the first frame update
    void Start()
    {
        //ARCamera = this.GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObjects.Length <= 0) return;

        string debug_string = "";

        // debug AR camera position relative to the world origin
        foreach (GameObject gO in gameObjects)
        {
            debug_string += gO.name + ", Pos: " + gO.transform.position.ToString() + "\n";
            debug_string += gO.name + ", Rot: " + gO.transform.eulerAngles.ToString() + "\n\n";
        }

        Debug.Log(debug_string);
    }
}
