using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_TestARScene_TestChangeText : MonoBehaviour
{
    [SerializeField]
    GameObject m_Prefab;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = Instantiate(m_Prefab);
        go.name = "testGO";
        go.transform.position = new(1, 4, -2);
        go.transform.eulerAngles = new(0, 90, 0);

        var pos = go.transform.position;
        var rot = go.transform.rotation;

        TextMesh tM = go.transform.GetChild(1).GetComponent<TextMesh>();
        tM.text = "Position: " + pos.ToString() + "\nRotation: " + rot.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
