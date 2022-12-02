using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// TODO:
/// - In this section, a function called to get correction
/// - By file input, dynamically called from MAPS_NUMBER
/// - It also call the LOAD_OBJECT_MANAGER to handle AR obj
/// - After the correction received, it send the alt obj back to LOAD_OBJECT_MANAGER
///

/// TODO:
/// - As for testing, this will generate new CSV
/// - This CSV contains new alt obj
///

public class CorrectionWithMarker
{    
    [SerializeField]
    GameObject m_DataImportCsv;

    [SerializeField]
    [Tooltip("This value can be changed depend on math unit used.")]
    float m_AdjustedValue = 1.0f;

    List<Vector3> MarkerErrorDifference(List<MarkerImportCsv.MarkerLocation> markers)
    {
        List<Vector3> mev = new();

        foreach (var m in markers)
        {
            //Vector3 ev = m.GT_Position - m.C_Position;
            Vector3 ev = m.C_Position - m.GT_Position;
            mev.Add(ev);
        }

        return mev;
    }

    List<float[]> MarkerToObjsDistance(List<MarkerImportCsv.MarkerLocation> markers,
                                       List<Test_ImportTrueObjPos.DataObj> dataObjs,
                                       int ref_asArraySize)
    {
        List<float[]> dist = new();

        foreach (var obj in dataObjs)
        {
            float[] d_array = new float[ref_asArraySize];
            int i = 0;

            foreach (var m in markers)
            {
                float d = Vector3.Distance(obj.Position, m.GT_Position);
                d_array[i] = d * m_AdjustedValue;
                i++;
            }

            dist.Add(d_array);
        }

        return dist;
    }

    List<float[]> WeightFunction(List<float[]> values, string type, int ref_asArraySize,
                                                       bool inverted = true, bool normalized = true)
    {
        List<float[]> weights = new();

        foreach (var v in values)
        {
            float[] w_array = new float[ref_asArraySize];

            for (int i = 0; i < v.Length; i++)
            {
                float result = 0;

                if (type == MathFunctions.SIGMOID)
                {
                    result = MathFunctions.Sigmoid(v[i], inverted);
                }

                if (type == MathFunctions.TANH)
                {
                    result = MathFunctions.Tanh(v[i], inverted);
                }

                w_array[i] = result;
            }

            weights.Add(w_array);
        }

        if (normalized)
        {
            // migration and clearance
            List<float[]> mig_Ws = new();
            foreach (var w in weights)
            {
                float[] temp_array = new float[ref_asArraySize];
                for (int i = 0; i < w.Length; i++)
                {
                    temp_array[i] = w[i];
                }
                mig_Ws.Add(temp_array);
            }
            weights.Clear();

            // normalization
            foreach (var mW in mig_Ws)
            {
                float[] norm_w_array = new float[ref_asArraySize];
                for (int i = 0; i < mW.Length; i++)
                {
                    float norm = MathFunctions.Normalized(mW[i], mW);
                    norm_w_array[i] = norm;
                }

                weights.Add(norm_w_array);
            }
        }

        return weights;
    }

    List<Vector3> CorrectedVector(List<Test_ImportTrueObjPos.DataObj> dataObjs,
                                  List<float[]> weightedValues,
                                  List<Vector3> markerErrorVector)
    {
        List<Vector3> altVec = new();
        int j = 0;

        foreach (var wV in weightedValues)               // this is all obj location
        {
            Vector3 mevMulw = new();                     // new Vec3 is 0,0,0
            var obj = dataObjs[j];
            
            for (int i = 0; i < wV.Length; i++)
            {
                mevMulw += markerErrorVector[i] * wV[i];    // we can get with array-like for mEV
                //Debug.Log(LoggingVec3(mevMulw));
            }

            Vector3 altObj = obj.Position + mevMulw;    // add the result to obj Pos
            altVec.Add(altObj);
            j++;
        }

        return altVec;
    }

    public List<Vector3> ProcessData(List<MarkerImportCsv.MarkerLocation> markers,
                                     List<CameraTrajectoryImportCsv.CameraTrajectory> cameras,
                                     List<Test_ImportTrueObjPos.DataObj> dataObjs,
                                     bool useCameraTrajectoryData=false)
    {
        List<MarkerImportCsv.MarkerLocation> marLoc = markers; 

        if (useCameraTrajectoryData)
        {
            // 1. input camera as obj
            List<Test_ImportTrueObjPos.DataObj> camTVecL = new();
            foreach (var camT in cameras)
            {
                Test_ImportTrueObjPos.DataObj temp = new(camT.Position);
                camTVecL.Add(temp);
            }
            int arrSize_2 = markers.Count;
            var med_2 = MarkerErrorDifference(markers);
            var mtod_2 = MarkerToObjsDistance(markers, camTVecL, arrSize_2);
            var wf_2 = WeightFunction(mtod_2, MathFunctions.SIGMOID, arrSize_2, true, true);
            var cv_2 = CorrectedVector(camTVecL, wf_2, med_2);

            // 2. use this adjusted camera as marker
            marLoc.Clear();
            for (int i = 0; i < cv_2.Count; i++)
            {
                MarkerImportCsv.MarkerLocation temp = new(
                    i.ToString(), cv_2[i], new(), cameras[i].Position, new());
                marLoc.Add(temp);
            }
        }

        // 3. use this to correct obj
        int arrSize = marLoc.Count;
        var med_3 = MarkerErrorDifference(marLoc);
        var mtod_3 = MarkerToObjsDistance(marLoc, dataObjs, arrSize);
        var wf_3 = WeightFunction(mtod_3, MathFunctions.SIGMOID, arrSize, true, true);

        return CorrectedVector(dataObjs, wf_3, med_3);
    }

    
    string LoggingFloat(float[] array)
    {
        string r = "[";

        for (int i = 0; i < array.Length; i++)
        {
            r += array[i] + ", ";
        }

        r += "]";
        return r;
    }

    string LoggingVec3(Vector3 v)
    {
        string r = "(";
        r += v.x + ", ";
        r += v.y + ", ";
        r += v.z;
        r += ")";
        return r;
    }

    void SaveToCSV(List<Vector3> vList, string title="")
    {
        List<string[]> dataS = new();
        int i = 1;

        foreach (var item in vList)
        {
            string[] data = new[]
            {
                i.ToString(),
                item.x.ToString(),
                item.y.ToString(),
                item.z.ToString()
            };

            dataS.Add(data);
            i++;
        }

        if (dataS.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.SAVE_INTO_MAP.ToString();
        string fileName = time + "__with_" + title + "__ObjectCorrection__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, dataS);

        Debug.Log(fileName);
        Debug.Log(path);
    }

    void TestDebug()
    {
        if (m_DataImportCsv == null) return;

        // import the data
        var camTra = m_DataImportCsv.GetComponent<CameraTrajectoryImportCsv>();
        camTra.ImportData();
        var camTraList = camTra.GetCameraTrajectories();

        var marLoc = m_DataImportCsv.GetComponent<MarkerImportCsv>();
        marLoc.ImportData();
        var marLocList = marLoc.GetMarkerLocations();
        var marLocListSum = marLoc.GetMarkerLocationsSummarized();

        // test debug
        foreach (var item in camTraList)
        {
            Debug.Log("pos: " + item.Position.ToString() + "\nrot: " + item.EulerAngle.ToString());
        }

        foreach (var item in marLocList)
        {
            Debug.Log("name: " + item.name +
                "gtpos: " + item.GT_Position.ToString() + "\ngtrot: " + item.GT_EulerAngle.ToString() +
                "cpos: " + item.C_Position.ToString() + "\ngtrot: " + item.GT_EulerAngle.ToString());
        }

        foreach (var item in marLocListSum)
        {
            Debug.Log("name: " + item.name +
                "gtpos: " + item.GT_Position.ToString() + "\ngtrot: " + item.GT_EulerAngle.ToString() +
                "cpos: " + item.C_Position.ToString() + "\ngtrot: " + item.GT_EulerAngle.ToString());
        }

        //////////////////////////////////////////////////////
        ///

        //List<Test_ImportTrueObjPos.DataObj> myDObjL = new();
        //Test_ImportTrueObjPos.DataObj myDObj = new();
        //myDObj.Position = new();
        //myDObjL.Add(myDObj);

        //float[] myWs = new[] { 0.1f, 0f, 0f, 0.5f, 0.4f };
        //List<float[]> myWsL = new();
        //myWsL.Add(myWs);

        //Vector3 myVa = new(-1f, 0, -1f);    // gt
        //Vector3 myVb = new(1f, 0, 1f);      // error
        //var myDif = myVa - myVb;
        //List<Vector3> myMEL = new();
        //myMEL.Add(myDif); myMEL.Add(myDif); myMEL.Add(myDif); myMEL.Add(myDif); myMEL.Add(myDif);

        //var myRes = CorrectedVector(myDObjL, myWsL, myMEL);
        //Debug.Log("value: " + LoggingVec3(myRes[0]));
    }

    void TestPreData()
    { // import the data
        var camTra = m_DataImportCsv.GetComponent<CameraTrajectoryImportCsv>();
        var camTraList = camTra.GetCameraTrajectories();

        var marLoc = m_DataImportCsv.GetComponent<MarkerImportCsv>();
        var marLocListSum = marLoc.GetMarkerLocationsSummarized();

        var objPos = m_DataImportCsv.GetComponent<Test_ImportTrueObjPos>();
        var objPosList = objPos.GetObjPoss();

        //int h = 1;
        //foreach (var item in marLocListSum)
        //{
        //    Debug.Log("name: " + h + ", value: " + item.GT_Position + ", value2: " + item.C_Position);
        //    h++;
        //}

        ///////////////////////////
        /// CORRECTION FUNCTION ///
        ///////////////////////////

        int ref_asArraySize = marLocListSum.Count;
        var med = MarkerErrorDifference(marLocListSum);

        //int i = 1;
        //foreach (var item in med)
        //{
        //    Debug.Log("name: " + i + ", value: " + LoggingVec3(item));
        //    i++;
        //}

        var mtod = MarkerToObjsDistance(marLocListSum, objPosList, ref_asArraySize);

        //int j = 1;
        //foreach (var item in mtod)
        //{
        //    Debug.Log("name: " + j + ", value: " + LoggingFloat(item));
        //    j++;
        //}

        var wf = WeightFunction(mtod, MathFunctions.SIGMOID, ref_asArraySize, true, true);

        //int k = 1;
        //foreach (var item in wf)
        //{
        //    Debug.Log("name: " + k + ", value: " + LoggingFloat(item));
        //    k++;
        //}

        var cv = CorrectedVector(objPosList, wf, med);

        //int l = 1;
        //foreach (var item in cv)
        //{
        //    Debug.Log("name: " + l + ", value: " + LoggingVec3(item));
        //    l++;
        //}

        ///////////////////////////////////////////////////
        /// CORRECTION FUNCTION USING CAMERA TRAJECTORY ///
        ///////////////////////////////////////////////////

        // I made this as like separate life, so we might have duplication

        // 1. input camera as obj
        List<Test_ImportTrueObjPos.DataObj> camTVecL = new();
        foreach (var camT in camTraList)
        {
            Test_ImportTrueObjPos.DataObj temp = new(camT.Position);
            camTVecL.Add(temp);
        }
        int arrSize_2 = marLocListSum.Count;
        var med_2 = MarkerErrorDifference(marLocListSum);
        var mtod_2 = MarkerToObjsDistance(marLocListSum, camTVecL, arrSize_2);
        var wf_2 = WeightFunction(mtod_2, MathFunctions.SIGMOID, arrSize_2, true, true);
        var cv_2 = CorrectedVector(camTVecL, wf_2, med_2);

        // 2. use this adjusted camera as marker
        List<MarkerImportCsv.MarkerLocation> newCamAsMar = new();
        for (int i = 0; i < cv_2.Count; i++)
        {
            MarkerImportCsv.MarkerLocation temp = new(
                i.ToString(), cv_2[i], new(), camTraList[i].Position, new());
            newCamAsMar.Add(temp);
        }

        // 3. use this to correct obj
        int arrSize_3 = newCamAsMar.Count;
        var med_3 = MarkerErrorDifference(newCamAsMar);
        var mtod_3 = MarkerToObjsDistance(newCamAsMar, objPosList, arrSize_3);
        var wf_3 = WeightFunction(mtod_3, MathFunctions.SIGMOID, arrSize_3, true, true);
        var cv_3 = CorrectedVector(objPosList, wf_3, med_3);

        SaveToCSV(cv_3);

        //////////////////////////////////
        /// VISUALIZATION OF AR OBJECT ///
        //////////////////////////////////

        foreach (var item in objPosList)
        {
            var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            a.transform.localScale = new(0.05f, 0.05f, 0.05f);
            a.transform.position = item.Position;

            Material mats = Resources.Load<Material>("Materials/R_transparent");
            a.GetComponent<Renderer>().material = mats;
        }


        foreach (var item in cv)
        {
            var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            a.transform.localScale = new(0.05f, 0.05f, 0.05f);
            a.transform.position = item;

            Material mats = Resources.Load<Material>("Materials/B_transparent");
            a.GetComponent<Renderer>().material = mats;
        }

        foreach (var item in cv_3)
        {
            var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            a.transform.localScale = new(0.05f, 0.05f, 0.05f);
            a.transform.position = item;

            Material mats = Resources.Load<Material>("Materials/G_transparent_2");
            a.GetComponent<Renderer>().material = mats;
        }

        ///////////////////////////////
        /// VISUALIZATION OF MARKER ///
        ///////////////////////////////

        foreach (var item in marLocListSum)
        {
            var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            a.transform.localScale = new(0.05f, 0.05f, 0.05f);
            a.transform.position = item.GT_Position;
        }


        foreach (var item in marLocListSum)
        {
            var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            a.transform.localScale = new(0.05f, 0.05f, 0.05f);
            a.transform.position = item.C_Position;
        }
    }

}
