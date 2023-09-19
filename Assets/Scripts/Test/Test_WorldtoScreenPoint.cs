using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_WorldtoScreenPoint : MonoBehaviour
{
    [SerializeField]
    Camera m_Camera;

    [SerializeField]
    GameObject m_TargetedGameObject, m_TargetedGameObject2;

    [SerializeField]
    Text m_DebugText;

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = m_Camera.WorldToScreenPoint(m_TargetedGameObject.transform.position);
        Vector3 screenPos2 = m_Camera.WorldToScreenPoint(m_TargetedGameObject2.transform.position);
        string debugText = string.Format("Targeted 1:\n" +
            "{0} pixels from left\n" +
            "{1} pixels from bottom\n" +
            "{2} is z position\n\n" +
            "" +
            "Targeted 2:\n" +
            "{3} pixels from left\n" +
            "{4} pixels from bottom\n" +
            "{5} is z position",
            screenPos.x, screenPos.y, System.Math.Abs(screenPos.z),
            screenPos2.x, screenPos2.y, System.Math.Abs(screenPos2.z));
        m_DebugText.text = debugText;
    }
}
