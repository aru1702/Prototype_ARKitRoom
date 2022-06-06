using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_AssignOriginToZero_ForLoadObject : MonoBehaviour
{
    [SerializeField]
    GameObject origin;

    private void Awake()
    {
        GlobalConfig.TempOriginGO = origin;
        GlobalConfig.ITT_VtriPos = origin.transform.position;
        GlobalConfig.ITT_EAngleRot = origin.transform.eulerAngles;
        GlobalConfig.ITT_QuatRot = origin.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        GlobalConfig.ITT_VtriPos = origin.transform.position;
        GlobalConfig.ITT_EAngleRot = origin.transform.eulerAngles;
        GlobalConfig.ITT_QuatRot = origin.transform.rotation;
    }
}
