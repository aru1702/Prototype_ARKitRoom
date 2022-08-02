using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Because changing the SLAM origin doesn't work to re-positioning
/// camera track into better adjustment, so we utilize the marker
/// to create another GO as point of reference.
///
/// The purpose is not to re-adjust the whole SLAM and World position,
/// instead only to get better visualization of camera track.
///
/// There is no system purpose in this function.
/// </summary>
public class Test_CameraTracksToMarker_AsSLAMPosition : MonoBehaviour
{
    [SerializeField]
    GameObject m_RecordPosition, m_MappingScannerForPointCloud;

    [SerializeField]
    bool m_SaveNewCamPos, m_SaveNewPointCloudPos;

    public void MainNewSave()
    {
        if (m_SaveNewCamPos) NewSave_RecordPosition();
        if (m_SaveNewPointCloudPos) NewSave_PointCloudPosition();
    }

    void NewSave_RecordPosition()
    {
        // check if RecordPosition gameObject has been attached
        if (!m_RecordPosition)
        {
            Debug.LogError("No RecordPosition gameObject.");
            return;
        }

        // check if marker location is exist
        if (!GlobalConfig.TempOriginGO)
        {
            Debug.LogError("No marker detected.");
            return;
        }

        // get marker location from its gameObject
        GameObject pointOfReference = GlobalConfig.TempOriginGO;

        // get the camera track list
        List<GameObject> cameraTracks = m_RecordPosition
            .GetComponent<RecordPosition_CameraEveryFrame>()
            .GetCameraTracks();

        // heterogenous matrix of SLAM origin to marker
        Matrix4x4 slamToMarker = pointOfReference.transform.worldToLocalMatrix;

        // calculate matrix from each camera tracks to marker
        // then add to list for export into csv
        List<string[]> cameraTracksByMarker_Pos = new();
        cameraTracksByMarker_Pos
            .Add(new[] {
                "timestamp",
                "pos x", "pos y", "pos z",
                "rot quat x", "rot quat y", "rot quat z", "rot quat w"
            });
        foreach (var track in cameraTracks)
        {
            // heterogenous matrix by SLAM origin
            Matrix4x4 trackToSLAM = track.transform.localToWorldMatrix;

            // calculate new matrix (marker <- SLAM <- track)
            Matrix4x4 trackToMarker = slamToMarker * trackToSLAM;

            // get pos and rot
            Vector3 pos = trackToMarker.GetPosition();
            Quaternion rot = Quaternion.LookRotation(
                trackToMarker.GetColumn(2),
                trackToMarker.GetColumn(1));

            // put into string[]
            string[] data = new[]
            {
                track.name,
                pos.x.ToString(),
                pos.y.ToString(),
                pos.z.ToString(),
                rot.x.ToString(),
                rot.y.ToString(),
                rot.z.ToString(),
                rot.w.ToString()
            };
            cameraTracksByMarker_Pos.Add(data);
        }

        // import to csv and save
        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.MapsSelection.ToString();
        string fileName = time + "_cameraTracksByMarker_Pos__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, cameraTracksByMarker_Pos);
    }

    void NewSave_PointCloudPosition()
    {
        // check if MappingScannerForPointCloud gameObject has been attached
        if (!m_MappingScannerForPointCloud)
        {
            Debug.LogError("No MappingScannerForPointCloud gameObject.");
            return;
        }

        // check if marker location is exist
        if (!GlobalConfig.TempOriginGO)
        {
            Debug.LogError("No marker detected.");
            return;
        }

        // get marker location from its gameObject
        GameObject pointOfReference = GlobalConfig.TempOriginGO;

        // import data from MappingScanner
        List<Vector3> pointClouds = m_MappingScannerForPointCloud
            .GetComponent<MappingScanner>()
            .GetPointCloudsVector3s();
        List<ulong> pointCloudUlongs = m_MappingScannerForPointCloud
            .GetComponent<MappingScanner>()
            .GetPointCloudsUlongs();

        // heterogenous matrix of SLAM origin to marker
        Matrix4x4 slamToMarker = pointOfReference.transform.worldToLocalMatrix;

        // calculate matrix from each camera tracks to marker
        // then add to list for export into csv
        List<string[]> pointCloudsByMarker_Pos = new();
        pointCloudsByMarker_Pos
            .Add(new[] {
                "identifier", "pos x", "pos y", "pos z", "status"
            });

        // create new dummy GO
        GameObject go = new GameObject();

        for (int i = 0; i < pointClouds.Count; i++)
        {
            go.transform.position = pointClouds[i];

            // heterogenous matrix by SLAM origin
            Matrix4x4 trackToSLAM = go.transform.localToWorldMatrix;

            // calculate new matrix (marker <- SLAM <- track)
            Matrix4x4 trackToMarker = slamToMarker * trackToSLAM;

            // get pos and rot
            Vector3 pos = trackToMarker.GetPosition();

            // put into string[]
            string[] data = new[]
            {
                pointCloudUlongs[i].ToString(),
                pos.x.ToString(),
                pos.y.ToString(),
                pos.z.ToString(),
                "success"
            };
            pointCloudsByMarker_Pos.Add(data);
        }

        // destroy dummy GO
        Destroy(go);

        // import to csv and save
        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.MapsSelection.ToString();
        string fileName = time + "_pointCloudsByMarker_Pos__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, pointCloudsByMarker_Pos);
    }
}
