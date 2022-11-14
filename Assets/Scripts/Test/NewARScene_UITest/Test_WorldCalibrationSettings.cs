using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_WorldCalibrationSettings : MonoBehaviour
{
    [SerializeField]
    GameObject m_ARCamera;

    bool CheckWorldCalibObj()
    {
        if (GlobalConfig.WORLD_CALIBRATION_OBJ != null)
            return true;

        return false;
    }

    [SerializeField]
    [Tooltip("0.01f equals to 1 cm")]
    float pos_value = 0.01f;

    [SerializeField]
    [Tooltip("1.0f equals to 1 degree (from 360)")]
    float rotE_value = 1.0f;

    GameObject tempGo;

    void CreateGo()
    {
        tempGo = new();
        tempGo.transform.position = m_ARCamera.transform.position;

        GlobalConfig.WORLD_CALIBRATION_OBJ.transform.parent
            = tempGo.transform;
    }

    void DestroyGo()
    {
        GlobalConfig.WORLD_CALIBRATION_OBJ.transform.parent = null;
        Destroy(tempGo);
    }

    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    public void Pos_X_plus()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.position;

        wo.position = new Vector3(wo_v3.x + pos_value, wo_v3.y, wo_v3.z);
    }

    public void Pos_X_minus()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.position;

        wo.position = new Vector3(wo_v3.x - pos_value, wo_v3.y, wo_v3.z);
    }

    /////////////////////////////////////
    
    public void Pos_Y_plus()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.position;

        wo.position = new Vector3(wo_v3.x, wo_v3.y + pos_value, wo_v3.z);
    }

    public void Pos_Y_minus()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.position;

        wo.position = new Vector3(wo_v3.x, wo_v3.y - pos_value, wo_v3.z);
    }

    /////////////////////////////////////

    public void Pos_Z_plus()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.position;

        wo.position = new Vector3(wo_v3.x, wo_v3.y, wo_v3.z + pos_value);
    }

    public void Pos_Z_minus()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.position;

        wo.position = new Vector3(wo_v3.x, wo_v3.y, wo_v3.z - pos_value);
    }

    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    public void Rot_rX_plus()
    {
        if (!CheckWorldCalibObj()) return;

        CreateGo();

        var wo = tempGo.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x + rotE_value, wo_v3.y, wo_v3.z);

        DestroyGo();
    }

    public void Rot_rX_minud()
    {
        if (!CheckWorldCalibObj()) return;

        CreateGo();

        var wo = tempGo.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x - rotE_value, wo_v3.y, wo_v3.z);

        DestroyGo();
    }

    /////////////////////////////////////

    public void Rot_rY_plus()
    {
        if (!CheckWorldCalibObj()) return;

        CreateGo();

        var wo = tempGo.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x, wo_v3.y + rotE_value, wo_v3.z);

        DestroyGo();
    }

    public void Rot_rY_minus()
    {
        if (!CheckWorldCalibObj()) return;

        CreateGo();

        var wo = tempGo.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x, wo_v3.y - rotE_value, wo_v3.z);

        DestroyGo();
    }

    /////////////////////////////////////

    public void Rot_rZ_plus()
    {
        if (!CheckWorldCalibObj()) return;

        CreateGo();

        var wo = tempGo.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x, wo_v3.y, wo_v3.z + rotE_value);

        DestroyGo();
    }

    public void Rot_rZ_minus()
    {
        if (!CheckWorldCalibObj()) return;

        CreateGo();

        var wo = tempGo.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x, wo_v3.y, wo_v3.z - rotE_value);

        DestroyGo();
    }

    //[SerializeField]
    //[Tooltip("Standard calibration object position")]
    //bool m_RecordCalibObjPos = false;
    //public bool GetRecordCalibObjPos() { return m_RecordCalibObjPos; }

    //[SerializeField]
    //[Tooltip("Result of calibration object from clone position")]
    //bool m_RecordCalibObjPosFromCloneOri = false;
    //public bool GetRRecordCalibObjPosFromCloneOri() { return m_RecordCalibObjPosFromCloneOri; }

    //[SerializeField]
    //[Tooltip("Get calibration result of one object that nearest from the camera")]
    //bool m_RecordCalibObjPosFromCloneOriOneObj = false;
    //public bool GetRecordCalibObjPosFromCloneOriOneObj() { return m_RecordCalibObjPosFromCloneOriOneObj; }
}
