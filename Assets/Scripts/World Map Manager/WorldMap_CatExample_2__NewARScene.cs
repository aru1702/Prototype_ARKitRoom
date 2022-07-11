using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

public class WorldMap_CatExample_2__NewARScene : MonoBehaviour
{
    // my modification
    const string myWorldMapName = "catExample_session.worldmap";
    const string myOriginPathName = "catExample_origin.csv";
    bool hasMarkerData = true;

    [Tooltip("The ARSession component controlling the session from which to generate ARWorldMaps.")]
    [SerializeField]
    ARSession m_ARSession;

    /// <summary>
    /// The ARSession component controlling the session from which to generate ARWorldMaps.
    /// </summary>
    public ARSession arSession
    {
        get { return m_ARSession; }
        set { m_ARSession = value; }
    }

    [Tooltip("A UI button component which will load a previously saved ARWorldMap from disk and apply it to the current session.")]
    [SerializeField]
    Button m_LoadButton;

    /// <summary>
    /// A UI button component which will load a previously saved ARWorldMap from disk and apply it to the current session.
    /// </summary>
    public Button loadButton
    {
        get { return m_LoadButton; }
        set { m_LoadButton = value; }
    }

    /// <summary>
    /// Load an <c>ARWorldMap</c> from disk and apply it
    /// to the current session.
    /// </summary>
    public void OnLoadButton()
    {
#if UNITY_IOS
        LoadOriginData();
        StartCoroutine(Load());
#endif
    }

    [SerializeField]
    GameObject m_UIManager;

    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    bool m_AutomaticLoadObjectManager = true;

    /**
      * Load map in coroutine, will wait if async task is performed
      * Example of async task
      * - yield return null
      * - yield return new WaitForSeconds(1)
      * 
      * UPDATE for NewARScene
      * - automatic load when enter scene
      * - use button (in developer mode only)
      */
    IEnumerator Load()
    {
        // open panel
        m_UIManager.GetComponent<UIManager_CatExample>().OpenPanel();
        Text MapStatusText = m_UIManager.GetComponent<UIManager_CatExample>().MapStatus;
        SetText(MapStatusText, "Loading map, please wait...");

        // if only no session subsystem
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
        if (sessionSubsystem == null)
        {
            SetText(MapStatusText, "No session subsystem available. Could not load.");
            yield break;
        }

        // this is kinda tricky
        // sometimes it tells "No file exists", and sometimes into exception
        // if it fells into exception, program will stop
        // so we route them using try-catch
        FileStream file = null;
        try
        {
            file = File.Open(path, FileMode.Open);
            if (file == null)
            {
                string errStr = string.Format("File {0} does not exist.", path);
                SetText(MapStatusText, errStr);
                yield break;
            }
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError(ex);
        }

        Log(string.Format("Reading {0}...", path));

        // reading if file exists
        int bytesPerFrame = 1024 * 10;
        var bytesRemaining = file.Length;
        var binaryReader = new BinaryReader(file);
        var allBytes = new List<byte>();
        while (bytesRemaining > 0)
        {
            var bytes = binaryReader.ReadBytes(bytesPerFrame);
            allBytes.AddRange(bytes);
            bytesRemaining -= bytesPerFrame;
            yield return null;
        }

        var data = new NativeArray<byte>(allBytes.Count, Allocator.Temp);
        data.CopyFrom(allBytes.ToArray());

        Log(string.Format("Deserializing to ARWorldMap...", path));
        ARWorldMap worldMap;
        if (ARWorldMap.TryDeserialize(data, out worldMap))
            data.Dispose();

        if (worldMap.valid)
        {
            Log("Deserialized successfully.");
        }
        else
        {
            SetText(MapStatusText, "Data is not a valid ARWorldMap.");
            yield break;
        }

        Log("Apply ARWorldMap to current session.");
        sessionSubsystem.ApplyWorldMap(worldMap);

        // by this the map should be loaded, but the origin hasn't changed
        // we can load the object but it will takes several seconds to adjust
        // just place the origin var with all zero
        // this because we don't need imageTarget as reference anymore
        Debug.Log("Map loaded!");

        SetText(MapStatusText, "Map loaded!");

        if (hasMarkerData) ActiveLoadObjectManager();
        else SetText(MapStatusText, "Map loaded, but no origin data found! Try to contact your administrator first.");
    }

    /**
      * Load previous saved image tracked from csv
      */
    void LoadOriginData()
    {
        // load csv
        List<string> origindata = new();
        try
        {
            origindata = ImportCSV.getDataPersistentPath(originPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("No csv!");
        }

        if (origindata.Count <= 0)
        {
            Debug.LogError("No marker data!");
            hasMarkerData = false;
            return;
        }

        // attach into each var
        Vector3 pos = new(float.Parse(origindata[0]),
                            float.Parse(origindata[1]),
                            float.Parse(origindata[2]));

        Vector3 eRot = new(float.Parse(origindata[3]),
                            float.Parse(origindata[4]),
                            float.Parse(origindata[5]));

        Quaternion rot = new(float.Parse(origindata[6]),
                            float.Parse(origindata[7]),
                            float.Parse(origindata[8]),
                            float.Parse(origindata[9]));

        GlobalConfig.ITT_VtriPos = pos;
        GlobalConfig.ITT_EAngleRot = eRot;
        GlobalConfig.ITT_QuatRot = rot;

        GameObject origin = new GameObject("tempOrigin");
        GlobalConfig.TempOriginGO = origin;
        origin.transform.SetPositionAndRotation(pos, rot);
    }

    void ActiveLoadObjectManager()
    {
        m_LoadObjectManager.SetActive(true);
        m_LoadObjectManager
            .GetComponent<LoadObject_CatExample_2__NewARScene>()
            .enabled = true;
    }

    string path
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, myWorldMapName);
        }
    }

    string originPath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, myOriginPathName);
        }
    }

    void Awake()
    {
        m_LogMessages = new List<string>();
    }

    void Log(string logMessage)
    {
        m_LogMessages.Add(logMessage);
        Debug.Log(logMessage);
    }

    static void SetActive(Button button, bool active)
    {
        if (button != null)
            button.gameObject.SetActive(active);
    }

    static void SetActive(Text text, bool active)
    {
        if (text != null)
            text.gameObject.SetActive(active);
    }

    static void SetText(Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    /**
      * Enable script, as well as if script being activated
      * If object rendering hasn't started, do Load()
      */
    void OnEnable()
    {
        Debug.Log("WorldMap active");

        if (m_AutomaticLoadObjectManager) OnLoadButton();
    }

    void Update()
    {
#if UNITY_IOS
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
#else
        XRSessionSubsystem sessionSubsystem = null;
#endif
        if (sessionSubsystem == null)
            return;

#if UNITY_IOS
        //SetText(mappingStatusText, string.Format("Mapping Status: {0}", sessionSubsystem.worldMappingStatus));
#endif
    }

    List<string> m_LogMessages;
}