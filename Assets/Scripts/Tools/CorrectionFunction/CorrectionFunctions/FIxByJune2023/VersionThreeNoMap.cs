using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace CorrectionFunctions
{
    public class VersionThreeNoMap : MonoBehaviour
    {
        // no map but need to see any marker first
        // this will locating where WC should be put in SC
        // then gradually use similar function to version 1.0

        [SerializeField]
        [Tooltip("To import object location.")]
        GameObject m_LoadObjectManager;

        List<GameObject> m_MarkersGroundTruth;

        [SerializeField]
        [Tooltip("Scalar for weight")]
        float m_Scalar = 1.0f;

        [SerializeField]
        [Tooltip("Threshold for removing unnecessary weights")]
        float m_Threshold = 0.85f;

        // the flow:
        // - WorldMapScript will not do anything since there is a trigger
        // - LoadObjectScript still run but it use dummy 0,0,0 origin
        // - Hide all the object by this script (run when enabled)
        // - Do ImageTarget to all marker (not only init marker) in this script
        // - Get the location to deploy precise WC origin, put to PlaySpaceOrigin
        // - Enable ImageTrackingForCorrection from LoadObjectScript
        // - Disable this

        [SerializeField]
        ARSessionOrigin m_ARSessionOrigin;

        ARTrackedImageManager m_ARTrackedImageManager;

        void Awake()
        {
            m_ARTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        }

        void OnEnable()
        {
            m_ARSessionOrigin.GetComponent<ARTrackedImageManager>().enabled = true;
            GetMarkerGroundTruth();
            ShowHideAllObject(false);
            m_ARTrackedImageManager.trackedImagesChanged += OnImageChanged;
        }

        void OnDisable()
        {
            m_ARTrackedImageManager.trackedImagesChanged -= OnImageChanged;
        }

        // OnImageChanged is called when the a marker is captured by camera
        public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
        {
            foreach (var updatedImage in args.updated)
            {
                if (string.Equals(updatedImage.trackingState.ToString(), "Limited")) return;

                foreach (var m in m_MarkersGroundTruth)
                {
                    if (m.name == updatedImage.referenceImage.name)
                    {
                        Vector3 pos = updatedImage.transform.position;
                        Quaternion rot = updatedImage.transform.rotation;

                        GlobalConfig.TempOriginGO.transform.SetPositionAndRotation(pos, rot);
                        ApplyRootTransformation(m);
                        FinishingAndCleaning();
                        return;
                    }
                }
            }
        }

        void GetMarkerGroundTruth()
        {
            m_MarkersGroundTruth = new();

            var parents = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyParents();
            foreach (var item in parents)
            {
                string[] names = item.name.Split("_");
                if (names[0] == "img")
                {
                    m_MarkersGroundTruth.Add(item);
                }
            }
        }

        void ShowHideAllObject(bool t)
        {
            var objects = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyObjects();
            foreach (var o in objects)
            {
                o.SetActive(t);
            }
        }

        void ApplyRootTransformation(GameObject marker)
        {
            MyOrigin item = new MyOrigin(marker.name, marker.transform.parent.name);
            item.position = marker.transform.localPosition;
            item.euler_rotation = marker.transform.localEulerAngles;

            // NEW MECHANIC: 2022-06-07
            // See also: Test_InverseImageToOrigin.cs - MyMethod()

            // ================== //
            // 1. create our root based on imagetarget
            GameObject root = new("root");
            root.transform.SetParent(GlobalConfig.TempOriginGO.transform, false);


            // ================== //
            // 2. make dummy object to inverse the transformation
            GameObject dummy = new();

            // rotate with our root to image target ROTATION data
            dummy = GlobalConfig.RotateOneByOne(dummy, item.euler_rotation);

            // get its inverse of rotation
            Quaternion imageTarget_rotinv = Quaternion.Inverse(dummy.transform.rotation);

            // apply to our root
            root.transform.localRotation = imageTarget_rotinv;


            // ================== //
            // 3. calculate our position with calculating the localToWorldMatrix

            // make our dummy to use the inverse rotation too
            dummy.transform.rotation = imageTarget_rotinv;

            // get the M4x4 matrix of from local to world of our dummy after rotation
            Matrix4x4 mat4 = dummy.transform.localToWorldMatrix;

            // vector multiplication with our root to image target POSITION DATA
            Vector3 vec3 = mat4 * item.position;

            // apply to our root, but inverse it (-)
            root.transform.localPosition = -vec3;


            // ================== //
            // 4. make our root become ROOT now
            root.transform.SetParent(null);
            //imageTarget.transform.SetParent(ourRoot.transform);

            // ================== //
            // 5. finishing

            // destroy the dummy object
            Destroy(dummy);

            GlobalConfig.PlaySpaceOriginGO = root;
        }

        void FinishingAndCleaning()
        {
            ShowHideAllObject(false);
            m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .EnableNewARSceneImageTrackingCorrection();

            GetComponent<VersionOne>().enabled = true;
            GetComponent<VersionOne>().AlsoWithRotatingObject(true);
            GetComponent<VersionOne>().SetMathFunction(MathFunctions.EXP);
            GetComponent<VersionOne>().AddThreshold(true);
            GetComponent<VersionOne>().Threshold(m_Threshold);

            GetComponent<VersionThreeNoMap>().enabled = false;
        }
    }
}
