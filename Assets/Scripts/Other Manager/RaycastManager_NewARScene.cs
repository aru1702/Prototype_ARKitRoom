using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class RaycastManager_NewARScene : MonoBehaviour
{
    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    [SerializeField]
    GameObject m_ARCamera;

    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    GameObject m_RaycastPrefab;

    ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    List<GameObject> m_LoadedGameObjects = new();
    GameObject spawnedObject, nearestObject;
    bool m_IsTestMode = true;
    float nearestGO_dis;

    const float RIGHT_SIDE_PANEL_PERC = 0.75f;

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
        float point_X = touchPosition.x;

        // since it starts from left
        float perc_screen_width = screen_width * RIGHT_SIDE_PANEL_PERC;

        if (point_X < perc_screen_width) return true;
        return false;
    }

    void Update()
    {
        // now will not active if test mode is disabled
        if (!m_IsTestMode) return;

        // if nothing is touching the screen, no proceed
        if (!TryGetTouchPosition(out Vector2 touchPosition)) return;

        Debug.Log("x: " + touchPosition.x + ", y: " + touchPosition.y);

        // check touch 2D screen position
        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.AllTypes))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;
            Vector3 hitPose_Pos = hitPose.position;
            Quaternion hitPose_Rot = hitPose.rotation;

            // check the nearest gameobject
            if (m_LoadedGameObjects.Count <= 0)
                m_LoadedGameObjects = m_LoadObjectManager
                    .GetComponent<LoadObject_CatExample_2__NewARScene>()
                    .GetMyObjects();

            nearestGO_dis = 99999;

            foreach (var item in m_LoadedGameObjects)
            {
                float dis = Vector3.Distance(hitPose_Pos, item.transform.position);
                if (dis < nearestGO_dis)
                {
                    nearestObject = item;
                    nearestGO_dis = dis;
                }
            }

            // create game prefab then show text about the distance
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(
                    m_RaycastPrefab,
                    hitPose_Pos,
                    new Quaternion());
                spawnedObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
            }

            // make the text look into camera
            if (m_ARCamera) MakingObjectLookAtCamera(spawnedObject, m_ARCamera);

            // edit text
            string text = nearestObject.name + "\n" + nearestGO_dis;
            spawnedObject
                .GetComponent<CrossLikeText>()
                .SetCrossLikeText(text);

            // add spawnedObject trails to nearest GO
            spawnedObject.AddComponent<CrossLikeText_Trails>();
            spawnedObject.GetComponent<CrossLikeText_Trails>()
                .SetTargetedGameObject(nearestObject);
        }
    }

    void MakingObjectLookAtCamera(GameObject GO, GameObject cam)
    {
        GO.transform.LookAt(cam.transform);
        GO.transform.Rotate(new Vector3(0, 180, 0));
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
