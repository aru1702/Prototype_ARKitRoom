using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CorrectionFunctions
{
    public class VersionTwoPreload : MonoBehaviour
    {
        // because only being called once after map loaded
        // we don't need periodic function
        // all goes to OnEnable function

        [SerializeField]
        [Tooltip("To import object location.")]
        GameObject m_LoadObjectManager;

        [SerializeField]
        [Tooltip("Scalar for weight")]
        float m_Scalar = 1.0f;

        [SerializeField]
        [Tooltip("Threshold for removing unnecessary weights")]
        float m_Threshold = 0.85f;

        void OnEnable()
        {
            // get scalar
            float scalar = GlobalConfig.OTM_SCALAR != 0 ?
                           GlobalConfig.OTM_SCALAR :
                           m_Scalar;

            // import all object position
            List<CustomTransform> data_all_objects = new List<CustomTransform>();
            var object_myobjects = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyObjects();
            foreach (var o in object_myobjects)
            {
                CustomTransform ct = new CustomTransform(
                    o.name,
                    GlobalConfig.ExtractVector3(o.transform.position),
                    GlobalConfig.ExtractVector3(o.transform.rotation.eulerAngles),
                    GlobalConfig.ExtractQuaternion(o.transform.rotation));
                data_all_objects.Add(ct);
            }

            // import marker data from local file
            string filename = "MarkerCalibration_New__Maps_" +
                              GlobalConfig.LOAD_MAP +
                              ".csv";
            string path = System.IO.Path.Combine(Application.persistentDataPath, filename);
            bool cannot_load = false;
            List<string[]> marker_calibrations_new = new List<string[]>();
            try
            {
                marker_calibrations_new = ImportCSV.getDataOutsource(path, true);
            } catch (System.Exception e)
            {
                Debug.Log("No such " + filename + " exist. See more: " + e);
                cannot_load = true;
            }
            if (cannot_load) return;

            // unload marker data
            List<MarkerLocation> data_marker = new List<MarkerLocation>();
            foreach (var d in marker_calibrations_new)
            {
                string name = d[0];
                Vector3 gt_pos = new(float.Parse(d[1]), float.Parse(d[2]), float.Parse(d[3]));
                Quaternion gt_rot = new(float.Parse(d[4]), float.Parse(d[5]), float.Parse(d[6]), float.Parse(d[7]));
                Vector3 rt_pos = new(float.Parse(d[8]), float.Parse(d[9]), float.Parse(d[10]));
                Quaternion rt_rot = new(float.Parse(d[11]), float.Parse(d[12]), float.Parse(d[13]), float.Parse(d[14]));
                Vector3 diff_pos = new(float.Parse(d[15]), float.Parse(d[16]), float.Parse(d[17]));
                Quaternion diff_rot = new(float.Parse(d[18]), float.Parse(d[19]), float.Parse(d[20]), float.Parse(d[21]));

                MarkerLocation ml = new MarkerLocation();
                ml.Marker_name = name;
                ml.GT_Position = gt_pos;
                ml.GT_Rotation = gt_rot;
                ml.C_Position = rt_pos;
                ml.C_Rotation = rt_rot;
                ml.Vector3Diff = diff_pos;
                ml.QuaternionDiff = diff_rot;
                data_marker.Add(ml);

                // Test debug to see marker data imported from csv
                // Comment below line if not necessary

                Debug.Log("name: " + name + "\n" +
                          "gt_pos: " + gt_pos.ToString() + "\t" + "gt_rot_eul: " + gt_rot.eulerAngles.ToString() + "\n" +
                          "rt_pos: " + rt_pos.ToString() + "\t" + "rt_rot_eul: " + rt_rot.eulerAngles.ToString());

                // End of debug line
            }

            // calculate for each object
            for (int j = 0; j < object_myobjects.Count; j++)
            {
                // Necessary for debug and get only certain object
                string[] nms = object_myobjects[j].name.Split("_");
                string tnm = nms[0] + "_" + nms[1];
                // End of line

                var object_by_world = GlobalConfig.GetM44ByGameObjRef(object_myobjects[j], GlobalConfig.PlaySpaceOriginGO);
                Vector3 obj_pos_w = object_by_world.GetPosition();

                // Test debug

                if (tnm == "dummy_obj")
                    Debug.Log("[" + object_myobjects[j].name + "]\n" +
                              "obj_pos: " + obj_pos_w.ToString());

                // End of debug line

                // get weight
                List<float> weights = new List<float>();
                foreach (var m in data_marker)
                {
                    float distance = Vector3.Distance(obj_pos_w, m.GT_Position);
                    float weight = Mathf.Exp(-distance * scalar);
                    weights.Add(weight);
                }
                weights = MathFunctions.NormalizedMany(weights);

                // Test debug to see weight after exponential function and normalized
                // Comment below line if not necessary

                if (tnm == "dummy_obj")
                    GlobalDebugging.DebugLogListFloat(weights, "[" + object_myobjects[j].name + "] weights before");

                // End of debug line

                float max = Mathf.Max(weights.ToArray());
                for (int i = 0; i < weights.Count; i++)
                {
                    float cur_w = Mathf.Exp(-(max - weights[i]));
                    if (cur_w < m_Threshold) weights[i] = 0;
                }
                weights = MathFunctions.NormalizedMany(weights);

                // Test debug to see weight after exponential function and normalized
                // Comment below line if not necessary

                if (tnm == "dummy_obj")
                    GlobalDebugging.DebugLogListFloat(weights, "[" + object_myobjects[j].name + "] weights after");

                // End of debug line

                // apply weight
                Vector3 new_pos = Vector3.zero;
                List<EigenMacHelper.QuaternionWeighted> qws = new();
                for (int i = 0; i < weights.Count; i++)
                {
                    new_pos += weights[i] * data_marker[i].Vector3Diff;
                    EigenMacHelper.QuaternionWeighted qw = new(
                        data_marker[i].QuaternionDiff,
                        weights[i]);
                    qws.Add(qw);
                }
                Quaternion new_rot = EigenMacHelper.EigenWeightedAvgMultiRotations(qws.ToArray());
                object_myobjects[j].transform.SetPositionAndRotation(
                    data_all_objects[j].custom_position + new_pos,
                    data_all_objects[j].custom_q_rotation * new_rot);
            }
        }
    }
}
