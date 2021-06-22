using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCReceiverFace : MonoBehaviour
{

    public OSC osc;
    public Planet planet;


    float rotation = 0.0f;



    int activeInteraction;
    float spectralCentroid;
    float spectralFlux;
    float spectralSharpness;
    float volume;

    float frequency;
    float lucranity;
    float gain;


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
    if(activeInteraction != message.GetInt(0)){
            planet = transform.GetComponent<Planet>();
            planet.GetNextFace();
    }
    activeInteraction = message.GetInt(0);
    spectralCentroid = message.GetFloat(1);
    spectralFlux = message.GetFloat(2);
    spectralSharpness = message.GetFloat(3);
    volume = message.GetFloat(4);
        
    }

    // Update is called once per frame
    void Update()
    {

        frequency = spectralSharpness * (maxFreq - minFreq) + minFreq;
        lucranity = spectralFlux * (maxLuc - minLuc) + minLuc;
        gain = (1 - volume) * (maxGain - minGain) + minGain;


        if (activeInteraction == 0)
        {
            rotation = volume * volume * volume * 5.0f;
            if (rotation < 1.0f)
                rotation = 0.0f;

            if (rotation >= 0.0 && rotation < 50.0f)
                transform.Rotate(new Vector3(0.0f, rotation, 0.0f), Space.World);
            else
                transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f), Space.World);

           

        }
        else
        {
            planet = transform.GetComponent<Planet>();

            TerrainFace face = planet.GetCurrentFace();
            GameObject faceGameObject = planet.GetCurrentFaceGameObject();

            transform.LookAt(face.localUp);
            
           if (frequency < face.randomManager.m_frequency)
            {
                face.randomManager.m_frequency -= frequency/100.0f;
                face.randomManager.m_lacunarity -= lucranity / 100.0f;
                face.randomManager.m_gain -= gain / 100.0f;
            }
            else
            {
                face.randomManager.m_frequency += frequency / 100.0f;
                face.randomManager.m_lacunarity += lucranity / 100.0f;
                face.randomManager.m_gain += gain / 100.0f;
            }
           

            face.DeformMesh();
        }


    }
}