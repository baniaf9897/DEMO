using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSC_Reveiver : MonoBehaviour
{

    public OSC osc;

    // Start is called before the first frame update
    void Start()
    {
        osc.SetAddressHandler("/value/", OnMessage);
    }

    void OnMessage(OscMessage message)
    {
        float value = message.GetFloat(0);

        Debug.Log("Value received :" + value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
