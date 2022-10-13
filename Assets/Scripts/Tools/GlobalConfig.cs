using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfig : MonoBehaviour
{
    public static List<GameObject> ThingsList = new();
    public static Things OurWorldOrigin_Things;
    public static GameObject OurWorldOrigin_GameObject;

    public static Transform ImageTargetTransform;
    public static Vector3 ITT_VtriPos;
    public static Vector3 ITT_EAngleRot;
    public static Quaternion ITT_QuatRot;
    public static List<GameObject> MyObjectList = new();
    //public static MyOrigin PlaySpaceMyOrigin;
    public static GameObject TempOriginGO;
    public static GameObject PlaySpaceOriginGO;
    public static bool AlreadyRender = false;

    public static List<Transform> AnchorParentTransform = new();
    public static List<string> AnchorTags = new();
    public const string PLAYER_KEY__ANCHOR_PARENT = "PlayerPrefsAnchorCountKey";
    public const string PLAYER_KEY__ANCHOR_TAG = "PlayerPrefsTag_";
    public static string GetAnchorTag(int count) { return PLAYER_KEY__ANCHOR_TAG + count; }

    /** <summary>
     * Rotation one by one to remove gimbal lock, or smth
     * - note that once rotation done, CS axis will change direction
     * - we should apply with updated CS, then do rotation one by one
     * - by this we can't apply all rotation at once!!!
     * - for example: after X-axis rot, now Y-axis and Z-axis direction have changed
     * </summary>
     */
    public static GameObject RotateOneByOne(GameObject gameObject, Vector3 eulerAngles)
    {
        gameObject.transform.Rotate(Vector3.right, eulerAngles.x);
        gameObject.transform.Rotate(Vector3.up, eulerAngles.y);
        gameObject.transform.Rotate(Vector3.forward, eulerAngles.z);
        return gameObject;
    }

    /// <summary>
    /// Returns Vector3 string from meters scale into centimenters
    /// with 0.01 scale (0.1 milimeters)
    /// </summary>
    /// <param name="vector3"></param>
    /// <returns></returns>
    public static string Vector3inCm(Vector3 vector3)
    {
        float x = vector3.x * 100;
        float y = vector3.y * 100;
        float z = vector3.z * 100;

        return string.Format(
                "({0}, {1}, {2})",
                x.ToString("0.00"), y.ToString("0.00"), z.ToString("0.00")
            );
    }

    /// <summary>
    /// This method return string which better for naming format (yyyy-mm-dd-hh-mm-ss)
    /// </summary>
    /// <returns>String in (yyyy-mm-dd-hh-mm-ss) format</returns>
    public static string GetNowDateandTime()
    {
        string y = System.DateTime.Now.Year.ToString();
        string mo = System.DateTime.Now.Month.ToString();
        string d = System.DateTime.Now.Day.ToString();

        string h = System.DateTime.Now.Hour.ToString();
        string mi = System.DateTime.Now.Minute.ToString();
        string s = System.DateTime.Now.Second.ToString();

        return y + "-" + mo + "-" + d + "-" + h + "-" + mi + "-" + s;
    }

    static int MAPS_NUMBER = 0; // zero is default

    /// <summary>
    /// To select the map based on number, 0 is default map.
    /// </summary>
    public static int MapsSelection
    {
        get { return MAPS_NUMBER; }
        set { MAPS_NUMBER = value; }
    }

    public static float AFTER_LOAD_START_TIME = 0.0f;

    public static bool TEST_MODE = false;

    // These have similarity with MapsSelection MAPS_NUMBER
    // But now can differentially save and load different map number
    public static int SAVE_INTO_MAP = 0;
    public static int LOAD_MAP = 0;


    public static GameObject DESK_RAYCAST_OBJ;
    public static GameObject WORLD_CALIBRATION_OBJ;
    public static bool WORLD_CALIBRATION_ONOFF = false;

    /// <summary>
    /// We get matrix4x4 of "from" gameObject by "reference" gameobject.
    /// Since both does have connectivity to root of Unity, we convert by equation
    /// target-to-refetence = world-to-reference * from-to-world
    /// </summary>
    /// <param name="from">From where gameobject</param>
    /// <param name="reference">Reference gameobject as source</param>
    /// <returns></returns>
    public static Matrix4x4 GetM44ByGameObjRef(GameObject from, GameObject reference)
    {
        return reference.transform.worldToLocalMatrix
            * from.transform.localToWorldMatrix;
    }

    public static Vector3 GetPositionFromM44(Matrix4x4 matrix)
    {
        return matrix.GetPosition();
    }

    public static Quaternion GetRotationFromM44(Matrix4x4 matrix)
    {
        return Quaternion.LookRotation(
                matrix.GetColumn(2), matrix.GetColumn(1));
    }

    public static GameObject GetNearestObject (List<GameObject> goList, GameObject goRef, out int itemC)
    {
        float nearestGO_dis = 99999f;
        itemC = 0;

        for (int i = 0; i < goList.Count; i++)
        {
            float dis = Vector3.Distance(
                goRef.transform.position,
                goList[i].transform.position);

            if (dis < nearestGO_dis)
            {
                itemC = i;
                nearestGO_dis = dis;
            }
        }

        return goList[itemC];
    }
}
