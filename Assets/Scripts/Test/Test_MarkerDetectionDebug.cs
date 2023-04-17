using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Test_MarkerDetectionDebug : MonoBehaviour
{
    /// <summary>
    /// AR Session Origin to retrieve AR Foundation data
    /// </summary>
    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    ARTrackedImageManager ArTrackedImageManager;

    private void Awake() { ArTrackedImageManager = FindObjectOfType<ARTrackedImageManager>(); }

    private void OnEnable() { ArTrackedImageManager.trackedImagesChanged += OnImageChanged; }

    private void OnDisable() { ArTrackedImageManager.trackedImagesChanged -= OnImageChanged; }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var newImage in args.added)
        {
            // Handle new event
        }

        // this method always updated per image recognition system
        foreach (var updatedImage in args.updated)
        {
            Debug.Log(updatedImage.referenceImage.name +
                    ": " + updatedImage.trackingState.ToString() +
                    ", at " + updatedImage.transform.position.ToString());
        }

        foreach (var removedImage in args.removed)
        {
            // Handle removed event
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
