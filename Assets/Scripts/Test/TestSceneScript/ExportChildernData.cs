using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExportChildernData : MonoBehaviour
{
    [SerializeField]
    GameObject[] m_ParentObject;

    [SerializeField]
    string m_DelimiterCSV = ",";

    [SerializeField]
    int m_CommaBehindDecimal = 3;


    List<string[]> m_ChildObjects;


    // Start is called before the first frame update
    void Start()
    {
        m_ChildObjects = new List<string[]>();
        string[] title = new[]
            {
                "name",
                "parent",
                "W_pos_x", "W_pos_y", "W_pos_z",
                "L_pos_x", "L_pos_y", "L_pos_z",
                "W_rot_x", "W_rot_y", "W_rot_z", "W_rot_w",
                "L_rot_x", "L_rot_y", "L_rot_z", "L_rot_w",
                "L_scl_x", "L_scl_y", "L_scl_z"
            };
        m_ChildObjects.Add(title);

        foreach (var item in m_ParentObject)
        {
            if (item.transform.childCount > 0)
            {
                ExtractChildren(item);
            }
        }

        string fileName = GlobalConfig.GetNowDateandTime(true) + "_ExportChildrenData.csv";
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            ExportCSV.exportData(path, m_ChildObjects);
            Debug.Log("Export complete!");
        }
        catch (System.Exception ex)
        {
            Debug.Log("Export failed! Reason: " + ex.ToString());
        }
    }

    void ExtractChildren(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            var child = parent.transform.GetChild(i).gameObject;

            string name = child.name;
            string prent = child.transform.parent.name;
            string pos_W = ExtractVector3ToString(child.transform.position);
            string pos_L = ExtractVector3ToString(child.transform.localPosition);
            string rot_W = ExtractQuaternionToString(child.transform.rotation);
            string rot_L = ExtractQuaternionToString(child.transform.localRotation);
            string scl_L = ExtractVector3ToString(child.transform.localScale);

            string[] compile = new[] { name,prent,pos_W,pos_L,rot_W,rot_L,scl_L };

            m_ChildObjects.Add(compile);
        }
    }

    /// <summary>
    /// Vector3 --> string (no delimiter at the end)
    /// </summary>
    string ExtractVector3ToString(Vector3 vector, string delimiter = ",", int commaBehind = 3)
    {
        if (m_DelimiterCSV != "") delimiter = m_DelimiterCSV;
        if (m_CommaBehindDecimal >= 0) commaBehind = m_CommaBehindDecimal;
        string cmBh = "F" + commaBehind.ToString();
        return vector.x.ToString(cmBh) + delimiter +
            vector.y.ToString(cmBh) + delimiter +
            vector.z.ToString(cmBh);
    }

    /// <summary>
    /// Quaternion --> string (no delimiter at the end)
    /// </summary>
    string ExtractQuaternionToString(Quaternion quaternion, string delimiter = ",", int commaBehind = 3)
    {
        if (m_DelimiterCSV != "") delimiter = m_DelimiterCSV;
        if (m_CommaBehindDecimal >= 0) commaBehind = m_CommaBehindDecimal;
        string cmBh = "F" + commaBehind.ToString();
        return quaternion.x.ToString(cmBh) + delimiter +
            quaternion.y.ToString(cmBh) + delimiter +
            quaternion.z.ToString(cmBh) + delimiter +
            quaternion.w.ToString(cmBh);
    }
}
