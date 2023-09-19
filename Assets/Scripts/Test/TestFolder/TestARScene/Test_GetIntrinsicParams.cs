using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Test_GetIntrinsicParams : MonoBehaviour
{
    [SerializeField]
    GameObject m_ARCamera;

    ARCameraManager m_ARCameraManager;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(Hajimaru());
    }

    IEnumerator Hajimaru()
    {
        while (true)
        {
            m_ARCameraManager = m_ARCamera.GetComponent<ARCameraManager>();

            bool intr = m_ARCameraManager.TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics);

            Debug.Log(intr);

            if (intr)
            {
                Debug.Log(cameraIntrinsics.focalLength.ToString());
                Debug.Log(cameraIntrinsics.principalPoint.ToString());
                Debug.Log(cameraIntrinsics.resolution.ToString());
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
