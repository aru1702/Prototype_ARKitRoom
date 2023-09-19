using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_NewARScene_ShowAxisObjectOnly : MonoBehaviour
{
    List<GameObject> m_ObjectWithoutAxis;
    bool m_HasActive = true;
    string[] m_ExceptorStrings = new string[] { "dummy", "img" };

    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    GameObject m_TestWorldCalibrationManager;

    bool fromLoad, fromTestWorld = false;

    // Start is called before the first frame update
    void Start()
    {
        m_ObjectWithoutAxis = new();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetObjectWithoutAxis()
    {
        var objects = m_LoadObjectManager
            .GetComponent<LoadObject_CatExample_2__NewARScene>()
            .GetMyObjects();

        if (objects.Count > 0)
        {
            foreach (var o in objects)
            {
                string[] names = o.name.Split("_");
                bool is_equal = false;

                for (int i = 0; i < m_ExceptorStrings.Length; i++)
                {
                    if (Equals(names[0], m_ExceptorStrings[i]))
                    {
                        is_equal = true;
                    }
                }

                if (!is_equal) m_ObjectWithoutAxis.Add(o);
            }

            fromLoad = true;
        }

        var calibrations = m_TestWorldCalibrationManager
            .GetComponent<Test_TurnOnOffWorldCalib>()
            .GetCloneObject();

        if (calibrations.Count > 0)
        {
            foreach (var o in calibrations)
            {
                string[] names = o.name.Split("_");
                bool is_equal = false;

                for (int i = 0; i < m_ExceptorStrings.Length; i++)
                {
                    if (Equals(names[0], m_ExceptorStrings[i]))
                    {
                        is_equal = true;
                    }
                }

                if (!is_equal) m_ObjectWithoutAxis.Add(o);
            }

            fromTestWorld = true;
        }
    }

    public void ShowAxisObjectOnly()
    {
        if (fromLoad == false || fromTestWorld == false) GetObjectWithoutAxis();

        ActiveDeactiveObject(!m_HasActive);
        m_HasActive = !m_HasActive;
    }

    void ActiveDeactiveObject(bool trigger)
    {
        foreach (var o in m_ObjectWithoutAxis)
        {
            o.SetActive(trigger);
        }
    }
}
