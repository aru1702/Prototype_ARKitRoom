using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTransform
{
    public string custom_name { get; set; }
    public Transform custom_transform { get; set; }
    public Vector3 custom_position { get; set; }
    public Vector3 custom_euler_rotation { get; set; }
    public Quaternion customer_q_rotation { get; set; }

    public CustomTransform(string name, Transform transform)
    {
        custom_name = name;
        custom_transform = transform;
    }

    public CustomTransform() { }
}
