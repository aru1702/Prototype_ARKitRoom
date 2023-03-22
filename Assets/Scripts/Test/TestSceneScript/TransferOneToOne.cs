using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferOneToOne : MonoBehaviour
{
    [SerializeField]
    GameObject m_Source, m_Destination;

    // Start is called before the first frame update
    void Start()
    {
        var sTow = m_Source.transform.localToWorldMatrix;
        var dTow = m_Destination.transform.localToWorldMatrix;

        // this is true
        var sTod = dTow.inverse * sTow;
        Vector3 init_pos = m_Destination.transform.position;
        Vector3 new_pos = sTod * init_pos;
        m_Source.transform.position = new_pos;

        ////////////

        // nothing happened
        //var dTos = dTow.inverse * sTow;
        //Vector3 new_pos_2 = dTos * m_Source.transform.position;
        //m_Source.transform.position = new_pos_2;

        // only if both object rotation are identity (0,0,0) in euler
        // scale is not affecting
        // shear hasn't tested yet
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
