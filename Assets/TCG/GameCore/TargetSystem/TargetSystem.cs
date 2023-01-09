using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public delegate void TargetSuccess (ITarget source, List<ITarget> targets);
public delegate void TargetCancel (string json);
public delegate void OnTargetBegin();

public class TargetSystem : Singleton<TargetSystem>
{
    public Image arrow;
    private RectTransform _arrowRect;

    private List<ITarget> _targets;
    private Transform _source;
    private ITarget _sourceTarget;

    private TargetSuccess onSuccessCallback;
    private TargetCancel onCancelCallback;
    public List<OnTargetBegin> onBeginTargetCallbacks;

    private int _targetAmount;

    private bool _isAllyOnly;
    private bool _isOponentOnly;

    public bool isOponentOnly{
        set{
            _isOponentOnly = value;
            if(value)
            {
                _isAllyOnly = false;
            }
        }
        get{
            return _isOponentOnly;
        }
    }
    public bool isAllyOnly{
        set{
            _isAllyOnly = value;
            if(value)
            {
                _isOponentOnly = false;
            }
        }
        get {
            return _isAllyOnly;
        }
    }
    public bool isAttack = false;

    private bool _isTargeting;
    public bool isTargeting
    {
        get {
            return _isTargeting;
        }
    }

    public bool sourceHasDirectHit{
        get {
            CardBehaviour card = (_sourceTarget as MonoBehaviour).GetComponent<CardBehaviour>();
            // return card.direct_hit;
            //TODO:
            return true;
        }
    }
    public CardBehaviour sourceCard{
        get{
            CardBehaviour card = (_sourceTarget as MonoBehaviour).GetComponent<CardBehaviour>();
            return card;
        }
    }

    private void Awake()
    {
        _targets = new List<ITarget>();
        _arrowRect = arrow.GetComponent<RectTransform>();
        arrow.gameObject.SetActive(false);
        onBeginTargetCallbacks = new List<OnTargetBegin>();
    }

    bool foundTarget = false;

    private void Update()
    {
        if(_isTargeting)
        {
            Vector3 mousePos = Input.mousePosition;

            _startPosition = Camera.main.WorldToScreenPoint(_source.position);
            _arrowRect.position = _startPosition;

            float distance = Vector3.Distance(_arrowRect.position, new Vector3(mousePos.x, mousePos.y, 0));
            _arrowRect.sizeDelta = new Vector2(_arrowRect.sizeDelta.x, distance-50f);

            float endPosX = mousePos.x - _startPosition.x;
            float endPosY = mousePos.y - _startPosition.y;

            float angle = (Mathf.Atan2(endPosX, endPosY) * Mathf.Rad2Deg) * -1;
            _arrowRect.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if(Input.GetMouseButton(1))
            {
                CancelTargeting(true);
            }
        }
    }

    private Vector3 _startPosition;

    public void BeginTargeting(ITarget source, TargetSuccess successCallback=null, TargetCancel cancelCallback=null, int targetAmount=1)
    {
        if(!_isTargeting)
        {

            _targetAmount = targetAmount;
            _source = (source as MonoBehaviour).transform;
            _sourceTarget = source;
            arrow.gameObject.SetActive(true);
            _isTargeting = true;
            onSuccessCallback = successCallback;
            onCancelCallback = cancelCallback;
            
            // _isTargeting = true;

            // Debug.Log(_source);
            // Debug.Log(_source.position);
            // Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            // Debug.Log(Camera.main.ScreenToWorldPoint(_source.position));

            // Debug.Log(source);
            // Debug.Log(_sourceTarget);

            foreach (OnTargetBegin callback in onBeginTargetCallbacks)
            {
                callback();
            }
        }
    }

    public void CancelTargeting(bool sendCallback=false)
    {
        isAttack = false;
        _isOponentOnly = true;
        _isAllyOnly = false;

        arrow.gameObject.SetActive(false);
        _isTargeting = false;
        _source = null;
        _targets = new List<ITarget>();
        _sourceTarget = null;

        // Debug.Log("CANCEL");

        if(sendCallback && onCancelCallback != null)
            onCancelCallback("cancel-targeting");

        ClearCallbacks();
    }

    private void ClearCallbacks()
    {
        onSuccessCallback = null;
        onCancelCallback = null;
    }

    public void SelectTarget(ITarget target)
    {
        NetworkIdentity card = (target as MonoBehaviour).GetComponent<NetworkIdentity>();
        CardBehaviour cardBehaviour = card.GetComponent<CardBehaviour>();

        if(_isAllyOnly && !card.hasAuthority && !_isOponentOnly)
        {
            Debug.Log("CANCEL1");
            //HIS CARD
            CancelTargeting(true);
            return;
        }

        if(_isOponentOnly && card.hasAuthority && !_isAllyOnly)
        {
            Debug.Log("CANCEL2");
            //MY CARD
            CancelTargeting(true);
            return;
        }

        if(cardBehaviour != null)
        {
            if(!cardBehaviour.isOnField)
            {
                Debug.Log("CANCEL3");
                CancelTargeting(true);
                return;
            }
        }

        _targets.Add(target);

        if(_targets.Count == _targetAmount)
        {
            if(onSuccessCallback != null)
            {
                onSuccessCallback(_sourceTarget, _targets);
            }
            CancelTargeting();
        } 
    }
}
