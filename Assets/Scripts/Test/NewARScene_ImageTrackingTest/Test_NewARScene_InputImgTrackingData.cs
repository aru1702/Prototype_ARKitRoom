using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_NewARScene_InputImgTrackingData : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Disable this in real build, since all function run in Update")]
    bool m_EnableThisFunction = false;

    [SerializeField]
    bool m_AutonomousTimedMode = false;
    bool autonomousHasOn = false;

    [SerializeField]
    bool m_Reset = false;

    [SerializeField]
    bool m_InputData0 = false;

    [SerializeField]
    bool m_InputData1 = false;

    [SerializeField]
    bool m_InputData2 = false;

    [SerializeField]
    bool m_InputData3 = false;

    [SerializeField]
    bool m_InputData4 = false;

    [SerializeField]
    bool m_InputData5 = false;


    /*
    marker ground truth in SLAM coordinate
        img_0	0		0		0
        img_1	-1.017	-0.715	-2.959
        img_2	-4.517	-0.715	-2.959
        img_3	-4.017	-0.715	3.041
        img_4	-1.017	-0.715	10.541
        img_5	-4.517	-0.715	7.541
    */

    [SerializeField]
    Vector3 m_Data0 = new(-0.01f, 0f, 0.01f);

    [SerializeField]
    Vector3 m_Data1 = new(-0.9675f, -0.713f, -2.909f);     

    [SerializeField]
    Vector3 m_Data2 = new(-4.3975f, -0.711f, -2.899f);

    [SerializeField]
    Vector3 m_Data3 = new(-4.087f, -0.717f, 3.102f);

    [SerializeField]
    Vector3 m_Data4 = new(-1.045f, -0.715f, 10.909f);

    [SerializeField]
    Vector3 m_Data5 = new(-4.410f, -0.710f, 7.199f);

    bool hasInputed0, hasInputedR0 = false;
    bool hasInputed1, hasInputedR1 = false;
    bool hasInputed2, hasInputedR2 = false;
    bool hasInputed3, hasInputedR3 = false;
    bool hasInputed4, hasInputedR4 = false;
    bool hasInputed5, hasInputedR5 = false;

    [SerializeField]
    bool m_ImgTrackingHasUpdate = false;

    [SerializeField]
    GameObject m_ImageTrackingCorrectionHandler;

    /// <summary>
    /// ///////////////////
    /// </summary>

    [SerializeField]
    [Tooltip("DO NOT TOUCH THIS, AS THIS IS TRIGGER ONLY")]
    bool AutonomousDoneDoNotTouch = false;

    [SerializeField]
    [Tooltip("DO NOT TOUCH THIS, AS THIS IS TRIGGER ONLY")]
    float AutonomousTimeDoNotTouch = 0;
    float lasttime;

    [SerializeField]
    [Tooltip("DO NOT TOUCH THIS, AS THIS IS TRIGGER ONLY")]
    int AutonomousCountDoNotTouch = 0;  // total: 10 times

    /// <summary>
    /// ///////////////////
    /// </summary>

    private void OnEnable()
    {
        m_ImageTrackingCorrectionHandler
            .GetComponent<CorrectionFunctions.VersionTwoA>()
            .SetUnityEditorModeTrue();
    }

    void Update()
    {
        if (!m_EnableThisFunction) return;

        InputData();
        HasUpdate();
        Reset();

        if (m_AutonomousTimedMode)
        {
            if (!autonomousHasOn)
            {
                StartCoroutine(AutonomousMode());
                autonomousHasOn = true;
            }

            if (!AutonomousDoneDoNotTouch)
            {
                var current_time = Time.time;
                AutonomousTimeDoNotTouch += Mathf.Abs(current_time - lasttime);
                lasttime = current_time;
            }
        }
    }

    void InputData()
    {
        if (m_InputData0)
        {
            CustomTransform newImgTgt = new();
            newImgTgt.custom_name = "img_0";
            newImgTgt.custom_position = m_Data0;
            newImgTgt.customer_q_rotation = new();
            newImgTgt.custom_euler_rotation = new(0, 0, 0);

            if (!hasInputed0)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputData(newImgTgt);
                hasInputed0 = true;
            }

            if (!hasInputedR0)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputDataRemove(newImgTgt);
                hasInputedR0 = true;
            }

            m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .UpdateInputData("img_0", m_Data0);
            m_ImgTrackingHasUpdate = true;
        }

        if (m_InputData1)
        {
            CustomTransform newImgTgt = new();
            newImgTgt.custom_name = "img_1";
            newImgTgt.custom_position = m_Data1;
            newImgTgt.customer_q_rotation = new();
            newImgTgt.custom_euler_rotation = new(0, 90f, 0);

            if (!hasInputed1)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputData(newImgTgt);
                hasInputed1 = true;               
            }

            if (!hasInputedR1)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputDataRemove(newImgTgt);
                hasInputedR1 = true;
            }

            m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .UpdateInputData("img_1", m_Data1);
            m_ImgTrackingHasUpdate = true;
        }

        if (m_InputData2)
        {
            CustomTransform newImgTgt = new();
            newImgTgt.custom_name = "img_2";
            newImgTgt.custom_position = m_Data2;
            newImgTgt.customer_q_rotation = new();
            newImgTgt.custom_euler_rotation = new(0, 90f, 0);

            if (!hasInputed2)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputData(newImgTgt);
                hasInputed2 = true;
            }

            if (!hasInputedR2)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputDataRemove(newImgTgt);
                hasInputedR2 = true;
            }
            
            m_ImageTrackingCorrectionHandler
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .UpdateInputData("img_2", m_Data2);
            m_ImgTrackingHasUpdate = true;
        }

        if (m_InputData3)
        {
            CustomTransform newImgTgt = new();
            newImgTgt.custom_name = "img_3";
            newImgTgt.custom_position = m_Data3;
            newImgTgt.customer_q_rotation = new();
            newImgTgt.custom_euler_rotation = new(0, 90f, 0);

            if (!hasInputed3)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputData(newImgTgt);
                hasInputed3 = true;
            }

            if (!hasInputedR3)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputDataRemove(newImgTgt);
                hasInputedR3 = true;
            }

            m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .UpdateInputData("img_3", m_Data3);
            m_ImgTrackingHasUpdate = true;
        }

        if (m_InputData4)
        {
            CustomTransform newImgTgt = new();
            newImgTgt.custom_name = "img_4";
            newImgTgt.custom_position = m_Data4;
            newImgTgt.customer_q_rotation = new();
            newImgTgt.custom_euler_rotation = new(0, 90f, 0);

            if (!hasInputed4)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputData(newImgTgt);
                hasInputed4 = true;
            }

            if (!hasInputedR4)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputDataRemove(newImgTgt);
                hasInputedR4 = true;
            }

            m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .UpdateInputData("img_4", m_Data4);
            m_ImgTrackingHasUpdate = true;
        }

        if (m_InputData5)
        {
            CustomTransform newImgTgt = new();
            newImgTgt.custom_name = "img_5";
            newImgTgt.custom_position = m_Data5;
            newImgTgt.customer_q_rotation = new();
            newImgTgt.custom_euler_rotation = new(0, 90f, 0);

            if (!hasInputed5)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputData(newImgTgt);
                hasInputed5 = true;
            }

            if (!hasInputedR5)
            {
                m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .TestInputDataRemove(newImgTgt);
                hasInputedR5 = true;
            }

            m_ImageTrackingCorrectionHandler
                    .GetComponent<NewARSceneImageTrackingCorrection>()
                    .UpdateInputData("img_5", m_Data5);
            m_ImgTrackingHasUpdate = true;
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
            m_InputData0 = false;
            m_InputData1 = false;
            m_InputData2 = false;
            m_InputData3 = false;
            m_InputData4 = false;
            m_InputData5 = false;

            m_ImgTrackingHasUpdate = false;

            m_ImageTrackingCorrectionHandler
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .ResetImageTrackedListWithRemove();
            hasInputedR0 = false;
            hasInputedR1 = false;
            hasInputedR2 = false;
            hasInputedR3 = false;
            hasInputedR4 = false;
            hasInputedR5 = false;

            m_Reset = false;
        }
    }

    IEnumerator AutonomousMode()
    {
        // 0-1-2-3-4-5-1-3-2-0
        if (!AutonomousDoneDoNotTouch)
        {
            yield return new WaitForSeconds(2);

            m_InputData0 = true;
            yield return new WaitForSeconds(1);
            m_Reset = true;
            AutonomousCountDoNotTouch++;

            /////////
            ///

            yield return new WaitForSeconds(20);

            m_InputData1 = true;
            yield return new WaitForSeconds(1);
            m_Reset = true;
            AutonomousCountDoNotTouch++;

            /////////
            ///

            yield return new WaitForSeconds(35);

            m_InputData2 = true;
            yield return new WaitForSeconds(1);
            m_Reset = true;
            AutonomousCountDoNotTouch++;

            /////////
            ///

            yield return new WaitForSeconds(40);

            m_InputData3 = true;
            yield return new WaitForSeconds(1);
            m_Reset = true;
            AutonomousCountDoNotTouch++;

            /////////
            ///

            yield return new WaitForSeconds(40);

            m_InputData4 = true;
            yield return new WaitForSeconds(1);
            m_Reset = true;
            AutonomousCountDoNotTouch++;

            /////////
            ///

            yield return new WaitForSeconds(60);

            m_InputData5 = true;
            yield return new WaitForSeconds(1);
            m_Reset = true;
            AutonomousCountDoNotTouch++;

            /////////
            ///

            yield return new WaitForSeconds(60);

            m_Data1 = new(-0.9475f, -0.713f, -2.909f);
            m_InputData1 = true;
            yield return new WaitForSeconds(1);
            m_Reset = true;
            AutonomousCountDoNotTouch++;

            /////////
            ///

            yield return new WaitForSeconds(15);

            m_Data3 = new(-4.007f, -0.717f, 3.152f);
            m_InputData3 = true;
            yield return new WaitForSeconds(1);
            m_Reset = true;
            AutonomousCountDoNotTouch++;

            /////////
            ///

            yield return new WaitForSeconds(40);

            m_Data2 = new(-4.4575f, -0.711f, -2.839f);
            m_InputData2 = true;
            yield return new WaitForSeconds(1);
            m_Reset = true;
            AutonomousCountDoNotTouch++;

            /////////
            ///

            yield return new WaitForSeconds(20);

            m_Data0 = new(-0.11f, 0f, -0.05f);
            m_InputData0 = true;
            yield return new WaitForSeconds(1);
            m_Reset = true;
            AutonomousCountDoNotTouch++;

            /////////
            ///

            AutonomousDoneDoNotTouch = true;
        }
    }
}
