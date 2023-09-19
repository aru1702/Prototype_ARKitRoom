using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class Test_RaycastManager_NewARScene : MonoBehaviour
{
    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    [SerializeField]
    GameObject m_ARCamera;

    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    GameObject m_RaycastPrefab;

    [SerializeField]
    GameObject m_ShowTextAboveLocation;

    [SerializeField]
    bool m_ResizePrefabToHalfSize;

    ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    List<GameObject> m_LoadedGameObjects = new();
    GameObject spawnedObject, nearestObject;
    bool m_IsTestMode = true;
    float nearestGO_dis;

    const float RIGHT_SIDE_PANEL_PERC = 0.75f;
    const float BOTTOM_SIDE_PANEL_PERC = 0.25f;

    void Awake()
    {
        m_RaycastManager = m_ARSessionOrigin.GetComponent<ARRaycastManager>();
        nearestObject = new();
    }

    /// <summary>
    ///   Check if user touch the screen
    /// </summary>
    /// <param name="touchPosition">2D position of screen touching</param>
    /// <returns>
    ///   1) Boolean of touch or not, 2) If touch, return 2D position (x,y from bottom left)
    /// </returns>
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return CheckTouchPositionFromRightSide(touchPosition);
        }

        touchPosition = default;
        return false;
    }

    /// <summary>
    /// X is to right, Y is to up
    /// </summary>
    bool CheckTouchPositionFromRightSide(Vector2 touchPosition)
    {
        float screen_width = Screen.currentResolution.width;
        float screen_height = Screen.currentResolution.height;

        float point_X = touchPosition.x;
        float point_Y = touchPosition.y;

        // since it starts from left-bottom
        float perc_screen_width = screen_width * RIGHT_SIDE_PANEL_PERC;
        float perc_screen_height = screen_height * BOTTOM_SIDE_PANEL_PERC;

        if (point_X < perc_screen_width)
            if (point_Y > perc_screen_height)
                return true;
        return false;
    }

    void Update()
    {
        // now will not active if test mode is disabled
        if (!m_IsTestMode) return;

        // if nothing is touching the screen, no proceed
        if (!TryGetTouchPosition(out Vector2 touchPosition)) return;

        //Debug.Log("x: " + touchPosition.x + ", y: " + touchPosition.y);

        // check touch 2D screen position
        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.AllTypes))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;
            Vector3 hitPose_Pos = hitPose.position;
            Quaternion hitPose_Rot = hitPose.rotation;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(
                        m_RaycastPrefab,
                        hitPose_Pos,
                        new Quaternion());
                spawnedObject.SetActive(true);

                if (m_ResizePrefabToHalfSize)
                    spawnedObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                GlobalConfig.DESK_RAYCAST_OBJ = spawnedObject;

                m_ShowTextAboveLocation
                    .GetComponent<Test_ShowLocationAboveObject>()
                    .AddGameObject(spawnedObject);
            }

            spawnedObject.transform.position = hitPose.position;

            // make the text look into camera
            if (m_ARCamera != null) MakingObjectLookAtCamera(spawnedObject, m_ARCamera);
        }
    }

    void MakingObjectLookAtCamera(GameObject GO, GameObject cam)
    {
        GO.transform.rotation = cam.transform.rotation;
        //GO.transform.Rotate(new Vector3(0, 180, 0));

        Debug.Log(GO.transform.rotation.ToString());
        Debug.Log(cam.transform.rotation.ToString());
    }

    public void SetTestMode(bool state)
    {
        m_IsTestMode = state;
    }

    public GameObject GetRaycastObject()
    {
        return spawnedObject;
    }

    public GameObject GetNearestObject()
    {
        return nearestObject;
    }

    public float GetRaycastToNearestDist()
    {
        return nearestGO_dis;
    }
}
