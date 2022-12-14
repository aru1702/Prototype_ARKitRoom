using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_NewARScene_InputImgTrackingData : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Disable this in real build, since all function run in Update")]
    bool m_EnableThisFunction = false;

    [SerializeField]
    bool m_Reset = false;

    [SerializeField]
    bool m_InputData1 = false;

    [SerializeField]
    Vector3 m_Data1 = new(-0.9675f, -0.713f, -2.909f);

    [SerializeField]
    bool m_InputData2 = false;

    [SerializeField]
    Vector3 m_Data2 = new(-4.8975f, -0.711f, -2.899f);

    bool hasInputed1 = false;
    bool hasInputed2 = false;

    [SerializeField]
    bool m_ImgTrackingHasUpdate = false;

    [SerializeField]
    GameObject m_ImageTrackingCorrectionHandler;

    void Update()
    {
        if (!m_EnableThisFunction) return;

        InputData();
        HasUpdate();
        Reset();
    }

    void InputData()
    {
        if (m_InputData1)
        {
            if (!hasInputed1)
            {
                CustomTransform newImgTgt = new();
                newImgTgt.custom_name = "img_1";
                newImgTgt.custom_position = m_Data1;
                newImgTgt.customer_q_rotation = new();
                newImgTgt.custom_euler_rotation = new(0, 90f, 0);

                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputData(newImgTgt);

                hasInputed1 = true;
                m_ImgTrackingHasUpdate = true;
            }
            else
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .UpdateInputData("img_1",m_Data1);

                m_ImgTrackingHasUpdate = true;
            }
        }

        if (m_InputData2)
        {
            if (!hasInputed2)
            {
                CustomTransform newImgTgt = new();
                newImgTgt.custom_name = "img_2";
                newImgTgt.custom_position = m_Data2;
                newImgTgt.customer_q_rotation = new();
                newImgTgt.custom_euler_rotation = new(0, 90f, 0);

                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputData(newImgTgt);

                hasInputed2 = true;
                m_ImgTrackingHasUpdate = true;
            }
            else
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .UpdateInputData("img_2", m_Data2);

                m_ImgTrackingHasUpdate = true;
            }
        }
    }

    void HasUpdate()
    {
        m_ImageTrackingCorrectionHandler
            .GetComponent<NewARSceneImageTrackingCorrection>()
            .UpdateHasUpdate(m_ImgTrackingHasUpdate);
    }

    void Reset()
    {
        if (m_Reset)
        {
            m_InputData1 = false;
            m_InputData2 = false;
            m_ImgTrackingHasUpdate = false;
            m_Reset = false;
        }
    }
}
