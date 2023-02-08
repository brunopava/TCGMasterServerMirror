using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
    // public Transform[] controlPoints;
    
    // private int curveCount = 1;
    // public float factor = 0.75f;

    // private void Start()
    // {
    //     curveCount = (int)controlPoints.Length / 3;
    // }

    // private void Update()
    // {
    //     Reposition();
    // }

    // private void Reposition()
    // {    
    //     if( transform.childCount <= 0)
    //          return;
        
    //     for (int j = 0; j < curveCount; j++)
    //     {
    //         for(int i = 0; i < transform.childCount; i++)
    //         {
    //             float t = (1f / (float)transform.childCount) * ((float)(i+1) * factor);
    //             int nodeIndex = j * 3;
    //             Vector3 pixel = CalculateCubicBezierPoint(t, controlPoints [nodeIndex].position, controlPoints [nodeIndex + 1].position, controlPoints [nodeIndex + 2].position, controlPoints [nodeIndex + 3].position);

    //             Transform child = transform.GetChild(i);
    //             child.position =  new Vector3(pixel.x,
    //                 child.position.y,
    //                 pixel.z
    //             );

    //             float endPosX = child.position.x - transform.position.x;
    //             float endPosY = child.position.z - transform.position.z;

    //             float angle = Mathf.Atan2(endPosX, endPosY) * Mathf.Rad2Deg;
    //             child.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
    //         }     
    //     }
    // }
        
    // private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    // {
    //     float u = 1 - t;
    //     float tt = t * t;
    //     float uu = u * u;
    //     float uuu = uu * u;
    //     float ttt = tt * t;
        
    //     Vector3 p = uuu * p0; 
    //     p += 3 * uu * t * p1; 
    //     p += 3 * u * tt * p2; 
    //     p += ttt * p3; 
        
    //     return p;
    // }

    public float spacing = 1.0f;
    public float parabolaWidth = 1.0f;
    public float parabolaHeight = 1.0f;
    public float rotation = 30.0f;

    private int lastChildCount = 0;

    void Update()
    {
        ReorderCards();
    }

    public void ReorderCards()
    {
        int childCount = transform.childCount;

        if(lastChildCount != childCount)
        {
            lastChildCount = childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);

                // calculate x and y positions using parabolic equation
                float x = spacing * i - (childCount - 1) * spacing / 2;
                float y = -parabolaWidth * x * x / ((childCount - 1) * spacing) + parabolaHeight;
                float yRotation = rotation * x / ((childCount - 1) * spacing);

                if(childCount == 1)
                {
                    x = spacing * i - spacing / 2;
                    y = -parabolaWidth * x * x / spacing + parabolaHeight;
                    yRotation = 0f;
                }

                Vector3 cardPosition = new Vector3(x, child.localPosition.y, y);
                Quaternion cardRotation = Quaternion.Euler(0, yRotation, 0);
                
                if(cardPosition != null)
                {
                    child.localPosition = cardPosition;
                }

                if(cardRotation != null)
                {
                    child.localRotation = cardRotation;
                }
            }
        }
    }
}
