using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TestEigenToUnityForiOSVer01 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Debug.Log(GetTrace().ToString());
#endif
    }

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern float GetTrace();
#endif
}
