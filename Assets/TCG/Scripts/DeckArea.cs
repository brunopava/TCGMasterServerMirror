using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckArea : MonoBehaviour
{
    private int lastChildCount = 0;

    private void Update()
    {
        if(lastChildCount == transform.childCount)
            return;

        lastChildCount = transform.childCount;
        Reposition();
    }

    private void Reposition()
    {    
        if( transform.childCount <= 0)
             return;

        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.position =  new Vector3(transform.position.x,
                child.position.y,
                transform.position.z
            );    
        }
    }
}
