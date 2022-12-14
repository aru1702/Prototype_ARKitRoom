using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrajectoryData
{
    public string Marker_name { get; set; }
    public float Camera_travel_distance { get; set; }
    public float Camera_travel_time { get; set; }
    public string Previous_marker { get; set; }

    public CameraTrajectoryData(string marker_name,
                                float camera_travel_distance,
                                float camera_travel_time,
                                string previous_marker)
    {
        Marker_name = marker_name;
        Camera_travel_distance = camera_travel_distance;
        Camera_travel_time = camera_travel_time;
        Previous_marker = previous_marker;
    }

    public CameraTrajectoryData() { }
}
