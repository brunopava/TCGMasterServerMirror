using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Mirror;
using DevionGames.UIWidgets;

public class CardBehaviour : CardBase, ITarget, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public CardDisplay display;

    private bool _isDragging = false;
    private Transform _dropArea;
    
    public bool isDraggable = true;
    public bool isInteractable = false;

    //===========
    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;
    //===========

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // if(isOnField)
        // {
        //     TooltipTrigger trigger = gameObject.GetComponent<TooltipTrigger>();
        //     Debug.Log(trigger);
        //     if(trigger != null)
        //     {
        //         trigger.enabled = true;
        //     }
        // }
        //Output to console the GameObject's name and the following message
        // Debug.Log("Cursor Entering " + name + " GameObject");
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        // if(isOnField)
        // {
        //     TooltipTrigger trigger = gameObject.GetComponent<TooltipTrigger>();
        //     Debug.Log(trigger);
        //     if(trigger != null)
        //     {
        //         trigger.enabled = false;
        //     }
        // }
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
        if(transform.parent == TCGArena.Instance.playerHand && TargetSystem.Instance.isTargeting)
        {
            TargetSystem.Instance.CancelTargeting(true);
        }
        
        List<CardBehaviour> tanks = new List<CardBehaviour>();

        for(int i  = 0; i < TCGArena.Instance.oponentField.transform.childCount; i++)
        {
            CardBehaviour card = TCGArena.Instance.oponentField.transform.GetChild(i).GetComponent<CardBehaviour>();

            if(card.tank)
            {
                tanks.Add(card);
            }
        }

        //ATTACK
        if(TargetSystem.Instance.isTargeting && TargetSystem.Instance.isAttack)
        {
            //No tanks in field or attack a tank
            if(tanks.Count == 0 || tanks.Contains(this))
            {
                TargetSystem.Instance.SelectTarget(this);
                isAttackEnabled = false;
            }
        }else if(TargetSystem.Instance.isTargeting && !TargetSystem.Instance.isOponentOnly && !TargetSystem.Instance.isAttack)
        {
            //CAST FRIENDLY
            TargetSystem.Instance.SelectTarget(this);
        }else if(TargetSystem.Instance.isTargeting && TargetSystem.Instance.isOponentOnly && !TargetSystem.Instance.isAttack)
        {
            //CAST ENEMY
            TargetSystem.Instance.SelectTarget(this);
        }

        if (isOnField)
        {
            OnDoubleClick();
        }

        if (pointerEventData.button == PointerEventData.InputButton.Right && !isOnField)
        {
            UIManager.Instance.OpenContextMenu();
        }
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
        if (!isInteractable || !TCGGameManager.Instance.gameClient.IsMyTurn())
        {
            return;
        }

        if(!isDraggable && isAttackEnabled && isOnField)
        {
            TargetSystem.Instance.isAttack = true;
            TargetSystem.Instance.isOponentOnly = true;
            TargetSystem.Instance.BeginTargeting(this, AttackComplete);
            return;
        }

        if(!isOnField && !isDead)
        {
            _isDragging = true;
            transform.SetParent(null); 
        }
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

    private void AttackComplete(ITarget source, List<ITarget> targets)
    {
        CardBehaviour card = (source as MonoBehaviour).GetComponent<CardBehaviour>();
        
        if(card != null)
        {
            uint sourceID = card.netId;
            uint targetID = 666; //c# forces declaration of a value
            if(targets.Count == 1)
            {
                CardBehaviour target = (targets[0] as MonoBehaviour).GetComponent<CardBehaviour>();
                if(target != null)
                {
                    targetID = target.netId;
                }else{
                    // PlayerLifeHitbox playerLife = (targets[0] as MonoBehaviour).GetComponent<PlayerLifeHitbox>();
                    // if(playerLife != null)
                    // {
                    //     targetID = playerLife.netId;
                    // }
                }
            }

            TCGGameManager.Instance.CMDCombat(NetworkClient.connection.identity.netId, sourceID, targetID);
        }
    }

    public void Cast()
    {
        isDraggable = false;
        transform.SetParent(TCGArena.Instance.playerField);

        TCGGameManager.Instance.CMDCastCard(NetworkClient.localPlayer.netId, netId);
    }

    public void ReturnToHand()
    {
        //TODO:
        //ANIMATE
        transform.SetParent(TCGArena.Instance.playerHand);
    }

    public void OnDoubleClick()
    {
        clicked++;
        if (clicked == 1) clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicked = 0;
            clicktime = 0;
            //TODO: chamar popup de detalhes
            // OpenCardInfo();
            UIManager.Instance.ShowRadialMenu(gameObject);
        }
        else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
    }

    public void UpdateData()
    {
        display.UpdateData(this);
    }
}
