using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastExamples : MonoBehaviour
{
    public void Update()
    {
        FromCameraMousePos();
    }

    private void FromCameraMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask layerMask = LayerMask.GetMask("Arena");
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            Debug.Log("Did Hit: "+hit.transform.name);
        }
    }
}
