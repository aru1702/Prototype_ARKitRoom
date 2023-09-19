using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportPointClouds : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Example:  Assets/.../file_name.csv")]
    string m_PointCloudsFilePath;

    [SerializeField]
    GameObject m_PointCloudsObject;


    // Start is called before the first frame update
    void Start()
    {
        GameObject pointcloud_root = new GameObject();

        var pointcloud_data = ImportCSV.getDataOutsource(m_PointCloudsFilePath, true, ",");

        foreach (string[] pc in pointcloud_data)
        {
            Vector3 pos = new Vector3(float.Parse(pc[1]),
                                      float.Parse(pc[2]),
                                      float.Parse(pc[3]));
            GameObject pc_obj = Instantiate(m_PointCloudsObject);
            pc_obj.name = "id_" + pc[0];
            pc_obj.transform.position = pos;
            pc_obj.SetActive(true);
            pc_obj.transform.SetParent(pointcloud_root.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
