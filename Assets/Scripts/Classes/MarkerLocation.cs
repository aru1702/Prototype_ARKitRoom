using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerLocation
{
    public string Marker_name { get; set; }
    public Vector3 GT_Position { get; set; }
    public Vector3 GT_EulerAngle { get; set; }
    public Quaternion GT_Rotation { get; set; }
    public Vector3 C_Position { get; set; }
    public Vector3 C_EulerAngle { get; set; }
    public Quaternion C_Rotation { get; set; }
    public string Marker_before { get; set; }

    public MarkerLocation(string marker_name,
                          Vector3 gT_Position, Vector3 gT_EulerAngle,
                          Vector3 c_Position, Vector3 c_EulerAngle,
                          string marker_before)
    {
        Marker_name = marker_name;
        GT_Position = gT_Position;
        GT_EulerAngle = gT_EulerAngle;
        C_Position = c_Position;
        C_EulerAngle = c_EulerAngle;
        Marker_before = marker_before;
    }

    public MarkerLocation() { }
}
