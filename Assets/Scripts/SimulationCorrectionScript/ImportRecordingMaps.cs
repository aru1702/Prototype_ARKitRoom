using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImportRecordingMaps : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Enter the number of map.")]
    string m_Map;

    const string FILENAME = "MarkerCalibration_New__Maps_";
    const string FILETYPE = ".csv";

    // Start is called before the first frame update
    void Start()
    {
        string filename = FILENAME + m_Map + FILETYPE;
        string path = Path.Combine(Application.persistentDataPath, filename);

        List<string[]> data = ImportCSV.getDataOutsource(path, true);

        //name
        //gt_pos_x	gt_pos_y	gt_pos_z	
        //gt_rot_x	gt_rot_y	gt_rot_z	gt_rot_w	
        //rt_pos_x	rt_pos_y	rt_pos_z	
        //rt_rot_x	rt_rot_y	rt_rot_z	rt_rot_w	
        //diff_pos_x	diff_pos_y	diff_pos_z	
        //diff_rot_x	diff_rot_y	diff_rot_z	diff_rot_w

        foreach (var d in data)
        {
            string name = d[0];
            Vector3 gt_pos = new(float.Parse(d[1]), float.Parse(d[2]), float.Parse(d[3]));
            Quaternion gt_rot = new(float.Parse(d[4]), float.Parse(d[5]), float.Parse(d[6]), float.Parse(d[7]));
            Vector3 rt_pos = new(float.Parse(d[8]), float.Parse(d[9]), float.Parse(d[10]));
            Quaternion rt_rot = new(float.Parse(d[11]), float.Parse(d[12]), float.Parse(d[13]), float.Parse(d[14]));
            Vector3 diff_pos = new(float.Parse(d[15]), float.Parse(d[16]), float.Parse(d[17]));
            Quaternion diff_rot = new(float.Parse(d[18]), float.Parse(d[19]), float.Parse(d[20]), float.Parse(d[21]));

            Debug.Log(name + "\n" +
                      gt_pos.ToString() + "\n" +
                      gt_rot.ToString() + "\n" +
                      rt_pos.ToString() + "\n" +
                      rt_rot.ToString() + "\n" +
                      diff_pos.ToString() + "\n" +
                      diff_rot.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
