using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_PositionLogging : MonoBehaviour
{
    public GameObject AR_Camera;

    // apply this script on Text gameobject
    private Text UI_Text;
    private Vector3 Camera_position;

    // Start is called before the first frame update
    void Start()
    {
        UI_Text = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Camera_position = AR_Camera.transform.position;
        UI_Text.text = Camera_position.ToString();
    }
}
