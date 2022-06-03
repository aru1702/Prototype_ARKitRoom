using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPData
{
    public string name { set; get; }
    public string ip_address { set; get; }
    public string authentication { set; get; }

    public IPData(string name, string ip_address, string authentication)
    {
        this.name = name;
        this.ip_address = ip_address;
        this.authentication = authentication;
    }
}
