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

    public CustomTransform(string custom_name,
                           Transform custom_transform,
                           Vector3 custom_position,
                           Vector3 custom_euler_rotation,
                           Quaternion customer_q_rotation)
                                : this(custom_name, custom_transform)
    {
        this.custom_position = custom_position;
        this.custom_euler_rotation = custom_euler_rotation;
        this.customer_q_rotation = customer_q_rotation;
    }

    public CustomTransform() { }

    public CustomTransform(string custom_name,
                           Vector3 custom_position,
                           Vector3 custom_euler_rotation,
                           Quaternion customer_q_rotation)
    {
        this.custom_name = custom_name;
        this.custom_position = custom_position;
        this.custom_euler_rotation = custom_euler_rotation;
        this.customer_q_rotation = customer_q_rotation;
    }
}
