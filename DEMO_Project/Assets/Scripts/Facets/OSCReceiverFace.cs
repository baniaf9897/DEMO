using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCReceiverFace : MonoBehaviour
{

    public OSC osc;
    public Planet planet;


    float rotation = 0.0f;
    Color oldColor = new Color(0,0.96f,1);


    int activeInteraction;
    float spectralCentroid;
    float spectralFlux;
    float spectralSharpness;
    float volume;

    float frequency;
    float lucranity;
    float gain;


    static public float minFreq = 0.0f;
    static public float maxFreq = 20.0f;

    static public float minLuc = 0.0f;
    static public float maxLuc = 30.0f;

    static public float minGain = 0.0f;
    static public float maxGain = 1.1f;


    bool needToUpdate = false;

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

     // change of interaction mode
    if(activeInteraction != message.GetInt(0)){


            planet = transform.GetComponent<Planet>();
            GameObject faceGameObject = planet.GetCurrentFaceGameObject();
            faceGameObject.GetComponent<Renderer>().material.color = oldColor;

            if(message.GetInt(0) == 1)
            {
                planet.GetNextFace();
                faceGameObject = planet.GetCurrentFaceGameObject();
             
                oldColor = faceGameObject.GetComponent<Renderer>().material.color;
                faceGameObject.GetComponent<Renderer>().material.color = new Color(0.96f, 0.75f, 0.73f);
            }
            else
            {
                //Take a screenshot every time active interaction is finished
                Debug.Log(Application.dataPath + "/Screenshots/" + GetCurrentTime());
                ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshots/" + GetCurrentTime() + ".png");
            }
        }
    
        
        activeInteraction = message.GetInt(0);
        spectralCentroid = message.GetFloat(1);
        spectralFlux = message.GetFloat(2);
        spectralSharpness = message.GetFloat(3);
        volume = message.GetFloat(4);

        frequency = spectralCentroid * (maxFreq - minFreq) + minFreq;
        lucranity = (spectralCentroid + spectralFlux)  * (maxLuc - minLuc) + minLuc;
        gain =   spectralFlux  * (maxGain - minGain) + minGain;

        needToUpdate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (needToUpdate) {

            if (activeInteraction == 0)
            {
                rotation = volume * volume * volume * 10.0f;
                if (rotation < 1.0f)
                    rotation = 0.0f;

                if (rotation >= 0.0 && rotation < 50.0f) {

                    transform.Rotate(new Vector3(Random.Range(-2.0f, 2.0f * rotation), rotation, Random.Range(-2.0f, 2.0f * rotation)), Space.World);
                }
                else
                    transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f), Space.World);

            }
            else
            {
                if(volume > 0.3f)
                {
                    planet = transform.GetComponent<Planet>();
                    TerrainFace face = planet.GetCurrentFace();

                    frequency = frequency - face.shapeGenerator.randomManager.m_frequency;
                    lucranity = lucranity - face.shapeGenerator.randomManager.m_lacunarity;
                    gain = (gain - face.shapeGenerator.randomManager.m_gain) / 5.0f ;

                    Debug.Log(frequency);
                    face.shapeGenerator.randomManager.addFreq(frequency / 100.0f);
                    face.shapeGenerator.randomManager.addLuc(lucranity / 100.0f);
                    face.shapeGenerator.randomManager.addGain(gain / 100.0f);

                    face.ConstructMesh();
                }
             };

                /*if (frequency < face.shapeGenerator.randomManager.m_frequency && volume > 0.3f)
                 {

                     frequency = frequency - face.shapeGenerator.randomManager.m_frequency;
                     lucranity = lucranity - face.shapeGenerator.randomManager.m_lacunarity;
                     gain = gain - face.shapeGenerator.randomManager.m_gain;

                     face.shapeGenerator.randomManager.addFreq(-1 * frequency / 100.0f);
                     face.shapeGenerator.randomManager.addLuc(-1 * frequency / 100.0f);
                     face.shapeGenerator.randomManager.addGain(-1 * frequency / 100.0f);

                 }
                 else if(frequency > face.shapeGenerator.randomManager.m_frequency && volume > 0.3f)
                 {
                     frequency = frequency - face.shapeGenerator.randomManager.m_frequency;
                     lucranity = lucranity - face.shapeGenerator.randomManager.m_lacunarity;
                     gain = gain - face.shapeGenerator.randomManager.m_gain;

                     face.shapeGenerator.randomManager.addFreq(frequency / 100.0f);
                     face.shapeGenerator.randomManager.addLuc(lucranity / 100.0f);
                     face.shapeGenerator.randomManager.addGain(gain / 100.0f);

                 }*/
                needToUpdate = false;
            }
    }

    string GetCurrentTime()
    {
        return System.DateTime.Now.ToString().Replace("/","-").Replace(" ", "_").Replace(":", "-");
    }
}
