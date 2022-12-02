using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Changed from angle into position (Nov 29, 2022). 
/// Position returned from world origin (play space) in meters.
/// </summary>
public class Test_OnlyDeviceAngle : MonoBehaviour
{
    [SerializeField]
    GameObject m_ARCamera;

    [SerializeField]
    Text m_ARCameraDebugText;

    // Update is called once per frame
    void Update()
    {
        //Vector3 camEulerAngle = m_ARCamera.transform.eulerAngles;
        //string debugT = string.Format("Device rotate: x:{0}, y:{1}, z:{2}",
        //    camEulerAngle.x.ToString("0.000"),
        //    camEulerAngle.y.ToString("0.000"),
        //    camEulerAngle.z.ToString("0.000"));

        if (GlobalConfig.PlaySpaceOriginGO == null) return;

        var m44 = GlobalConfig.GetM44ByGameObjRef(m_ARCamera, GlobalConfig.PlaySpaceOriginGO);
        var pos = GlobalConfig.GetPositionFromM44(m44);

        string debugT = string.Format("Device position: x:{0}, y:{1}, z:{2} in {3}",
            pos.x.ToString("0.000"),
            pos.y.ToString("0.000"),
            pos.z.ToString("0.000"),
            "meters");

        m_ARCameraDebugText.text = debugT;
    }
}
