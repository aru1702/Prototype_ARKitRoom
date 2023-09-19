using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransformRotationTransformation : MonoBehaviour
{
    [SerializeField]
    GameObject m_GTObjectImport;

    [SerializeField]
    GameObject m_RTObjectImport;

    //[SerializeField]
    //Vector3 m_TargetPositionOrigin;

    [SerializeField]
    Quaternion m_TargetRotation;

    bool done = false;

    private void Start()
    {
        StartCoroutine(routine());
    }

    IEnumerator routine()
    {
        //try { if (!done) MainFunc(); } catch (System.Exception) { }
        //while (true)
        //{
        yield return new WaitForSeconds(2);
        MainFunc();
        //}
    }

    void MainFunc()
    {
        Quaternion GT_Rotation = m_GTObjectImport
            .GetComponent<ObjectTransformRotationObjectImport>()
            .GetAverageRotation();
        Quaternion gt_to_w = m_TargetRotation * Quaternion.Inverse(GT_Rotation);
        m_GTObjectImport
            .GetComponent<ObjectTransformRotationObjectImport>()
            .GetRoot()
            .transform.rotation *= gt_to_w;

        Quaternion RT_Rotation = m_RTObjectImport
            .GetComponent<ObjectTransformRotationObjectImport>()
            .GetAverageRotation();
        Quaternion rt_to_w = m_TargetRotation * Quaternion.Inverse(RT_Rotation);
        m_RTObjectImport
            .GetComponent<ObjectTransformRotationObjectImport>()
            .GetRoot()
            .transform.rotation *= rt_to_w;

        done = true;
    }
}
