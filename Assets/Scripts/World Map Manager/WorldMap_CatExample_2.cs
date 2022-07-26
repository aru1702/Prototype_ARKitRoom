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

public class WorldMap_CatExample_2 : MonoBehaviour
{
    // my modification
    const string myWorldMapName = "catExample_session.worldmap";
    const string myOriginPathName = "catExample_origin.csv";
    float startTime;

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

    [Tooltip("A UI button component which will generate an ARWorldMap and save it to disk.")]
    [SerializeField]
    Button m_SaveButton;

    /// <summary>
    /// A UI button component which will generate an ARWorldMap and save it to disk.
    /// </summary>
    public Button saveButton
    {
        get { return m_SaveButton; }
        set { m_SaveButton = value; }
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
    /// Create an <c>ARWorldMap</c> and save it to disk.
    /// </summary>
    public void OnSaveButton()
    {
#if UNITY_IOS
        SaveOriginData();
        StartCoroutine(Save());
#endif
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
    GameObject UIManager;

#if UNITY_IOS
    /**
      * Save map in coroutine, will wait if async task is performed
      * Example of async task
      * - yield return null
      * - yield return new WaitForSeconds(1)
      */
    IEnumerator Save()
    {
        // open panel
        Text MapStatusText = UIManager.GetComponent<UIManager_CatExample>().MapStatus;

        SetText(MapStatusText, "Saving map, please wait...");
        UIManager.GetComponent<UIManager_CatExample>().OpenPanel();

        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
        if (sessionSubsystem == null)
        {
            SetText(MapStatusText, "No session subsystem available. Could not save.");
            UIManager.GetComponent<UIManager_CatExample>().OpenPanel();
            yield break;
        }

        var request = sessionSubsystem.GetARWorldMapAsync();

        while (!request.status.IsDone())
            yield return null;

        if (request.status.IsError())
        {
            string errStr = string.Format("Session serialization failed with status {0}", request.status);
            SetText(MapStatusText, errStr);
            UIManager.GetComponent<UIManager_CatExample>().OpenPanel();
            yield break;
        }

        var worldMap = request.GetWorldMap();
        request.Dispose();

        SaveAndDisposeWorldMap(worldMap);

        // At this moment, the map should be saved into the defined path
        // It saves the AR Session Origin, which means our AR origin (0,0,0)
        Debug.Log("Map saved!");

        float endTime = Time.time;
        float time_spend = System.Math.Abs(endTime - startTime);

        SetText(MapStatusText, "Map tracked for " + time_spend.ToString("0.00") + " secs., and saved successfully!");
        UIManager.GetComponent<UIManager_CatExample>().OpenPanel();
    }

    /**
      * Save and dispose the current map
      * I don't know why we have to dispose the current map after save id
      * It should just continue, otherwise the user will overwrite when save it again
      * If using "Save and Quit", this is good
      */
    void SaveAndDisposeWorldMap(ARWorldMap worldMap)
    {
        Log("Serializing ARWorldMap to byte array...");
        var data = worldMap.Serialize(Allocator.Temp);
        Log(string.Format("ARWorldMap has {0} bytes.", data.Length));

        var file = File.Open(path, FileMode.Create);
        var writer = new BinaryWriter(file);
        writer.Write(data.ToArray());
        writer.Close();
        data.Dispose();
        worldMap.Dispose();
        Log(string.Format("ARWorldMap written to {0}", path));
    }
#endif

    /**
      * Save previous image tracked into csv
      */
    void SaveOriginData()
    {
        // create long string
        string strLong = "";
        strLong += GlobalConfig.ITT_VtriPos.x + ",";        // 0
        strLong += GlobalConfig.ITT_VtriPos.y + ",";        // 1
        strLong += GlobalConfig.ITT_VtriPos.z + ",";        // 2
        strLong += GlobalConfig.ITT_EAngleRot.x + ",";
        strLong += GlobalConfig.ITT_EAngleRot.y + ",";
        strLong += GlobalConfig.ITT_EAngleRot.z + ",";
        strLong += GlobalConfig.ITT_QuatRot.x + ",";
        strLong += GlobalConfig.ITT_QuatRot.y + ",";
        strLong += GlobalConfig.ITT_QuatRot.z + ",";
        strLong += GlobalConfig.ITT_QuatRot.w;              // 9

        Debug.Log(strLong);

        // call export
        ExportCSV.exportData(originPath, strLong);
    }

    /**
      * Load map in coroutine, will wait if async task is performed
      * Example of async task
      * - yield return null
      * - yield return new WaitForSeconds(1)
      */
    IEnumerator Load()
    {
        float start_time = Time.time;

        // open panel
        Text MapStatusText = UIManager.GetComponent<UIManager_CatExample>().MapStatus;

        SetText(MapStatusText, "Loading map, please wait...");
        UIManager.GetComponent<UIManager_CatExample>().OpenPanel();

        // if only no session subsystem
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
        if (sessionSubsystem == null)
        {
            SetText(MapStatusText, "No session subsystem available. Could not load.");
            UIManager.GetComponent<UIManager_CatExample>().OpenPanel();
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
                UIManager.GetComponent<UIManager_CatExample>().OpenPanel();
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
            UIManager.GetComponent<UIManager_CatExample>().OpenPanel();
            yield break;
        }

        Log("Apply ARWorldMap to current session.");
        sessionSubsystem.ApplyWorldMap(worldMap);

        // by this the map should be loaded, but the origin hasn't changed
        // we can load the object but it will takes several seconds to adjust
        // just place the origin var with all zero
        // this because we don't need imageTarget as reference anymore
        Debug.Log("Map loaded!");

        float end_time = Time.time;
        float time_spend = System.Math.Abs(end_time - start_time);

        SetText(MapStatusText, "Map loaded for " + time_spend.ToString("0.00") + " secs.!");
        UIManager.GetComponent<UIManager_CatExample>().OpenPanel();
    }

    /**
      * Load previous saved image tracked from csv
      */
    void LoadOriginData()
    {
        // load csv
        List<string> origindata = ImportCSV.getDataPersistentPath(originPath);

        if (origindata.Count <= 0)
        {
            Debug.LogError("No marker data!");
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

    string GetPath()
    {
        int maps_number = GlobalConfig.MapsSelection;
        if (maps_number > 0)
        {
            string new_map_filename = "catExample_session_" + maps_number + ".worldmap";
            return Path.Combine(Application.persistentDataPath, new_map_filename);
        }
        else
        {
            return Path.Combine(Application.persistentDataPath, myWorldMapName);
        }
    }

    string path
    {
        get
        {
            return GetPath();
        }
    }

    string GetOriginPath()
    {
        int maps_number = GlobalConfig.MapsSelection;
        if (maps_number > 0)
        {
            string new_origin_filename = "catExample_origin_" + maps_number + ".csv";
            return Path.Combine(Application.persistentDataPath, new_origin_filename);
        }
        else
        {
            return Path.Combine(Application.persistentDataPath, myOriginPathName);
        }
    }

    string originPath
    {
        get
        {
            return GetOriginPath();
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
        startTime = Time.time;
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