using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTargetNamePrefabController : MonoBehaviour
{
    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    private void Awake()
    {
        m_TrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    private void Start()
    {
        m_ARSessionOrigin.GetComponent<ARTrackedImageManager>().enabled = true;
    }

    ARTrackedImageManager m_TrackedImageManager;

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            // Handle added event
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event

            if (updatedImage.trackingState == TrackingState.Tracking)
            {
                Debug.Log(updatedImage.referenceImage.name + ": Tracking");
            }

            if (updatedImage.trackingState == TrackingState.Limited)
            {
                Debug.Log(updatedImage.referenceImage.name + ": Limited");
            }
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
        }
    }
}
