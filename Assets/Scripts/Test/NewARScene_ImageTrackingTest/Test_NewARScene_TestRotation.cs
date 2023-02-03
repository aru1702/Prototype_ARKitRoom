using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_NewARScene_TestRotation : MonoBehaviour
{
    [SerializeField]
    GameObject m_ImageTrackingCorrection;

    // Update is called once per frame
    void Update()
    {
        Update1();
        Update4();

        if (!m_Update1 && !m_Update4)
        {
            m_ImageTrackingCorrection
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .UpdateHasUpdate(false);

            update1_done = false;
            update4_done = false;
        }
    }

    [SerializeField]
    string m_MarkerName = "img_1";

    [SerializeField]
    Vector3 m_DesirePosition = new(-1.017f, -0.715f, -2.94f);

    [SerializeField]
    [Tooltip("Unity will calculate X, Y, Z in this order.")]
    Vector3 m_DesireRotation = new(0, 95f, 0);

    public bool m_Update1 = false;
    bool update1_done = false;
    bool update1_added = false;

    void Update1()
    {
        if (m_Update1 && !update1_done)
        {            
            CustomTransform new_ct = new();
            new_ct.custom_name = m_MarkerName;
            new_ct.custom_position = new(m_DesirePosition.x, m_DesirePosition.y, m_DesirePosition.z);
            new_ct.custom_euler_rotation = new(m_DesireRotation.x, m_DesireRotation.y, m_DesireRotation.z);
            new_ct.custom_q_rotation = Quaternion.Euler(new_ct.custom_euler_rotation);

            if (!update1_added)
            {
                m_ImageTrackingCorrection
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputData(new_ct);
            }
            else
            {
                m_ImageTrackingCorrection
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .UpdateInputData(new_ct);
            }

            m_ImageTrackingCorrection
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .UpdateHasUpdate(true);

            update1_done = true;
            update1_added = true;
        }
    }

    [SerializeField]
    string m_MarkerName4 = "img_4";

    [SerializeField]
    Vector3 m_DesirePosition4 = new(-1.015f, -0.708f, 10.49f);

    [SerializeField]
    [Tooltip("Unity will calculate X, Y, Z in this order.")]
    Vector3 m_DesireRotation4 = new(0, 87f, 0);

    public bool m_Update4 = false;
    bool update4_done = false;
    bool update4_added = false;

    void Update4()
    {
        if (m_Update4 && !update4_done)
        {
            CustomTransform new_ct = new();
            new_ct.custom_name = m_MarkerName4;
            new_ct.custom_position = new(m_DesirePosition4.x, m_DesirePosition4.y, m_DesirePosition4.z);
            new_ct.custom_euler_rotation = new(m_DesireRotation4.x, m_DesireRotation4.y, m_DesireRotation4.z);
            new_ct.custom_q_rotation = Quaternion.Euler(new_ct.custom_euler_rotation);

            if (!update4_added)
            {
                m_ImageTrackingCorrection
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputData(new_ct);
            }
            else
            {
                m_ImageTrackingCorrection
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .UpdateInputData(new_ct);
            }

            m_ImageTrackingCorrection
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .UpdateHasUpdate(true);

            update4_done = true;
            update4_added = true;
        }
    }
}
