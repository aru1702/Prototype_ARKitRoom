using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Manages the label and plane material color for each recognized plane based on
/// the PlaneClassification enumeration defined in ARSubsystems.
/// </summary>

[RequireComponent(typeof(ARPlane))]
[RequireComponent(typeof(MeshRenderer))]
public class PlaneTagging : MonoBehaviour
{
    /// <summary>
    /// AR Camera that used in this script so the text will rotate,
    /// following where the camera move.
    /// </summary>
    [SerializeField]
    Camera m_ARCamera;
    public GameObject ARCamera
    {
        get { return m_ARCamera.gameObject; }
    }

    /// <summary>
    /// If yes, show texts on AR Plane: class, pos, rot
    /// </summary>
    [SerializeField]
    bool m_EnableTextOnPlane = false;
    public bool EnableTextOnPlane
    {
        get { return m_EnableTextOnPlane; }
        set { m_EnableTextOnPlane = value; }
    }

    /// <summary>
    /// If yes, use classification color
    /// If no, just use dark or black transparent
    /// </summary>
    [SerializeField]
    bool m_EnableMultiColorPlane = false;
    public bool EnableMultiColorPlane
    {
        get { return m_EnableMultiColorPlane; }
        set { m_EnableMultiColorPlane = value; }
    }

    ARPlane m_ARPlane;
    MeshRenderer m_PlaneMeshRenderer;
    TextMesh m_TextMesh;
    GameObject m_TextObj;

    Vector3 m_TextFlipVec = new(0, 180, 0);
    string materialPath = "Materials/Transparency_box";

    void OnEnable()
    {
        m_ARPlane = GetComponent<ARPlane>();
        m_PlaneMeshRenderer = GetComponent<MeshRenderer>();

        // Setup label
        m_TextObj = new GameObject();
        if (!m_TextMesh) m_TextMesh = m_TextObj.AddComponent<TextMesh>();
        m_TextMesh.characterSize = 0.05f;
        m_TextMesh.color = Color.black;

        // Setup transparent? (if only no mats assigned, or missing mats)
        var transparent_mats = Resources.Load<Material>(materialPath);
        if (m_PlaneMeshRenderer.materials.Length <= 0)
        {
            if (transparent_mats) GetComponent<Renderer>().material = transparent_mats;
        }
        else if (!m_PlaneMeshRenderer.materials[0])
        {
            if (transparent_mats) GetComponent<Renderer>().material = transparent_mats;
        }
    }

    void Update()
    {
        if (!Camera.main && !m_ARCamera)
        {
            return;
        }

        UpdateLabel();
        UpdatePlaneColor();
    }

    void UpdateLabel()
    {
        if (!m_TextMesh)
        {
            m_TextMesh = m_TextObj.AddComponent<TextMesh>();
            m_TextMesh.characterSize = 0.065f; // change from 0.05 to 0.065
            m_TextMesh.color = Color.black;
        }

        // Update text
        if (m_EnableTextOnPlane)
        {
            m_TextMesh.text =
            string.Format(
                    "{0}\n" +
                    "Pos (cm): {1}\n" +
                    "Rot (Q): {2}"
                    , m_ARPlane.classification.ToString()
                    , GlobalConfig.Vector3inCm(m_ARPlane.transform.position)
                    , m_ARPlane.transform.rotation.ToString()
                );
        }

        if (!m_TextObj) m_TextObj = new GameObject();

        // Update Pose
        m_TextObj.transform.position = m_ARPlane.center;

        if (!Camera.main && m_ARCamera) m_TextObj.transform.LookAt(m_ARCamera.transform);
        else m_TextObj.transform.LookAt(Camera.main.transform);

        m_TextObj.transform.Rotate(m_TextFlipVec);
    }

    void UpdatePlaneColor()
    {
        if (!m_ARPlane) m_ARPlane = GetComponent<ARPlane>();

        Color planeMatColor = Color.gray;

        if (m_EnableMultiColorPlane)
        {
            switch (m_ARPlane.classification)
            {
                case PlaneClassification.None:
                    planeMatColor = Color.gray;
                    break;
                case PlaneClassification.Wall:
                    planeMatColor = Color.cyan;
                    break;
                case PlaneClassification.Floor:
                    planeMatColor = Color.green;
                    break;
                case PlaneClassification.Ceiling:
                    planeMatColor = Color.blue;
                    break;
                case PlaneClassification.Table:
                    planeMatColor = Color.yellow;
                    break;
                case PlaneClassification.Seat:
                    planeMatColor = Color.magenta;
                    break;
                case PlaneClassification.Door:
                    planeMatColor = Color.red;
                    break;
                case PlaneClassification.Window:
                    planeMatColor = Color.clear;
                    break;
            }
        }

        planeMatColor.a = 0.20f;

        m_PlaneMeshRenderer.material.color = planeMatColor;
    }

    void OnDestroy()
    {
        Destroy(m_TextObj);
    }
}
