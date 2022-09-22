using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_WorldCalibrationSettings : MonoBehaviour
{
    bool CheckWorldCalibObj()
    {
        if (GlobalConfig.WORLD_CALIBRATION_OBJ != null)
            return true;

        return false;
    }

    const float pos_value = 0.01f;      // 0.01f equals to 1 cm
    const float rotE_value = 1.0f;      // 1.0f equals to 1 degree (from 360)

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

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x + rotE_value, wo_v3.y, wo_v3.z);
    }

    public void Rot_rX_minud()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x - rotE_value, wo_v3.y, wo_v3.z);
    }

    /////////////////////////////////////

    public void Rot_rY_plus()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x, wo_v3.y + rotE_value, wo_v3.z);
    }

    public void Rot_rY_minus()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x, wo_v3.y - rotE_value, wo_v3.z);
    }

    /////////////////////////////////////

    public void Rot_rZ_plus()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x, wo_v3.y, wo_v3.z + rotE_value);
    }

    public void Rot_rZ_minus()
    {
        if (!CheckWorldCalibObj()) return;

        var wo = GlobalConfig.WORLD_CALIBRATION_OBJ.transform;
        var wo_v3 = wo.eulerAngles;

        wo.eulerAngles = new Vector3(wo_v3.x, wo_v3.y, wo_v3.z - rotE_value);
    }
}
