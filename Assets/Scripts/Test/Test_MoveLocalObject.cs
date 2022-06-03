using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_MoveLocalObject : MonoBehaviour
{
    [SerializeField]
    GameObject parent, child;

    // Start is called before the first frame update
    void Start()
    {
        child.transform.SetParent(parent.transform, false);
        child.transform.localPosition += new Vector3(-0.5f, 0, 1.5f);
        child.transform.Rotate(new Vector3(90, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
