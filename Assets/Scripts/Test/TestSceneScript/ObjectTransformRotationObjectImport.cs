using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransformRotationObjectImport : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Input format:\n\nA1,B1,C1\nA2,B2,C2\n...\nAn,Bn,Cn")]
    [TextArea(5, 15)]
    string m_ObjectPositions;

    [SerializeField]
    [Tooltip("Input format:\n\nA1,B1,C1,D1\nA2,B2,C2,D2\n...\nAn,Bn,Cn,Dn")]
    [TextArea(5, 15)]
    string m_ObjectRotations;

    [SerializeField]
    [Tooltip("Target rotation")]
    Quaternion m_TargetRotation = new(0, 0, 0, 1);

    [SerializeField]
    string m_ObjectContext;

    [SerializeField]
    GameObject m_GameObjectPrefab;

    GameObject m_RootObject;
    List<GameObject> m_Objects;
    Quaternion m_AverageRotation;

    private void Start()
    {
        // create new list
        m_Objects = new();

        // create new root object
        m_RootObject = new();
        m_RootObject.transform.SetPositionAndRotation(new(0,0,0), new(0,0,0,1));
        m_RootObject.name = m_ObjectContext + "_root";
        m_RootObject.transform.SetParent(this.transform);

        // handle object position
        string[] obj_poss = m_ObjectPositions.Split("\n");
        for (int i = 0; i < obj_poss.Length; i++)
        {
            string[] get_pos = obj_poss[i].Split(",");

            GameObject new_obj = new();
            new_obj.name = m_ObjectContext + "_" + (i + 1);
            new_obj.transform.position = new(float.Parse(get_pos[0]),
                                                float.Parse(get_pos[1]),
                                                float.Parse(get_pos[2]));
            new_obj.transform.SetParent(m_RootObject.transform);

            if (m_GameObjectPrefab != null)
            {
                GameObject temp_obj = Instantiate(m_GameObjectPrefab);
                temp_obj.transform.SetParent(new_obj.transform);
                temp_obj.transform.localPosition = new(0, 0, 0);
            }

            m_Objects.Add(new_obj);
        }

        // handle object rotation
        List<EigenMacHelper.QuaternionWeighted> qws = new();
        string[] obj_rots = m_ObjectRotations.Split("\n");
        for (int i = 0; i < obj_rots.Length; i++)
        {
            string[] get_rot = obj_rots[i].Split(",");
            EigenMacHelper.QuaternionWeighted qw = new(
                new(float.Parse(get_rot[0]),
                    float.Parse(get_rot[1]),
                    float.Parse(get_rot[2]),
                    float.Parse(get_rot[3]))
                , 1);
            qws.Add(qw);
        }
        m_AverageRotation = EigenMacHelper.EigenWeightedAvgMultiRotations(qws.ToArray());

        // handle rotating root
        Quaternion rot_value = m_TargetRotation * Quaternion.Inverse(m_AverageRotation);
        m_RootObject.transform.rotation *= rot_value;

        // handle showing result
        string s = m_ObjectContext + "\n";
        s += "x,y,z,x,y,z,w\n";
        foreach (var item in m_Objects)
        {
            var pos = item.transform.position;
            var rot = item.transform.rotation;
            string c = ",";

            s += pos.x + c + pos.y + c + pos.z + c + rot.x + c + rot.y + c + rot.z + c + rot.w + "\n";
        }
        Debug.Log(s);
    }

    public GameObject GetRoot() { return m_RootObject; }
    public List<GameObject> GetObjects() { return m_Objects; }
    public Quaternion GetAverageRotation() { return m_AverageRotation; }
}
