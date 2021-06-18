using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSC_Reveiver : MonoBehaviour
{

    public OSC osc;

    float rotation = 0.0f;



    int activeInteraction;
    float spectralCentroid;
    float spectralFlux;
    float spectralSharpness;
    float volume;


    float minFreq = 2.0f;
    float maxFreq = 10.0f;

    float minLuc = 0.1f;
    float maxLuc = 10.0f;

    float minGain = 0.1f;
    float maxGain = 2.0f;

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
        //m_volume
        if (!transform.GetComponent<DeformationManager>().isCalculating) {
            activeInteraction = message.GetInt(0);
            spectralCentroid = message.GetFloat(1);
            spectralFlux = message.GetFloat(2);
            spectralSharpness = message.GetFloat(3);
            volume = message.GetFloat(4);
        }
    }

    // Update is called once per frame
    void Update()
    {

        spectralSharpness = spectralSharpness * (maxFreq - minFreq) + minFreq;
        spectralFlux = spectralFlux * (maxLuc - minLuc) + minLuc;
        spectralCentroid = spectralCentroid * (maxGain - minGain) + minGain;


        if (activeInteraction == 0)
        {
            rotation = volume * 10.0f;
            if(rotation >= 0.0 && rotation < 50.0f)
                transform.Rotate(new Vector3(0.0f, rotation, 0.0f), Space.Self);
            else
                transform.Rotate(new Vector3(0.0f, 2.0f, 0.0f), Space.Self);

        }
        else
        {
            
            transform.GetComponent<DeformationManager>().randomManager1.m_frequency = spectralSharpness;
            transform.GetComponent<DeformationManager>().randomManager2.m_frequency = spectralSharpness;
            transform.GetComponent<DeformationManager>().randomManager3.m_frequency = spectralSharpness;

            transform.GetComponent<DeformationManager>().randomManager1.m_gain = spectralCentroid;
            transform.GetComponent<DeformationManager>().randomManager2.m_gain = spectralCentroid;
            transform.GetComponent<DeformationManager>().randomManager3.m_gain = spectralCentroid;


            transform.GetComponent<DeformationManager>().randomManager1.m_lacunarity = spectralFlux;
            transform.GetComponent<DeformationManager>().randomManager2.m_lacunarity = spectralFlux;
            transform.GetComponent<DeformationManager>().randomManager3.m_lacunarity = spectralFlux;

        }

   
    }
}
