using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerImportCsv
{
    List<MarkerLocation> markerLocations = new();

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
            string name = csvData[1];

            Vector3 gt_pos = new(float.Parse(csvData[2]),
                                   float.Parse(csvData[3]),
                                   float.Parse(csvData[4]));

            Vector3 gt_e_rot = new(float.Parse(csvData[5]),
                                        float.Parse(csvData[6]),
                                        float.Parse(csvData[7]));

            Vector3 c_pos = new(float.Parse(csvData[8]),
                                   float.Parse(csvData[9]),
                                   float.Parse(csvData[10]));

            Vector3 c_e_rot = new(float.Parse(csvData[11]),
                                        float.Parse(csvData[12]),
                                        float.Parse(csvData[13]));


            var mL = new MarkerLocation
            {
                name = name,
                GT_Position = gt_pos,
                GT_EulerAngle = gt_e_rot,
                C_Position = c_pos,
                C_EulerAngle = c_e_rot
            };

            markerLocations.Add(mL);
        }
    }

    public List<MarkerLocation> GetMarkerLocations()
    {
        return markerLocations;
    }

    public class MarkerLocation
    {
        public string name { get; set; }
        public Vector3 GT_Position { get; set; }
        public Vector3 GT_EulerAngle { get; set; }
        public Vector3 C_Position { get; set; }
        public Vector3 C_EulerAngle { get; set; }
        public MarkerLocation(string name, Vector3 GT_Position, Vector3 GT_EulerAngle,
                                           Vector3 C_Position, Vector3 C_EulerAngle)
        {
            this.name = name;
            this.GT_Position = GT_Position;
            this.GT_EulerAngle = GT_EulerAngle;
            this.C_Position = C_Position;
            this.C_EulerAngle = C_EulerAngle;
        }
        public MarkerLocation() { }
    }



    ////////////////////////////////////////////////////////////////
    /// A FUNCTIONS OF SUMMARIZING SIMILAR NAME AND AVERAGE THEM ///
    ////////////////////////////////////////////////////////////////
    
    public List<MarkerLocation> GetMarkerLocationsSummarized(string path = "")
    {
        List<MarkerLocation> newMarLoc = new();

        if (markerLocations.Count <= 0)
        {
            ImportData(path);
        }

        List<MarkerLocationExtend> tempMarLocEx = new();

        foreach (var mL in markerLocations)
        {
            //bool foundItem = false;

            //foreach (var newML in newMarLoc)
            //{
            //    if (mL.name == newML.name)
            //    {
            //        Vector3 newVec3 = (mL.C_Position + newML.C_Position) / 2;
            //        newML.C_Position = newVec3;

            //        newVec3 = (mL.C_EulerAngle + newML.C_EulerAngle) / 2;
            //        newML.C_EulerAngle = newVec3;

            //        foundItem = true;
            //    }
            //}

            //if (!foundItem)
            //{
            //    newMarLoc.Add(mL);
            //}

            bool foundItem = false;
            foreach (var newML in tempMarLocEx)
            {
                if (mL.name == newML.markerLocation.name)
                {
                    newML.markerLocation.C_Position += mL.C_Position;
                    newML.markerLocation.C_EulerAngle += mL.C_EulerAngle;
                    newML.count++;

                    foundItem = true;
                }
            }

            if (!foundItem)
            {
                MarkerLocationExtend mle = new();
                mle.markerLocation = mL;
                mle.count = 1;
                tempMarLocEx.Add(mle);
            }
        }


        foreach (var mle in tempMarLocEx)
        {
            MarkerLocation tempML = mle.markerLocation;
            tempML.C_Position /= mle.count;
            tempML.C_EulerAngle /= mle.count;

            newMarLoc.Add(tempML);
        }

        return newMarLoc;
    }

    public class MarkerLocationExtend
    {
        public MarkerLocation markerLocation { get; set; }
        public int count { get; set; }
    }
}