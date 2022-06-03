using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// src: https://www.andreasjakl.com/raycast-anchor-placing-ar-foundation-holograms-part-3/
// notes:
// - [SerializeField] means to make private field assignable in Unity Editor

public class Test_ARPlaceHologram : MonoBehaviour
{
    // The prefab to instantiate on touch.
    [SerializeField]
    private GameObject _prefabToPlace;

    [SerializeField]
    private bool _createPlaneOK;

    // Cache ARRaycastManager GameObject from ARCoreSession
    private ARRaycastManager _raycastManager;

    // Cache ARAnchorManager GameObject from XROrigin
    private ARAnchorManager _anchorManager;

    // List for raycast hits is re-used by raycast manager
    private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    // Unity ARKit Plugin - WorldMapManager
    //public WorldMapManager worldMapManager;

    // Save and Load button
    [SerializeField]
    private Button SaveButton;

    [SerializeField]
    private Button LoadButton;

    void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        _anchorManager = GetComponent<ARAnchorManager>();
    }

    void Update()
    {
        // Only consider single-finger touches that are beginning
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) { return; }

        // Perform AR raycast to any kind of trackable
        if (_raycastManager.Raycast(touch.position, Hits, TrackableType.AllTypes))
        {
            // Raycast hits are sorted by distance, so the first one will be the closest hit.
            //var hitPose = Hits[0].pose;

            // Instantiate the prefab at the given position
            // Note: the object is not anchored yet!
            // got replaced --> Instantiate(_prefabToPlace, hitPose.position, hitPose.rotation);

            // Now the object is anchored!
            CreateAnchor(Hits[0]);

            // Debug output what we actually hit
            Debug.Log($"Instantiated on: {Hits[0].hitType}");
        }
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor;

        // ... here, we'll place the plane anchoring code!

        // If we hit a plane, try to "attach" the anchor to the plane
        if (hit.trackable is ARPlane plane && _createPlaneOK)
        {
            //var planeManager = GetComponent<ARPlaneManager>();
            //if (planeManager)
            //{
                var oldPrefab = _anchorManager.anchorPrefab;
                _anchorManager.anchorPrefab = _prefabToPlace;
                anchor = _anchorManager.AttachAnchor(plane, hit.pose);
                _anchorManager.anchorPrefab = oldPrefab;

                Debug.Log($"Created anchor attachment for plane (id: {anchor.nativePtr}).");
                //return anchor;
            //}
        }
        else
        {

            // Otherwise, just create a regular anchor at the hit pose

            // Note: the anchor can be anywhere in the scene hierarchy
            var instantiatedObject = Instantiate(_prefabToPlace, hit.pose.position, hit.pose.rotation);

            // Make sure the new GameObject has an ARAnchor component
            anchor = instantiatedObject.GetComponent<ARAnchor>();
            if (anchor == null)
            {
                anchor = instantiatedObject.AddComponent<ARAnchor>();
            }

            Debug.Log($"Created regular anchor (id: {anchor.nativePtr}).");
        }

        //AnchorCountUp(anchor);

        return anchor;
    }

    private void AnchorCountUp(ARAnchor arAnchor)
    {
        string a = arAnchor.nativePtr.ToString();

        Vector3 aPos = arAnchor.transform.position;
        string aT = aPos.x + "," + aPos.y + "," + aPos.z;

        Quaternion aRot = arAnchor.transform.rotation;
        string aR = aRot.x + "," + aRot.y + "," + aRot.z + "," + aRot.w;

        GlobalConfig.AnchorParentTransform.Add(arAnchor.transform);
        GlobalConfig.AnchorTags.Add(a + "," + aT + "," + aR);

        Debug.Log("new anchor: " + a + "," + aT + "," + aR);
    }

    public void SaveBtn()
    {
        // save how many of them
        int updatedAnchorCounts = GlobalConfig.AnchorParentTransform.Count;
        PlayerPrefs.SetInt(GlobalConfig.PLAYER_KEY__ANCHOR_PARENT, updatedAnchorCounts);

        // save for each anchor
        for (int i = 0; i < GlobalConfig.AnchorParentTransform.Count; i++)
        {
            PlayerPrefs.SetString(GlobalConfig.GetAnchorTag(i), GlobalConfig.AnchorTags[i]);
        }

        // save PlayerPrefs
        PlayerPrefs.Save();

        //worldMapManager.Save();
    }

    public void LoadBtn()
    {
        //worldMapManager.Load();

        // remove current scene
        foreach (Transform eachAnchor in GlobalConfig.AnchorParentTransform)
        {
            Destroy(eachAnchor.gameObject);
        }

        int howManyAnchors = PlayerPrefs.GetInt(GlobalConfig.PLAYER_KEY__ANCHOR_PARENT, 0);
        for (int i = 0; i < howManyAnchors; i++)
        {
            string anchorTagName = PlayerPrefs.GetString(GlobalConfig.GetAnchorTag(i));
            Debug.Log("Name: " + anchorTagName);
            string[] anchorTagNameStr = anchorTagName.Split(",");
            Vector3 anchorPos = new(float.Parse(anchorTagNameStr[1]),
                                    float.Parse(anchorTagNameStr[2]),
                                    float.Parse(anchorTagNameStr[3]));
            Quaternion anchorRot = new(float.Parse(anchorTagNameStr[4]),
                                    float.Parse(anchorTagNameStr[5]),
                                    float.Parse(anchorTagNameStr[6]),
                                    float.Parse(anchorTagNameStr[7]));

            // restore
            Instantiate(_prefabToPlace, anchorPos, anchorRot);
        }
    }
}
