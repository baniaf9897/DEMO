using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{

    public float height;

    public ShapeGenerator(float height)
    {
        this.height = height;
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        return pointOnUnitSphere * height;
    }
}
