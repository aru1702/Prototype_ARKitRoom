using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_JustAnotherScript : MonoBehaviour
{
    [SerializeField]
    int m_SaveMap = 0;

    [SerializeField]
    int m_LoadMap = 0;

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

        //Debug.Log(Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
