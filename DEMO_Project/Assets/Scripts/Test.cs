using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update

    public RandomManager randomManager = new RandomManager();

    void Start()
    {
        randomManager.m_frequency = 0.1f;
        randomManager.m_gain = 0.1f;
        randomManager.m_lacunarity = 0.1f;
        randomManager.m_NumOctaves = 2;
        randomManager.m_offsetX = 2;
        randomManager.m_offsetY = 4;
        randomManager.m_offsetZ= 1;

        Debug.Log(randomManager.FBM3D(1.0f, 1.0f, 1.0f));
        Debug.Log(randomManager.FBM3D(1.0f, 1.0f, 1.0f));
        Debug.Log(randomManager.FBM3D(2.0f, 1.0f, 2.0f));
        Debug.Log(randomManager.FBM3D(1.0f, 2.0f, 1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
