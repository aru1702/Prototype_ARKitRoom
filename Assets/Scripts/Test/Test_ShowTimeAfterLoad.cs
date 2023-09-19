using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_ShowTimeAfterLoad : MonoBehaviour
{
    [SerializeField]
    GameObject m_UIManager;

    UIManager_CatExample UIManager;

    void Start()
    {
        UIManager = m_UIManager.GetComponent<UIManager_CatExample>();
    }

    public void ShowTime()
    {
        float start_time = GlobalConfig.AFTER_LOAD_START_TIME;
        if (start_time == 0.0f)
        {
            UIManager.MapStatus.text = "No starting time, cannot calculate intermediate time.";
            UIManager.OpenPanel();
        }

        float end_time = Time.time;
        float time_spend = System.Math.Abs(end_time - start_time);

        UIManager.MapStatus.text = "Time spend: " + time_spend.ToString("0.00") + " secs.";
        UIManager.OpenPanel();
    }
}
