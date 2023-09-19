using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRecognition_CatExample_2__TransferSLAMOrigin : MonoBehaviour
{
    bool m_TransferSLAMOrigin;

    /// <summary>
    /// Check if system wants to transfer SLAM origin
    /// </summary>
    public bool transferSLAMOrigin
    {
        get { return m_TransferSLAMOrigin; }
        set { m_TransferSLAMOrigin = value; }
    }

    GameObject m_DesireOriginGameObject;

    /// <summary>
    /// Attach the desire gameObject as reference for new SLAM origin position
    /// </summary>
    public GameObject desireOriginGameObject
    {
        get { return m_DesireOriginGameObject; }
        set { m_DesireOriginGameObject = value; }
    }

    UnityEngine.XR.ARFoundation.ARSessionOrigin m_ARSessionOrigin;

    public UnityEngine.XR.ARFoundation.ARSessionOrigin arSessionOrigin
    {
        get { return m_ARSessionOrigin; }
        set { m_ARSessionOrigin = value; }
    }

    /// <summary>
    /// Once upon a time...
    /// </summary>
    public void TransferNow()
    {
        if (!m_TransferSLAMOrigin)
        {
            Debug.Log("Transfer SLAM origin system is disabled.");
            return;
        }

        if (!m_DesireOriginGameObject)
        {
            Debug.LogError("No gameObject being assigned as origin.");
            return;
        }

        if (!m_ARSessionOrigin)
        {
            Debug.LogError("No AR session origin has been assigned.");
            return;
        }

        // position and rotation based on current SLAM origin reference
        //Vector3 pos = m_DesireOriginGameObject.transform.position;
        //Quaternion rot = m_DesireOriginGameObject.transform.rotation;

        //m_ARSessionOrigin.MakeContentAppearAt(
        //    m_DesireOriginGameObject.transform,
        //    pos, rot);

        //Debug.Log("before");
        //Debug.Log(m_DesireOriginGameObject.transform.position.ToString());
        //Debug.Log(m_DesireOriginGameObject.transform.rotation.ToString());

        Matrix4x4 SLAMtoMarker = m_DesireOriginGameObject.transform.worldToLocalMatrix;
        Vector3 newPos = SLAMtoMarker.GetPosition();
        Quaternion newRot = Quaternion.LookRotation(
            SLAMtoMarker.GetColumn(2),
            SLAMtoMarker.GetColumn(1));

        try
        {
            m_ARSessionOrigin.gameObject.transform.position = newPos;
            m_ARSessionOrigin.gameObject.transform.rotation = newRot;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error at TransferSLAMOrigin: " + ex);
        }

        //Debug.Log("after");
        //Debug.Log(m_DesireOriginGameObject.transform.position.ToString());
        //Debug.Log(m_DesireOriginGameObject.transform.rotation.ToString());
    }
}
