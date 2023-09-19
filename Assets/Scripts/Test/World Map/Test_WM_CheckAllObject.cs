using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class Test_WM_CheckAllObject : MonoBehaviour
{
    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    public ARSessionOrigin ARSessionOrigin
    {
        get { return m_ARSessionOrigin; }
    }

    [SerializeField]
    GameObject m_testGameObject;

    public GameObject TestGameObject
    {
        get { return m_testGameObject; }
    }

    [SerializeField]
    Text m_LogText;

    [SerializeField]
    Vector3 desireLocation;

    [SerializeField]
    bool useDesireLocation = false;

    public Text logText
    {
        get { return m_LogText; }
        set { m_LogText = value; }
    }

    public void LogAllObject()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        string longstr = "ALL objects are belong to mine!\n\n";

        foreach (GameObject gameObject in allObjects)
        {
            longstr += gameObject.name + "\n";
        }

        longstr += "\nTotal number: " + allObjects.Length;

        Debug.Log(longstr);
    }

    public void PlaceARAnchor()
    {
        ARAnchorManager anchorManager = m_ARSessionOrigin.GetComponent<ARAnchorManager>();

        if (anchorManager == null)
        {
            Debug.LogError("No anchor manager on ARSessionOrigin!");
            return;
        }

        if (m_testGameObject == null)
        {
            Debug.LogError("No test game object attached, please check again!");
            return;
        }

        if (m_testGameObject.GetComponent<ARAnchor>() == null)
        {
            m_testGameObject.AddComponent<ARAnchor>();
        }

        ARAnchor arAnchor = m_testGameObject.GetComponent<ARAnchor>();
        Debug.Log(string.Format(
            "AR anchor: {0} \ntrackableId: {1} \nsessionId: {2} \nnativePtr: {3}",
            arAnchor.name,
            arAnchor.trackableId,
            arAnchor.sessionId,
            arAnchor.nativePtr));
    }

    public void UseDesire()
    {
        if (useDesireLocation)
        {
            useDesireLocation = true;
            m_testGameObject.transform.position = desireLocation;
        }
        else
        {
            useDesireLocation = false;
            m_testGameObject.transform.position = Vector3.zero;
        }
    }

    private void Update()
    {
        logText.text = string.Format("Position:\n{0}\n\nRotation:\n{1}",
            m_testGameObject.transform.position,
            m_testGameObject.transform.rotation);
    }


}
