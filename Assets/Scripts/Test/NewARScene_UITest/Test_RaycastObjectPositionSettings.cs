using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_RaycastObjectPositionSettings : MonoBehaviour
{
    bool CheckIfRaycastDeskExist()
    {
        if (GlobalConfig.DESK_RAYCAST_OBJ == null)
            return false;

        return true;
    }

    ///////////////////////////
    /// BUTTON ASSIGNED ///
    ///////////////////////////

    public void X_Plus()
    {
        if (!CheckIfRaycastDeskExist()) return;

        GlobalConfig.DESK_RAYCAST_OBJ.transform.position =
            new Vector3(
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.x + 0.01f,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.y,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.z
                );
    }

    public void X_Minus()
    {
        if (!CheckIfRaycastDeskExist()) return;

        GlobalConfig.DESK_RAYCAST_OBJ.transform.position =
            new Vector3(
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.x - 0.01f,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.y,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.z
                );
    }

    ///////////////////////////

    public void Y_Plus()
    {
        if (!CheckIfRaycastDeskExist()) return;

        GlobalConfig.DESK_RAYCAST_OBJ.transform.position =
            new Vector3(
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.x,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.y + 0.01f,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.z
                );
    }

    public void Y_Minus()
    {
        if (!CheckIfRaycastDeskExist()) return;

        GlobalConfig.DESK_RAYCAST_OBJ.transform.position =
            new Vector3(
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.x,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.y - 0.01f,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.z
                );
    }

    ///////////////////////////

    public void Z_Plus()
    {
        if (!CheckIfRaycastDeskExist()) return;

        GlobalConfig.DESK_RAYCAST_OBJ.transform.position =
            new Vector3(
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.x,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.y,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.z + 0.01f
                );
    }

    public void Z_Minus()
    {
        if (!CheckIfRaycastDeskExist()) return;

        GlobalConfig.DESK_RAYCAST_OBJ.transform.position =
            new Vector3(
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.x,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.y,
                    GlobalConfig.DESK_RAYCAST_OBJ.transform.position.z - 0.01f
                );
    }
}
