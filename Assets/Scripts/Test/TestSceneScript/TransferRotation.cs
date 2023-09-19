using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferRotation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Input format:\n\nA1,B1,C1,D1\n...\nAn,Bn,Cn,Dn")]
    [TextArea(5, 15)]
    string m_GroundTruthQuaternions;

    [SerializeField]
    [Tooltip("Input format:\n\nA1,B1,C1,D1\n...\nAn,Bn,Cn,Dn")]
    [TextArea(5, 15)]
    string m_RuntimeQuaternions;

    // Start is called before the first frame update
    void Start()
    {
        //List<Quaternion> gts = new(); List<Quaternion> rts = new();

        //try
        //{
        //    string[] gts_nw = m_GroundTruthQuaternions.Split("\n");
        //    foreach (string nw_item in gts_nw)
        //    {
        //        string[] cm_item = nw_item.Split(",");
        //        gts.Add(new(float.Parse(cm_item[0]), float.Parse(cm_item[1]), float.Parse(cm_item[2]), float.Parse(cm_item[3])));
        //    }
        //} catch (System.Exception)
        //{
        //    Debug.Log("Wrong format of Ground Truth input.");
        //}

        //try
        //{
        //    string[] rts_nw = m_RuntimeQuaternions.Split("\n");
        //    foreach (string nw_item in rts_nw)
        //    {
        //        string[] cm_item = nw_item.Split(",");
        //        rts.Add(new(float.Parse(cm_item[0]), float.Parse(cm_item[1]), float.Parse(cm_item[2]), float.Parse(cm_item[3])));
        //    }
        //}
        //catch (System.Exception)
        //{
        //    Debug.Log("Wrong format of Runtime input.");
        //}

        //List<Quaternion> rts_to_gts_list = new();
        List<EigenMacHelper.QuaternionWeighted> qws = new(); List<EigenMacHelper.QuaternionWeighted> qws_2 = new();
        try
        {
            string[] gts_nw = m_GroundTruthQuaternions.Split("\n");
            string[] rts_nw = m_RuntimeQuaternions.Split("\n");

            if (gts_nw.Length != rts_nw.Length) { Debug.Log("Ground truth and Runtime data size is not match!"); return; }
            int length = gts_nw.Length;

            for (int i = 0; i < length; i++)
            {
                string[] gts_cm = gts_nw[i].Split(",");
                string[] rts_cm = rts_nw[i].Split(",");

                Quaternion gt = new(float.Parse(gts_cm[0]), float.Parse(gts_cm[1]), float.Parse(gts_cm[2]), float.Parse(gts_cm[3]));
                Quaternion rt = new(float.Parse(rts_cm[0]), float.Parse(rts_cm[1]), float.Parse(rts_cm[2]), float.Parse(rts_cm[3]));

                m_GTVisualization.transform.rotation = gt;
                m_RTVisualization.transform.rotation = rt;

                Quaternion rt_to_gt = gt * Quaternion.Inverse(rt);
                //rts_to_gts_list.Add(rt_to_gt);
                qws.Add(new(rt_to_gt, 1));

                Quaternion rt_to_gt_2 = rt * Quaternion.Inverse(gt);
                qws_2.Add(new(rt_to_gt_2, 1));
            }
        }
        catch (System.Exception)
        {
            Debug.Log("Wrong format of input, could be any of them.");
        }

        Quaternion avg = EigenMacHelper.EigenWeightedAvgMultiRotations(qws.ToArray());
        Debug.Log("Q: " + avg.ToString() + ", EA: " + avg.eulerAngles.ToString());

        Quaternion avg_2 = EigenMacHelper.EigenWeightedAvgMultiRotations(qws_2.ToArray());
        Debug.Log("Q: " + avg_2.ToString() + ", EA: " + avg_2.eulerAngles.ToString());

        m_FromRTtoGTVisualization.transform.rotation = m_RTVisualization.transform.rotation * avg;
        m_FromGTtoRTVisualization.transform.rotation = m_GTVisualization.transform.rotation * avg_2;
    }

    [SerializeField]
    GameObject m_GTVisualization;

    [SerializeField]
    GameObject m_RTVisualization;

    [SerializeField]
    GameObject m_FromRTtoGTVisualization;

    [SerializeField]
    GameObject m_FromGTtoRTVisualization;
}
