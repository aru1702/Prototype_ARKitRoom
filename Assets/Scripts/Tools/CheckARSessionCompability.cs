using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;
using UnityEngine.UI;

public class CheckARSessionCompability : MonoBehaviour
{
    [SerializeField]
    ARSession m_ARSession;

    [SerializeField]
    Text m_StatusText;

    void OnEnable()
    {
        if (!DeviceSupport)
        {
            if (m_StatusText)
            {
                m_StatusText.gameObject.SetActive(true);
                m_StatusText.text = "This device cannot support ARKit world map system.\n" +
                    "We are very sorry but currently world map system only available in iOS devices.";
            }
        }
    }

    bool DeviceSupport
    {
        get
        {
#if UNITY_IOS
            return m_ARSession.subsystem is ARKitSessionSubsystem && ARKitSessionSubsystem.worldMapSupported;
#else
            return false;
#endif
        }
    }
}
