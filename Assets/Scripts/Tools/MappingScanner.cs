
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARSubsystems;

public class MappingScanner : MonoBehaviour
{
    [SerializeField]
    ARSessionOrigin m_Origin;

    [SerializeField]
    ARSession m_Session;

    List<GameObject> PlaneList = new();
    int countPlane, countPoint, fps;
    string mappingStatus = "None";

    [SerializeField]
    GameObject m_MappingConfigurationUI;

    [SerializeField]
    GameObject m_ARCamera;

    [SerializeField]
    [Tooltip("A line renderer used to outline the ARPlanes in a scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
    LineRenderer m_LineRendererPrefab;

    /// <summary>
    /// Specifies the line renderer that will be used to outline planes in the scene.
    /// </summary>
    /// <value>
    /// A line renderer used to outline planes in the scene.
    /// </value>
    public LineRenderer lineRendererPrefab
    {
        get => m_LineRendererPrefab;
        set => m_LineRendererPrefab = value;
    }

    [SerializeField]
    [Tooltip("A particle system to represent ARPointClouds.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
    ParticleSystem m_PointCloudParticleSystem;

    /// <summary>
    /// Specifies a particle system to visualize an <see cref="ARPointCloud"/>.
    /// </summary>
    /// <value>
    /// A particle system that will visualize point clouds.
    /// </value>
    public ParticleSystem pointCloudParticleSystem
    {
        get => m_PointCloudParticleSystem;
        set => m_PointCloudParticleSystem = value;
    }

    [SerializeField]
    [Tooltip("An ARPlanes plane prefab in a scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
    GameObject m_PlanePrefab;

    /// <summary>
    /// Specifies the line renderer that will be used to outline planes in the scene.
    /// </summary>
    /// <value>
    /// A line renderer used to outline planes in the scene.
    /// </value>
    public GameObject planePrefab
    {
        get => m_PlanePrefab;
        set => m_PlanePrefab = value;
    }

    //[SerializeField]
    //bool enablePointCloudPrefab, enablePlanePrefab = false;

    bool showPointCloud, showPlane;

    GameObject m_PlaneVisualizers;
    int m_NumParticles;
    int m_PreviousTrackingMode = -1;
    XRSessionSubsystem m_SessionSubsystem;
    Configuration m_CurrentConfiguration;
    int m_PreviousConfigCol = -1;

    Dictionary<ARPlane, LineRenderer> m_PlaneLineRenderers = new Dictionary<ARPlane, LineRenderer>();
    Dictionary<ulong, Vector3> m_Points = new Dictionary<ulong, Vector3>();
    ParticleSystem.Particle[] m_Particles;

    void Configure()
    {
        // Debug.Log("Configure()");

        // CURRENTLY UNUSED
        //var planeManager = m_Origin.GetComponent<ARPlaneManager>();
        //if (m_LineRendererPrefab && planeManager && enablePlanePrefab)
        //{
        //    // Debug.Log("line prefab OK, plane manager OK");

        //    m_PlaneVisualizers = new GameObject("PlaneVisualizers");
        //    //m_PlaneVisualizers.SetActive(false);
        //    //m_ShowPlanesButton.interactable = true;
        //    //m_ShowPlanesButton.onValueChanged.AddListener(delegate { TogglePlanesVisibility(); });
        //    planeManager.planesChanged += OnPlaneChanged;
        //}

        var planeManager = m_Origin.GetComponent<ARPlaneManager>();
        //if (enablePlanePrefab)
        //{
            var m_ARPlaneManager = m_Origin.GetComponent<ARPlaneManager>();
            if (!m_ARPlaneManager) return;

            // check if plane prefab hasn't attached, or no prefab at all
            var checkPlanePrefab = m_ARPlaneManager.planePrefab;
            if (planePrefab && !checkPlanePrefab)
                m_ARPlaneManager.planePrefab = m_PlanePrefab;
            else if (!planePrefab && !checkPlanePrefab)
                return;

            //DisplayPlane(true);
            planeManager.planesChanged += OnPlaneChanged;
        //}

        var pointCloudManager = m_Origin.GetComponent<ARPointCloudManager>();
        if (m_PointCloudParticleSystem && pointCloudManager)// && enablePointCloudPrefab)
        {
            // Debug.Log("point cloud particle system OK, point cloud manager OK");

            m_PointCloudParticleSystem = Instantiate(m_PointCloudParticleSystem, m_Origin.trackablesParent);
            var renderer = m_PointCloudParticleSystem.GetComponent<Renderer>();
            //renderer.enabled = false;
            pointCloudManager.pointCloudsChanged += OnPointCloudChanged;
            //m_ShowPointCloudsButton.interactable = true;
            //m_ShowPointCloudsButton.onValueChanged.AddListener(delegate { TogglePointCloudVisibility(renderer); });
        }
    }

    /**
     * Main system
     */

    void OnPlaneChanged(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (var plane in eventArgs.added)
        {
            var planeGameObj = plane.gameObject;
            PlaneList.Add(planeGameObj);
            countPlane++;
            if (countPlane != PlaneList.Count) countPlane = PlaneList.Count;
        }

        foreach (var plane in eventArgs.removed)
        {
            string planeGameObjName = plane.gameObject.name;
            foreach (var listedPlane in PlaneList)
            {
                if (listedPlane.name == planeGameObjName)
                    PlaneList.Remove(listedPlane);
                    countPlane--;
                    if (countPlane != PlaneList.Count) countPlane = PlaneList.Count;
                break;
            }
        }

        DisplayPlane(showPlane);
    }

    //void OnPlaneChanged(ARPlanesChangedEventArgs eventArgs)
    //{
    //    // Debug.Log("enter OnPlaneChange");

    //    foreach (var plane in eventArgs.added)
    //    {
    //        var lineRenderer = GetOrCreateLineRenderer(plane);
    //        UpdateLine(plane, lineRenderer);
    //    }

    //    foreach (var plane in eventArgs.updated)
    //    {
    //        var lineRenderer = GetOrCreateLineRenderer(plane);
    //        UpdateLine(plane, lineRenderer);
    //    }

    //    foreach (var plane in eventArgs.removed)
    //    {
    //        if (m_PlaneLineRenderers.TryGetValue(plane, out var lineRenderer))
    //        {
    //            m_PlaneLineRenderers.Remove(plane);
    //            if (lineRenderer)
    //            {
    //                Destroy(lineRenderer.gameObject);
    //            }
    //        }
    //    }
    //}

    void UpdateLine(ARPlane plane, LineRenderer lineRenderer)
    {
        // Debug.Log("enter UpdateLine");

        if (!lineRenderer)
        {
            return;
        }

        Transform planeTransform = plane.transform;
        bool useWorldSpace = lineRenderer.useWorldSpace;
        if (!useWorldSpace)
        {
            lineRenderer.transform.SetPositionAndRotation(planeTransform.position, planeTransform.rotation);
        }

        var boundary = plane.boundary;
        lineRenderer.positionCount = boundary.Length;
        for (int i = 0; i < boundary.Length; ++i)
        {
            var point2 = boundary[i];
            var localPoint = new Vector3(point2.x, 0, point2.y);
            if (useWorldSpace)
            {
                lineRenderer.SetPosition(i, planeTransform.position + (planeTransform.rotation * localPoint));
            }
            else
            {
                lineRenderer.SetPosition(i, new Vector3(point2.x, 0, point2.y));
            }
        }
    }

    LineRenderer GetOrCreateLineRenderer(ARPlane plane)
    {
        // Debug.Log("enter GetOrCreateLineRenderer");

        if (m_PlaneLineRenderers.TryGetValue(plane, out var foundLineRenderer) && foundLineRenderer)
        {
            return foundLineRenderer;
        }

        var go = Instantiate(m_LineRendererPrefab, m_PlaneVisualizers.transform);
        var lineRenderer = go.GetComponent<LineRenderer>();
        m_PlaneLineRenderers[plane] = lineRenderer;

        return lineRenderer;
    }

    void OnPointCloudChanged(ARPointCloudChangedEventArgs eventArgs)
    {
        // Debug.Log("enter OnPointCloudChanged");

        foreach (var pointCloud in eventArgs.added)
        {
            CreateOrUpdatePoints(pointCloud);
        }

        foreach (var pointCloud in eventArgs.updated)
        {
            CreateOrUpdatePoints(pointCloud);
        }

        foreach (var pointCloud in eventArgs.removed)
        {
            RemovePoints(pointCloud);
        }

        if (showPointCloud) RenderPoints();
    }

    void CreateOrUpdatePoints(ARPointCloud pointCloud)
    {
        // Debug.Log("enter CreateOrUpdatePoints");

        if (!pointCloud.positions.HasValue)
            return;

        var positions = pointCloud.positions.Value;

        // Store all the positions over time associated with their unique identifiers
        if (pointCloud.identifiers.HasValue)
        {
            var identifiers = pointCloud.identifiers.Value;
            for (int i = 0; i < positions.Length; ++i)
            {
                m_Points[identifiers[i]] = positions[i];
            }
        }
    }

    void RemovePoints(ARPointCloud pointCloud)
    {
        // Debug.Log("enter RemovePoints");

        if (!pointCloud.positions.HasValue)
            return;

        var positions = pointCloud.positions.Value;

        if (pointCloud.identifiers.HasValue)
        {
            var identifiers = pointCloud.identifiers.Value;
            for (int i = 0; i < positions.Length; ++i)
            {
                m_Points.Remove(identifiers[i]);
            }

            countPoint = m_Points.Count;
        }
    }

    void RenderPoints()
    {
        // Debug.Log("enter RenderPoints");

        if (m_Particles == null || m_Particles.Length < m_Points.Count)
        {
            m_Particles = new ParticleSystem.Particle[m_Points.Count];
        }

        int particleIndex = 0;
        foreach (var kvp in m_Points)
        {
            SetParticlePosition(particleIndex++, kvp.Value);
        }

        for (int i = m_Points.Count; i < m_NumParticles; ++i)
        {
            m_Particles[i].remainingLifetime = -1f;
        }

        m_PointCloudParticleSystem.SetParticles(m_Particles, System.Math.Max(m_Points.Count, m_NumParticles));
        m_NumParticles = m_Points.Count;

        countPoint = m_Points.Count;
    }

    void SetParticlePosition(int index, Vector3 position)
    {
        // Debug.Log("enter SetParticlePosition");

        m_Particles[index].startColor = m_PointCloudParticleSystem.main.startColor.color;
        m_Particles[index].startSize = m_PointCloudParticleSystem.main.startSize.constant;
        m_Particles[index].position = position;
        m_Particles[index].remainingLifetime = 1f;
    }

    void HighlightCurrentConfiguration(ConfigurationDescriptor currentConfiguration)
    {
        var descriptors = m_SessionSubsystem.GetConfigurationDescriptors(Allocator.Temp);
        int configColumn = -1;
        for (int i = 0; i < descriptors.Length; i++)
        {
            if (descriptors[i] == currentConfiguration)
            {
                configColumn = i;
            }
        }

        //if (m_PreviousConfigCol != -1)
        //{
        //    var prevColAlpha = m_ColumnLabels[m_PreviousConfigCol].color;
        //    prevColAlpha.a = 0.4f;
        //    m_ColumnLabels[m_PreviousConfigCol].color = prevColAlpha;

        //    for (int k = 0; k < m_ConfigurationUI[m_PreviousConfigCol].Count; k++)
        //    {
        //        var newAlpha = m_ConfigurationUI[m_PreviousConfigCol][k].color;
        //        newAlpha.a = 0.4f;
        //        m_ConfigurationUI[m_PreviousConfigCol][k].color = newAlpha;
        //    }
        //}

        //if (configColumn != -1)
        //{
        //    var newColAlpha = m_ColumnLabels[configColumn].color;
        //    newColAlpha.a = 1f;
        //    m_ColumnLabels[configColumn].color = newColAlpha;

        //    for (int j = 0; j < m_ConfigurationUI[configColumn].Count; j++)
        //    {
        //        var newAlpha = m_ConfigurationUI[configColumn][j].color;
        //        newAlpha.a = 1f;
        //        m_ConfigurationUI[configColumn][j].color = newAlpha;
        //    }
        //}

        m_PreviousConfigCol = configColumn;
    }

    void OnEnable()
    {
        Configure();

        //m_PlaneVisualizers.SetActive(true);
        //m_PointCloudParticleSystem.GetComponent<Renderer>().enabled = true;

        SliderShowPlane();
        SliderShowPointCloud();
    }

    void OnDisable()
    {
        if (m_Origin)
        {
            var planeManager = m_Origin.GetComponent<ARPlaneManager>();
            if (planeManager)
            {
                planeManager.planesChanged -= OnPlaneChanged;
            }
        }
    }

    void Update()
    {
        int nowFps = (int)(1.0f / Time.unscaledDeltaTime);
        if (nowFps != fps)
        {
            //m_FpsLabel.text = fps.ToString();
            fps = nowFps;
        }

        var state = (int)m_Session.currentTrackingMode;
        if (state != m_PreviousTrackingMode)
        {
            //m_TrackingModeLabel.text = m_Session.currentTrackingMode.ToString();
            m_PreviousTrackingMode = state;
        }

        //if (m_CameraFollow == true)
        //{
        //    FollowCamera();
        //}

#if UNITY_IOS
        var sessionSubsystem = (ARKitSessionSubsystem)m_Session.subsystem;
        mappingStatus = sessionSubsystem.worldMappingStatus.ToString();
#endif

        // get ARCamera
        Vector3 arCam_Pos = new();
        Quaternion arCam_Rot = new();

        if (m_ARCamera)
        {
            arCam_Pos = m_ARCamera.transform.position;
            arCam_Rot = m_ARCamera.transform.rotation;
        }

        // UpdateTrackingStatus
        if (!m_MappingConfigurationUI) return;
        m_MappingConfigurationUI
            .GetComponent<MappingConfigurationUI_CatExample>()
            .MappingStatusText =
                string.Format(
                        "Detected trackable plane:\n" +
                        "{0} planes\n\n" +

                        "Detected trackable feature point:\n" +
                        "{1} points\n\n" +

                        "FPS: {2}\n" +
                        "Mapping status: {3}\n\n\n" +


                        "AR Camera status:\n" +
                        "Pos (cm):\n{4}\n\n" +
                        "Rot (Q):\n{5}"
                    , countPlane, countPoint, fps, mappingStatus
                    , GlobalConfig.Vector3inCm(arCam_Pos)
                    , arCam_Rot.ToString());
    }

    void LateUpdate()
    {
        //if (!m_ConfigMenuSetup)
        //{
        //    SetupConfigurationMenu();
        //    m_ConfigMenuSetup = true;
        //}

        if (m_SessionSubsystem != null)
        {
            if (m_SessionSubsystem.currentConfiguration.HasValue && m_SessionSubsystem.currentConfiguration.Value != m_CurrentConfiguration)
            {
                m_CurrentConfiguration = (Configuration)m_SessionSubsystem.currentConfiguration;
                HighlightCurrentConfiguration(m_CurrentConfiguration.descriptor);
            }
        }
    }

    /**
     * Additional system
     */

    void DisplayPlane(bool display)
    {
        if (display)
        {
            foreach (var item in PlaneList)
            {
                item.SetActive(true);
            }
        }
        else
        {
            foreach (var item in PlaneList)
            {
                item.SetActive(false);
            }
        }
    }

    public void SliderShowPlane()
    {
        if (m_MappingConfigurationUI
            .GetComponent<MappingConfigurationUI_CatExample>()
            .GetDisplayPlaneSlider())
        {
            showPlane = true;
        }
        else
        {
            showPlane = false;
        }
    }

    public void SliderShowPointCloud()
    {
        if (m_MappingConfigurationUI
            .GetComponent<MappingConfigurationUI_CatExample>()
            .GetDisplayPointCloudSlider())
        {
            showPointCloud = true;
        }
        else
        {
            showPointCloud = false;
        }
    }
}
