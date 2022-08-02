using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

public class BackButton_DisposeWorldmap : MonoBehaviour
{
    [SerializeField]
    ARSession m_ARSession;

#if UNITY_IOS
    public void DisposeMap()
    {
        StartCoroutine(Disposing_Coroutine());
    }
#endif

#if UNITY_IOS
    IEnumerator Disposing_Coroutine()
    {
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
        if (sessionSubsystem == null)
        {
            Debug.LogError("No session subsystem");
            yield break;
        }

        var request = sessionSubsystem.GetARWorldMapAsync();
        while (!request.status.IsDone())
            yield return null;

        if (request.status.IsError())
        {
            Debug.LogError("Session serialization failed with status: " + request.status);
            yield break;
        }

        var worldMap = request.GetWorldMap();
        request.Dispose();
        worldMap.Dispose();
    }
#endif
}
