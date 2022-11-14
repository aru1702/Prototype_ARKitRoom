using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_AnObjectWithTextSetLocToDesignatedWorldOrigin : MonoBehaviour
{
    [SerializeField]
    GameObject m_TargetedGameObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalConfig.PlaySpaceOriginGO == null) { return; }

        var text = m_TargetedGameObject.GetComponent<CrossLikeText>();            
        var alt_m44 = GlobalConfig.GetM44ByGameObjRef(m_TargetedGameObject, GlobalConfig.PlaySpaceOriginGO);
        var pos = GlobalConfig.GetPositionFromM44(alt_m44);
        var rot = GlobalConfig.GetEulerAngleFromM44(alt_m44);
        var str = pos + "\n" + rot;
        text.SetCrossLikeText(str);
    }
}
