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
        //active flag
        //m_spectralCentroid
        //m_spectralFlux 
        //m_spectralSharpness
        if (!transform.GetComponent<DeformationManager>().isCalculating) {
            Debug.Log("Calc");

            int activeInteraction = message.GetInt(0);
            float spectralCentroid = message.GetFloat(1);
            float spectralFlux = message.GetFloat(2);
            float spectralSharpness = message.GetFloat(3);

            transform.GetComponent<DeformationManager>().randomManager1.m_frequency = spectralCentroid/1000.0f;
            transform.GetComponent<DeformationManager>().randomManager2.m_frequency = spectralCentroid / 1000.0f;
            transform.GetComponent<DeformationManager>().randomManager3.m_frequency = spectralCentroid / 1000.0f;

            transform.GetComponent<DeformationManager>().randomManager1.m_gain = spectralSharpness / 1000.0f;
            transform.GetComponent<DeformationManager>().randomManager2.m_gain = spectralSharpness / 1000.0f;
            transform.GetComponent<DeformationManager>().randomManager3.m_gain = spectralSharpness / 1000.0f;


            transform.GetComponent<DeformationManager>().randomManager1.m_lacunarity = spectralFlux / 100.0f;
            transform.GetComponent<DeformationManager>().randomManager2.m_lacunarity = spectralFlux / 100.0f;
            transform.GetComponent<DeformationManager>().randomManager3.m_lacunarity = spectralFlux / 100.0f;


            // transform.GetComponent<DeformationManager>().UpdateSphere();
        }
        //Debug.Log("Value received :" + activeInteraction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
