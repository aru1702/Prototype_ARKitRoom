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
        Update0();
        Update1();
        Update2();
        Update3();
        Update4();

        if (!m_Update0 && !m_Update1 && !m_Update2 && !m_Update3 && !m_Update4)
        {
            m_ImageTrackingCorrection
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .UpdateHasUpdate(false);

            update0_done = false;
            update1_done = false;
            update2_done = false;
            update3_done = false;
            update4_done = false;
        }
    }

    [SerializeField]
    string m_MarkerName0 = "img_0";

    [SerializeField]
    Vector3 m_DesirePosition0 = new(0.004787229f, 0.000308275194f, -0.00356803089f);

    [SerializeField]
    [Tooltip("Unity will calculate X, Y, Z in this order.")]
    Vector3 m_DesireRotation0 = new(359.783966f, 356.538879f, 0.388705671f);

    public bool m_Update0 = false;
    bool update0_done = false;
    bool update0_added = false;

    void Update0()
    {
        if (m_Update0 && !update0_done)
        {
            CustomTransform new_ct = new();
            new_ct.custom_name = m_MarkerName0;
            new_ct.custom_position = new(m_DesirePosition0.x, m_DesirePosition0.y, m_DesirePosition0.z);
            new_ct.custom_euler_rotation = new(m_DesireRotation0.x, m_DesireRotation0.y, m_DesireRotation0.z);
            new_ct.custom_q_rotation = Quaternion.Euler(new_ct.custom_euler_rotation);

            if (!update0_added)
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

            update0_done = true;
            update0_added = true;
        }
    }

    [SerializeField]
    string m_MarkerName1 = "img_1";

    [SerializeField]
    //Vector3 m_DesirePosition1 = new(-1.017f, -0.715f, -2.94f);
    Vector3 m_DesirePosition1 = new(-0.963484585f, -0.752422929f, -2.93551755f);

    [SerializeField]
    [Tooltip("Unity will calculate X, Y, Z in this order.")]
    //Vector3 m_DesireRotation1 = new(0, 95f, 0);
    Vector3 m_DesireRotation1 = new(5.91967201f, 89.5457993f, 341.306091f);

    public bool m_Update1 = false;
    bool update1_done = false;
    bool update1_added = false;

    void Update1()
    {
        if (m_Update1 && !update1_done)
        {            
            CustomTransform new_ct = new();
            new_ct.custom_name = m_MarkerName1;
            new_ct.custom_position = new(m_DesirePosition1.x, m_DesirePosition1.y, m_DesirePosition1.z);
            new_ct.custom_euler_rotation = new(m_DesireRotation1.x, m_DesireRotation1.y, m_DesireRotation1.z);
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
    string m_MarkerName2 = "img_2";

    [SerializeField]
    Vector3 m_DesirePosition2 = new(-4.46681213f, -0.769292593f, -2.85921693f);

    [SerializeField]
    [Tooltip("Unity will calculate X, Y, Z in this order.")]
    Vector3 m_DesireRotation2 = new(6.22995043f, 91.6016846f, 6.37432766f);

    public bool m_Update2 = false;
    bool update2_done = false;
    bool update2_added = false;

    void Update2()
    {
        if (m_Update2 && !update2_done)
        {
            CustomTransform new_ct = new();
            new_ct.custom_name = m_MarkerName2;
            new_ct.custom_position = new(m_DesirePosition2.x, m_DesirePosition2.y, m_DesirePosition2.z);
            new_ct.custom_euler_rotation = new(m_DesireRotation2.x, m_DesireRotation2.y, m_DesireRotation2.z);
            new_ct.custom_q_rotation = Quaternion.Euler(new_ct.custom_euler_rotation);

            if (!update2_added)
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

            update2_done = true;
            update2_added = true;
        }
    }

    [SerializeField]
    string m_MarkerName3 = "img_3";

    [SerializeField]
    Vector3 m_DesirePosition3 = new(-3.97346735f, -1.88474703f, 3.09090471f);

    [SerializeField]
    [Tooltip("Unity will calculate X, Y, Z in this order.")]
    Vector3 m_DesireRotation3 = new(4.88889217f, 92.2085114f, 9.87865257f);

    public bool m_Update3 = false;
    bool update3_done = false;
    bool update3_added = false;

    void Update3()
    {
        if (m_Update3 && !update3_done)
        {
            CustomTransform new_ct = new();
            new_ct.custom_name = m_MarkerName3;
            new_ct.custom_position = new(m_DesirePosition3.x, m_DesirePosition3.y, m_DesirePosition3.z);
            new_ct.custom_euler_rotation = new(m_DesireRotation3.x, m_DesireRotation3.y, m_DesireRotation3.z);
            new_ct.custom_q_rotation = Quaternion.Euler(new_ct.custom_euler_rotation);

            if (!update3_added)
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

            update3_done = true;
            update3_added = true;
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
