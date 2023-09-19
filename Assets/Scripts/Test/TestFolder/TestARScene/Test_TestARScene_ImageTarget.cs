using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Test_TestARScene_ImageTarget : MonoBehaviour
{
    ARTrackedImageManager m_ARTrackedImageManager;

    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    [SerializeField]
    GameObject m_ARCamera;

    [SerializeField]
    GameObject m_Prefab;

    List<Cube> m_Cubes;

    private void Awake()
    {
        m_ARTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        m_ARTrackedImageManager.trackedImagesChanged += OnImageChanged;

        m_Cubes = new();
    }

    private void OnDisable()
    {
        m_ARTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var newImage in args.added)
        {
            // Handle new event
            Debug.Log("Added: " + newImage.name);
        }

        foreach (var updatedImage in args.updated)
        {
            // Handle updated event
            Debug.Log("Updated: " + updatedImage.name + "\n" +
                        "status: " + updatedImage.trackingState.ToString() + "\n" +
                        "transform: " + updatedImage.transform.position.ToString());
        }

        foreach (var removedImage in args.removed)
        {
            // Handle removed event
            Debug.Log("Removed: " + removedImage.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ARTrackedImageManager.trackables.count > 0)
        {
            foreach (var trackedImage in m_ARTrackedImageManager.trackables)
            {
                var pos = trackedImage.transform.position;
                var rot = trackedImage.transform.rotation;
                var name = trackedImage.referenceImage.name;

                if (FindCube(name, out GameObject cube))
                {
                    cube.transform.SetPositionAndRotation(pos, rot);

                    cube.transform.GetChild(1).GetComponent<TextMesh>().text
                        = "Name: " + name +
                          "\nPosition: " + pos.ToString() +
                          "\nRotation: " + rot.ToString();
                }
                else
                {
                    GameObject go = Instantiate(m_Prefab, pos, rot);
                    go.name = name;

                    Cube c = new(go, name);
                    m_Cubes.Add(c);

                    go.transform.GetChild(1).GetComponent<TextMesh>().text
                        = "Name: " + name +
                          "\nPosition: " + pos.ToString() +
                          "\nRotation: " + rot.ToString();
                }
            }
        }
    }

    class Cube
    {
        public GameObject obj { set; get; }
        public string name { set; get; }

        public Cube(GameObject obj, string name)
        {
            this.obj = obj;
            this.name = name;
        }
    }

    bool FindCube(string cubename, out GameObject cube)
    {
        foreach (var cb in m_Cubes)
        {
            if (string.Equals(cubename, cb.name))
            {
                cube = cb.obj;
                return true;
            }
        }

        cube = null;
        return false;
    }
}
