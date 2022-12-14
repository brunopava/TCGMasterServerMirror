using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CardBehaviour : CardBase, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool _isDragging = false;
    private Transform _dropArea;

    public bool isDraggable = true;
    public bool isInteractable = false;

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        // Debug.Log("Cursor Entering " + name + " GameObject");
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        // Debug.Log("Cursor Exiting " + name + " GameObject");
    }

    private void OnTriggerEnter(Collider other) 
    {
        // Debug.Log("OnTriggerEnter2D: "+other.name);
        _dropArea = other.transform;
    }

    private void OnTriggerExit(Collider other) 
    {
        // Debug.Log("OnTriggerExit2D: "+other.name);
        _dropArea = null;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if(_isDragging || !isInteractable)
            return;
    }

    public void OnDrag(PointerEventData data)
    {
        if (!isDraggable || !isInteractable)
            return;
        
        if (_isDragging)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localPosition = new Vector3(mousePosition.x, 0.1f , mousePosition.z);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInteractable || !isDraggable)
        {
            return;
        }

        _isDragging = true;
        transform.SetParent(null);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable || !_isDragging || !isInteractable)
            return;

        //DRAGGED CARD OVER THE FIELD
        if (_dropArea != null)
        {
            bool canCast = true;

            if (canCast)
            {
                Cast();
            }
            else{
                //NO RESOURCES
                ReturnToHand();
                return;
            }
        }
        else
        {
            //DROPPED CARD OUTSIDE THE BOUNDS OF THE FIELD
            ReturnToHand();
        }
    }

    public void Cast()
    {
        isDraggable = false;
        transform.SetParent(TCGArena.Instance.playerField);
    }

    public void ReturnToHand()
    {
        //TODO:
        //ANIMATE
        transform.SetParent(TCGArena.Instance.playerHand);
    }
}
