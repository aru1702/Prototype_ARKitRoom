using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrajectoryImportCsv
{
    List<CameraTrajectory> cameraTrajectories = new();

    [SerializeField]
    string m_predefinedPath;

    [SerializeField]
    bool m_testMode = false;

    public void ImportData(string path = "")
    {
        List<string[]> data;

        if (m_testMode)
        {
            if (string.IsNullOrEmpty(m_predefinedPath))
            {
                Debug.LogError("No defined path!");
                return;
            }

            data = ImportCSV.getData(m_predefinedPath, true);
        }
        else
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("No defined path!");
                return;
            }

            data = ImportCSV.getDataOutsource(path, true);
        }              

        // put into class
        foreach (var csvData in data)
        {
            Vector3 position = new(float.Parse(csvData[1]),
                                   float.Parse(csvData[2]),
                                   float.Parse(csvData[3]));

            Vector3 eulerRotation = new(float.Parse(csvData[4]),
                                        float.Parse(csvData[5]),
                                        float.Parse(csvData[6]));

            // add this when data has quaternion
            //Quaternion rotation = new(float.Parse(csvData[7]),
            //                            float.Parse(csvData[8]),
            //                            float.Parse(csvData[9]),
            //                            float.Parse(csvData[10]));

            var cT = new CameraTrajectory
            {
                Position = position,
                EulerAngle = eulerRotation
            };
            //cT.Rotation = rotation;

            cameraTrajectories.Add(cT);
        }
    }

    public List<CameraTrajectory> GetCameraTrajectories(string path = "")
    {
        if (cameraTrajectories.Count <= 0) ImportData(path);

        return cameraTrajectories;
    }

    public class CameraTrajectory
    {
        public Vector3 Position { get; set; }
        public Vector3 EulerAngle { get; set; }
        public Quaternion Rotation { get; set; }
        public CameraTrajectory(Vector3 Position, Vector3 EulerAngle, Quaternion Rotation)
        {
            this.Position = Position;
            this.EulerAngle = EulerAngle;
            this.Rotation = Rotation;
        }
        public CameraTrajectory() { }
    }
}
