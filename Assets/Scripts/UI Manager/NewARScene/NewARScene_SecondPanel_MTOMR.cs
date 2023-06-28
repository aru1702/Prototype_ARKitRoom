using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewARScene_SecondPanel_MTOMR : MonoBehaviour
{
    [SerializeField]
    GameObject m_VersionOneWithRotation;

    CorrectionFunctions.VersionOneWithRotation vor;


    // Start is called before the first frame update
    void Start()
    {
        vor = m_VersionOneWithRotation.GetComponent<CorrectionFunctions.VersionOneWithRotation>();
    }


    public void ButtonStartMTO() { vor.SetStartMTO(true); }

    public void ButtonStopMTO() { vor.SetStartMTO(false);}

    public void ButtonStartMR() { vor.SetStartMR(true); }

    public void ButtonStopMR() { vor.SetStartMR(false); }

    public void ButtonResetR() { vor.ResetR(); }

    public void ButtonResetAllObj() { vor.ResetAll(); }

    public void CameraUpdateaModeToggle(Toggle toggle) { vor.EnableCameraUpdateMode(toggle.isOn); }
}
