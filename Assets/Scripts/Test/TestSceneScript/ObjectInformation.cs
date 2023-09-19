using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInformation : MonoBehaviour
{
    [SerializeField]
    Vector3 position, scale, euler_angle;

    [SerializeField]
    Quaternion rotation;

    [SerializeField]
    Matrix4x4 local_to_world, world_to_local;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        scale = transform.localScale;
        euler_angle = transform.eulerAngles;
        rotation = transform.rotation;
        local_to_world = transform.localToWorldMatrix;
        world_to_local = transform.worldToLocalMatrix;
    }
}
