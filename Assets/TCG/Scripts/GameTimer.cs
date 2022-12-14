using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Mirror;

public delegate void OnTimerComplete();

public class GameTimer : MonoBehaviour
{
    public float currentTime;

    private bool _isTimerPaused = true;
    private bool _isTimerComplete = true;
    public bool _selfDestroy = true;
    private bool _repeater = true;

    public OnTimerComplete _onComplete;
    public float _completionTime;

    public bool startOnAwake;

    private void Awake()
    {
        if(startOnAwake)
        {
            _isTimerPaused = false;
        }
    }

    public void StartTimer(float completionTime, OnTimerComplete onComplete, bool selfDestroy = false)
    {
        _selfDestroy = selfDestroy;
        _completionTime = completionTime;
        _onComplete = onComplete;
        _isTimerPaused = false;
        _isTimerComplete = false;
    }

    public void PauseTimer()
    {
        _isTimerPaused = true;
    }
    
    private void Update()
    {
        if(!_isTimerPaused && !_isTimerComplete)
        {
            currentTime += Time.deltaTime;
            if(currentTime >= _completionTime)
            {
                if(!_repeater)
                {
                    _isTimerComplete = true;
                }else{
                    currentTime = 0;
                }
                _onComplete();
                if(_selfDestroy)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void ResetTimer()
    {
        _isTimerPaused = false;
        currentTime = 0;
    }
}