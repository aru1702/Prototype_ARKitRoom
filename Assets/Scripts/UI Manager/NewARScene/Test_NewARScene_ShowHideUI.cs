using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_NewARScene_ShowHideUI : MonoBehaviour
{
    [SerializeField]
    List<GameObject> m_UIList;

    bool m_HasActive = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowHideUI()
    {
        try
        {
            if (m_UIList.Count <= 0) { }
        }
        catch (System.Exception ex)
        {
            Debug.Log("No object attached on UI List.\nEx: " + ex);
            return;
        }

        ActiveDeactiveObject(!m_HasActive);
        m_HasActive = !m_HasActive;
    }

    void ActiveDeactiveObject(bool trigger)
    {
        foreach (var o in m_UIList)
        {
            o.SetActive(trigger);
        }
    }
}
