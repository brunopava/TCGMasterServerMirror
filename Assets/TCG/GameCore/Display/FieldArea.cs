using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldArea : MonoBehaviour
{
    public float spacing = 1f;
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

        Transform firstChild = transform.GetChild(0);
        Vector3 childSize = firstChild.GetComponent<MeshRenderer>().bounds.size;

        float totalSize = ((transform.childCount+1)  * childSize.x) + (spacing*transform.childCount);

        Vector3 cornerL = -new Vector3((totalSize/2), 0, 0);
        Vector3 cornerR = -new Vector3(-(totalSize/2), 0, 0);

        Vector3 dir = cornerR - cornerL;

        float dist = dir.magnitude;

        dir.Normalize();

        float step = dist / (transform.childCount + 1);
        
        for(int i = 0; i < transform.childCount; i++)
        {
            Vector3 position = cornerL + dir * step * (i + 1);

            Transform child = transform.GetChild(i);
            child.position =  new Vector3(position.x,
                child.position.y,
                transform.position.z
            );

            child.rotation = Quaternion.identity;
        }      
    }
}
