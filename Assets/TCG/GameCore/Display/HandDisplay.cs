using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
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
