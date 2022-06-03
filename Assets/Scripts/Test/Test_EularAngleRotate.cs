using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EularAngleRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // this basically rotate the object to 30 degree of Y-axis
        // Noted that Unity use left hand rule
        this.gameObject.transform.Rotate(new Vector3(0, 30, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
