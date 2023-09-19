using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Being attached in RecordPosition game object
/// of MappingConfiguration scene
/// </summary>
public class Test_InstantiateCameraTrack : MonoBehaviour
{
    [SerializeField]
    GameObject m_RecordPositionForCameraTracks;

    [SerializeField]
    GameObject m_DesiredInstantiatePrefab;

    [SerializeField]
    Text m_ShowHideCameraTracksText;

    [SerializeField]
    float m_PrefabSizeInMeter = 0.05f;

    List<GameObject> cameraTracks = new();
    int cameraTracks_Count;
    bool showTrack;

    // Update is called once per frame
    void Update()
    {
        // update list
        List<GameObject> tempcameraTracks = m_RecordPositionForCameraTracks
            .GetComponent<RecordPosition_CameraEveryFrame>()
            .GetCameraTracks();

        //Debug.Log(cameraTracks.Count);

        if (tempcameraTracks.Count <= 0) return;

        //Debug.Log("camera track count not 0");

        // if no new created game object
        if (tempcameraTracks.Count <= cameraTracks_Count) return;

        //Debug.Log("there is new camera track go");

        cameraTracks_Count = tempcameraTracks.Count;

        //Debug.Log("reach remove and create");
        // remove and recreate
        RemoveTracks();
        foreach (var track in tempcameraTracks)
        {
            GameObject newGo = CreateTrack(track);
            ShowUnshowTracks(showTrack, newGo);
        }
    }

    GameObject CreateTrack(GameObject go)
    {
        GameObject newGo;
        if (!m_DesiredInstantiatePrefab)
        {
            newGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
        else
        {
            newGo = Instantiate(m_DesiredInstantiatePrefab);
        }

        newGo.transform.parent = go.transform;
        newGo.transform.localPosition = Vector3.zero;
        float a = m_PrefabSizeInMeter;
        newGo.transform.localScale = new Vector3(a, a, a);
        cameraTracks.Add(newGo);

        return newGo;
    }

    void RemoveTracks()
    {
        foreach (var track in cameraTracks)
        {
            Destroy(track);
        }
        cameraTracks.Clear();
    }

    void ShowUnshowTracks(bool value, GameObject go)
    {
        go.SetActive(value);
    }

    public void ShowHideCameraTrack()
    {
        if (showTrack)
        {
            showTrack = false;
            m_ShowHideCameraTracksText.text = "Show camera tracks";
        }
        else
        {
            showTrack = true;
            m_ShowHideCameraTracksText.text = "Hide camera tracks";
        }
    }
}
