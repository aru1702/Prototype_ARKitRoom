using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_JustAnotherScript : MonoBehaviour
{
    enum Ver { Version1, Version2 };

    [SerializeField]
    int m_SaveMap = 0;

    [SerializeField]
    int m_LoadMap = 0;

    [SerializeField]
    Ver m_CorrectionVersion;

    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    GameObject[] m_PanelUIs;

    // Start is called before the first frame update
    void Start()
    {
        //try
        //{
        //    bool a = transform.parent == transform.root;
        //    bool b = gameObject.transform.parent != null;
        //    //string c = gameObject.transform.parent.gameObject.name;
        //    Debug.Log("name: " + gameObject.name +
        //                " + parent=root: " + a +
        //                " + hasparent: " + b);
        //                //" + parentname: " + c);
        //}
        //catch (System.Exception e)
        //{
        //    Debug.Log(e);
        //}

        GlobalConfig.SAVE_INTO_MAP = m_SaveMap;
        GlobalConfig.LOAD_MAP = m_LoadMap;
        GlobalConfig.TempOriginGO = new();
        GlobalConfig.TEST_MODE = true;
        GlobalConfig.CorrectionFunctionVersion = ((int)m_CorrectionVersion) + 1;

        m_LoadObjectManager.SetActive(true);

        foreach (var item in m_PanelUIs)
        {
            item.SetActive(true);
        }

        //Debug.Log(Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
