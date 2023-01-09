using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
    public Transform[] controlPoints;
    
    private int curveCount = 1;
    public float factor = 0.75f;

    private void Start()
    {
        curveCount = (int)controlPoints.Length / 3;
    }

    private void Update()
    {
        Reposition();
    }

    private void Reposition()
    {    
        if( transform.childCount <= 0)
             return;
        
        for (int j = 0; j < curveCount; j++)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                float t = (1f / (float)transform.childCount) * ((float)(i+1) * factor);
                int nodeIndex = j * 3;
                Vector3 pixel = CalculateCubicBezierPoint(t, controlPoints [nodeIndex].position, controlPoints [nodeIndex + 1].position, controlPoints [nodeIndex + 2].position, controlPoints [nodeIndex + 3].position);

                Transform child = transform.GetChild(i);
                child.position =  new Vector3(pixel.x,
                    child.position.y,
                    pixel.z
                );
            }     
        }
    }
        
    private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        
        Vector3 p = uuu * p0; 
        p += 3 * uu * t * p1; 
        p += 3 * u * tt * p2; 
        p += ttt * p3; 
        
        return p;
    }
}
