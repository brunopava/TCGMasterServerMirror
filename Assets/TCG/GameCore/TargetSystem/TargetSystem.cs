using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using com.yah.LineRendererDemo;

public delegate void TargetSuccess (ITarget source, List<ITarget> targets);
public delegate void TargetCancel (string json);
public delegate void OnTargetBegin();

public class TargetSystem : Singleton<TargetSystem>
{
    [Header("Line Animation params")]
    [SerializeField] private LineRenderer castLine = null; // Actual line renderer
    [Space(10)]
    [SerializeField] private float inActiveLength = 0.25f;
    [SerializeField] private float inActiveWidth = 0.01f;
    [SerializeField] private float activeWidth = 0.1f;
    [Space(10)]
    //These are for the Bezier curve smooth follow
    [SerializeField] private float curveActiveFollowSpeed = 20;
    [SerializeField] private float curveInActiveFollowSpeed = 100;
    [SerializeField] private float curveHitPointOffset = 0.25f;
    [SerializeField] private Transform[] curvePoints = null;
    [SerializeField] private int numberOfPointsOnCurve = 25;
    private Bezier curveGenerator = null;
    private Vector3 curvePointPosition = Vector3.zero;

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

        onBeginTargetCallbacks = new List<OnTargetBegin>();

        curveGenerator = new Bezier(numberOfPointsOnCurve);
        castLine.positionCount = numberOfPointsOnCurve;

        castLine.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(_isTargeting)
        {
            DrawLine();

            if(Input.GetMouseButton(1))
            {
                CancelTargeting(true);
            }
        }
    }

    public float offsetX = 1f;

    public float offsetZ = 1f;

    private void DrawLine()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float x = curvePoints[0].position.x + (mousePos.x - curvePoints[0].position.x) / 2;
        float y = 5f;
        float z = curvePoints[0].position.z + (mousePos.z - curvePoints[0].position.z) / 2;

        curvePointPosition = new Vector3(x + (mousePos.x*offsetX), y, z+offsetZ);
        // curvePointPosition = new Vector3(mousePos.x, mousePos.y, mousePos.z) + (mousePos.normalized * curveHitPointOffset);
        curvePoints[1].position = Vector3.Lerp(curvePoints[1].position, curvePointPosition, curveActiveFollowSpeed * Time.deltaTime);

        //set endpoint
        curvePoints[2].position = Vector3.Lerp(curvePoints[2].position, new Vector3(mousePos.x, 0.1f, mousePos.z), curveActiveFollowSpeed * Time.deltaTime);

        Vector3[] newPositions = curveGenerator.GetQuadraticCurvePoints(curvePoints[0].position, curvePoints[1].position, curvePoints[2].position);
        castLine.SetPositions(newPositions);
        castLine.startWidth = activeWidth;
        castLine.endWidth = activeWidth;
    }

    private Vector3 _startPosition;

    public void BeginTargeting(ITarget source, TargetSuccess successCallback=null, TargetCancel cancelCallback=null, int targetAmount=1)
    {

        if(!_isTargeting)
        {

            _targetAmount = targetAmount;
            _source = (source as MonoBehaviour).transform;
            _sourceTarget = source;
            _isTargeting = true;
            onSuccessCallback = successCallback;
            onCancelCallback = cancelCallback;

            Debug.Log("BeginTargeting From: "+_source.name);

            curvePoints[0].localPosition = _source.position;

            castLine.gameObject.SetActive(true);

            foreach (OnTargetBegin callback in onBeginTargetCallbacks)
            {
                callback();
            }
        }
    }

    public void CancelTargeting(bool sendCallback=false)
    {
        Debug.Log("CancelTargeting");
        isAttack = false;
        _isOponentOnly = true;
        _isAllyOnly = false;

        _isTargeting = false;
        _source = null;
        _targets = new List<ITarget>();
        _sourceTarget = null;

        castLine.gameObject.SetActive(false);

        curvePoints[1].position = Vector3.zero;
        curvePoints[2].position = curvePoints[0].localPosition;

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
            // Debug.Log("CANCEL1");
            //HIS CARD
            CancelTargeting(true);
            return;
        }

        if(_isOponentOnly && card.hasAuthority && !_isAllyOnly)
        {
            // Debug.Log("CANCEL2");
            //MY CARD
            CancelTargeting(true);
            return;
        }

        if(cardBehaviour != null)
        {
            if(!cardBehaviour.isOnField)
            {
                // Debug.Log("CANCEL3");
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
