using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrail
{
    public string name { set; get; }
    public Vector3 position { set; get; }
    public Quaternion rotation { set; get; }
    public string laps { set; get; }

    public CameraTrail(string name, Vector3 position, Quaternion rotation, string laps)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
        this.laps = laps;
    }
}
