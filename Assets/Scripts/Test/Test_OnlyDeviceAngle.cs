using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_OnlyDeviceAngle : MonoBehaviour
{
    [SerializeField]
    GameObject m_ARCamera;

    [SerializeField]
    Text m_ARCameraDebugText;

    // Update is called once per frame
    void Update()
    {
        Vector3 camEulerAngle = m_ARCamera.transform.eulerAngles;
        string debugT = string.Format("Device rotate: x:{0}, y:{1}, z:{2}",
            camEulerAngle.x.ToString("0.000"),
            camEulerAngle.y.ToString("0.000"),
            camEulerAngle.z.ToString("0.000"));
        m_ARCameraDebugText.text = debugT;
    }
}
