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

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Demonstrates the saving and loading of an
    /// <a href="https://developer.apple.com/documentation/arkit/arworldmap">ARWorldMap</a>
    /// </summary>
    /// <remarks>
    /// ARWorldMaps are only supported by ARKit, so this API is in the
    /// <c>UntyEngine.XR.ARKit</c> namespace.
    /// </remarks>
    public class WorldMap_CatExample : MonoBehaviour
    {
        // my modification
        const string myWorldMapName = "catExample_session.worldmap";
        const string myOriginPathName = "catExample_origin.csv";

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

        //[Tooltip("UI Text component to display error messages")]
        //[SerializeField]
        //Text m_ErrorText;

        ///// <summary>
        ///// The UI Text component used to display error messages
        ///// </summary>
        //public Text errorText
        //{
        //    get { return m_ErrorText; }
        //    set { m_ErrorText = value; }
        //}

        //[Tooltip("The UI Text element used to display log messages.")]
        //[SerializeField]
        //Text m_LogText;

        ///// <summary>
        ///// The UI Text element used to display log messages.
        ///// </summary>
        //public Text logText
        //{
        //    get { return m_LogText; }
        //    set { m_LogText = value; }
        //}

        //[Tooltip("The UI Text element used to display the current AR world mapping status.")]
        //[SerializeField]
        //Text m_MappingStatusText;

        ///// <summary>
        ///// The UI Text element used to display the current AR world mapping status.
        ///// </summary>
        //public Text mappingStatusText
        //{
        //    get { return m_MappingStatusText; }
        //    set { m_MappingStatusText = value; }
        //}

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

            // deactive image target and load button
            SetActive(imageTargetButton, false);
            SetActive(loadButton, false);
#endif
        }

        /// <summary>
        /// Reset the <c>ARSession</c>, destroying any existing trackables,
        /// such as planes. Upon loading a saved <c>ARWorldMap</c>, saved
        /// trackables will be restored.
        /// </summary>
        public void OnResetButton()
        {
            m_ARSession.Reset();
        }

        // my modification
        //[SerializeField]
        //Button m_ExitButton;

        //public Button exitButton
        //{
        //    get { return m_ExitButton; }
        //    set { m_ExitButton = value; }
        //}

        //public void OnExitButton()
        //{
        //    if (GlobalConfig.AlreadyRender) { StartCoroutine(Save()); }
        //}

        [SerializeField]
        Button m_UseImageTargetButton;

        public Button imageTargetButton
        {
            get { return m_UseImageTargetButton; }
            set { m_UseImageTargetButton = value; }
        }

        public void OnImageTargetButton()
        {
            // deactive all buttons
            SetActive(imageTargetButton, false);
            SetActive(saveButton, false);
            SetActive(loadButton, false);

            // load into Image Target Manager
            ImageRecognitionManager
                .GetComponent<ImageRecognition_CatExample>()
                .enabled = true;
        }

        IEnumerator ResetMap()
        {
            yield return new WaitForSeconds(0.5f);

            GlobalConfig.AlreadyRender = false;

            GlobalConfig.MyObjectList.Clear();
            GlobalConfig.PlaySpaceMyOrigin = null;
            GlobalConfig.PlaySpaceOriginGO = null;

            ImageRecognitionManager
                .GetComponent<ImageRecognition_CatExample>()
                .enabled = false;

            LoadObjectManager
                .GetComponent<LoadObject_CatExample>()
                .enabled = false;

            m_ARSession.Reset();

            yield return new WaitForSeconds(1);

            ImageRecognitionManager
                .GetComponent<ImageRecognition_CatExample>()
                .enabled = true;

            yield break;
        }

        [SerializeField]
        GameObject ImageRecognitionManager;

        [SerializeField]
        GameObject LoadObjectManager;

        [SerializeField]
        bool enableARCameraLogging = false;

        [SerializeField]
        GameObject ARCamera;

        [Tooltip("The UI Text element used to display the current AR world mapping status.")]
        [SerializeField]
        Text m_ARCameraLocationText;

        /// <summary>
        /// The UI Text element used to display the current AR world mapping status.
        /// </summary>
        public Text arCameraLocationText
        {
            get { return m_ARCameraLocationText; }
            set { m_ARCameraLocationText = value; }
        }

        /**
         * Performed if loading map is failed
         * Automatically active image recognition manager
         * On that script, it automatically deactive this script
         */
        void CannotLoadMap()
        {
            Log("Cannot load map, enter image recognition mode");

            // load image target Manager (same implementation
            OnImageTargetButton();
        }

#if UNITY_IOS
        /**
         * Save map in coroutine, will wait if async task is performed
         * Example of async task
         * - yield return null
         * - yield return new WaitForSeconds(1)
         */
        IEnumerator Save()
        {
            var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
            if (sessionSubsystem == null)
            {
                Log("No session subsystem available. Could not save.");
                yield break;
            }

            var request = sessionSubsystem.GetARWorldMapAsync();

            while (!request.status.IsDone())
                yield return null;

            if (request.status.IsError())
            {
                Log(string.Format("Session serialization failed with status {0}", request.status));
                yield break;
            }

            var worldMap = request.GetWorldMap();
            request.Dispose();

            SaveAndDisposeWorldMap(worldMap);

            // At this moment, the map should be saved into the defined path
            // It saves the AR Session Origin, which means our AR origin (0,0,0)
            Debug.Log("Map saved!");
        }

        /**
         * Load map in coroutine, will wait if async task is performed
         * Example of async task
         * - yield return null
         * - yield return new WaitForSeconds(1)
         */
        IEnumerator Load()
        {
            // if only no session subsystem
            var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
            if (sessionSubsystem == null)
            {
                Log("No session subsystem available. Could not load.");
                CannotLoadMap();
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
                    Log(string.Format("File {0} does not exist.", path));
                    CannotLoadMap();
                    yield break;
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.LogError(ex);
                CannotLoadMap();
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
                Debug.LogError("Data is not a valid ARWorldMap.");
                CannotLoadMap();
                yield break;
            }

            Log("Apply ARWorldMap to current session.");
            sessionSubsystem.ApplyWorldMap(worldMap);

            // by this the map should be loaded, but the origin hasn't changed
            // we can load the object but it will takes several seconds to adjust
            // just place the origin var with all zero
            // this because we don't need imageTarget as reference anymore
            Debug.Log("Map loaded!");

            LoadObjectManager
                    .GetComponent<LoadObject_CatExample>()
                    .enabled = true;

            GlobalConfig.AlreadyRender = true;
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
         * Load previous saved image tracked from csv
         */
        void LoadOriginData()
        {
            // load csv
            List<string> origindata = ImportCSV.getDataPersistentPath(originPath);

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

            Debug.Log("after load:");
            Debug.Log(pos.ToString());
            Debug.Log(eRot.ToString());
            Debug.Log(rot.ToString());

            GameObject origin = new GameObject("tempOrigin");
            GlobalConfig.TempOriginGO = origin;
            origin.transform.SetPositionAndRotation(pos, rot);
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

        bool supported
        {
            get
            {
#if UNITY_IOS
                return m_ARSession.subsystem is ARKitSessionSubsystem && ARKitSessionSubsystem.worldMapSupported;
#else
                return false;
#endif
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

            // let's deactivate this first
            //if (!GlobalConfig.AlreadyRender)
            //{
            //    StartCoroutine(Load());
            //}

            // first time run program, only image target and load button is OK
            if (!GlobalConfig.AlreadyRender)
            {
                SetActive(saveButton, false);
            }

            // now image target is no no, and save and load button is OK
            else
            {
                SetActive(imageTargetButton, false);
                SetActive(saveButton, true);
                SetActive(loadButton, true);
            }

            // ar loc debug
            if (!enableARCameraLogging) SetActive(arCameraLocationText, false);
        }

        void Update()
        {
            //if (supported)
            //{
            //    SetActive(errorText, false);
            //    SetActive(saveButton, true);
            //    SetActive(loadButton, true);
            //    SetActive(mappingStatusText, true);
            //}
            //else
            //{
            //    SetActive(errorText, true);
            //    SetActive(saveButton, false);
            //    SetActive(loadButton, false);
            //    SetActive(mappingStatusText, false);
            //}

            // Actually here I want to make something like checking
            // This will check if the origin has moved
            // From PREVIOUS_OPEN_CAMERA to WORLDMAP_DATA
            // For example:
            // - worldmap said our origin is on the edge of room
            // - but at first time open app, our origin is on the center of room
            // - now how to tell the system that our origin HAS MOVED
            //
            // This will very useful for:
            // - hold rendering before enter THE CORRECT origin
            // - render only if the origin has MOVED to correct position
            //
            // Good luck!

#if UNITY_IOS
            var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
#else
            XRSessionSubsystem sessionSubsystem = null;
#endif
            if (sessionSubsystem == null)
                return;

            var numLogsToShow = 20;
            string msg = "";
            for (int i = Mathf.Max(0, m_LogMessages.Count - numLogsToShow); i < m_LogMessages.Count; ++i)
            {
                msg += m_LogMessages[i];
                msg += "\n";
            }

            // debugging AR Camera Location
            if (enableARCameraLogging)
            {
                string camPos = ARCamera.transform.position.ToString();
                SetText(arCameraLocationText, string.Format("AR Camera:\n{0}", camPos));
            }

            //SetText(logText, msg);

#if UNITY_IOS
            //SetText(mappingStatusText, string.Format("Mapping Status: {0}", sessionSubsystem.worldMappingStatus));
#endif
        }

        List<string> m_LogMessages;
    }
}