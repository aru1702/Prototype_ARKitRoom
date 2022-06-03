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
    public static MyOrigin PlaySpaceMyOrigin;
    public static GameObject TempOriginGO;
    public static GameObject PlaySpaceOriginGO;
    public static bool AlreadyRender = false;

    public static List<Transform> AnchorParentTransform = new();
    public static List<string> AnchorTags = new();
    public const string PLAYER_KEY__ANCHOR_PARENT = "PlayerPrefsAnchorCountKey";
    public const string PLAYER_KEY__ANCHOR_TAG = "PlayerPrefsTag_";
    public static string GetAnchorTag(int count) { return PLAYER_KEY__ANCHOR_TAG + count; }
}
