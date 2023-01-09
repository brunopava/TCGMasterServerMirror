using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Mirror;

public delegate void PlayerAction(int actionID);

public class ActionChain : NetworkBehaviour
{
    public int actionCounter = 0;

    [SerializeField]
    public Dictionary<int, PlayerAction> chain = new Dictionary<int, PlayerAction>();
    public Dictionary<int, string> labels = new Dictionary<int, string>();

    private GameTimer inactivityTimer;
    private GameTimer maxGameInactivityTimer;

    public bool isChainBusy = false;
    public bool isChainPaused = false;

    public bool isWaiting;
    public int waitingID = 0;

    private void Awake()
    {
        GameObject inactivity = new GameObject("[Timer] - Inactivity (pass turn)");
        inactivityTimer = inactivity.AddComponent<GameTimer>();

        GameObject maxInactivity = new GameObject("[Timer] - Max Inactivity (close room)");
        maxGameInactivityTimer = maxInactivity.AddComponent<GameTimer>();
    }

    public void GameStarted(OnTimerComplete onPlayerInactive)
    {
        if(inactivityTimer != null)
        {
            inactivityTimer.StartTimer(
                TCGConstants.MAX_TURN_TIME, 
                ()=>{
                    print("Auto turn pass");
                    onPlayerInactive();
                }
            );
        }
    }

    public void ResetTurnInactivity()
    {
        if(inactivityTimer != null)
        {
            inactivityTimer.ResetTimer();
        }
    }

    public void SetMaxInactivity(OnTimerComplete onPlayerInactive)
    {
        if(maxGameInactivityTimer != null)
        {
            maxGameInactivityTimer.StartTimer(
                TCGConstants.MAX_INACTIVE_TIME, 
                ()=>{
                    onPlayerInactive();
                }
            );
        }
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            foreach(KeyValuePair<int, string> current in labels)
            {
                print(current.Key.ToString()+"_"+current.Value);
            }

            if(labels.ContainsKey(waitingID))
                print(waitingID.ToString() +"_"+ labels[waitingID]);
        }
    }

    public void Pause()
    {
        isChainPaused = true;
    }

    public void Play()
    {
        isChainPaused = false;
        isChainBusy = false;

        Debug.Log("-------");
        Debug.Log("RESUME CHAIN");
        Debug.Log(isChainBusy);
        Debug.Log(isChainPaused);
        Debug.Log("-------");

        ResolveChain();
    }

    public void ResetChain()
    {
        actionCounter = 0;
        chain = new Dictionary<int, PlayerAction>();
    }

    public int AddToChain(string actionLabel, PlayerAction action)
    {
        // Debug.Log("-----------------------------------------------");
        actionCounter++;

        chain.Add(actionCounter, action);
        labels.Add(actionCounter, actionLabel);

        return actionCounter;
    }

    public void ResolveChain()
    {
        if(!isChainBusy && !isChainPaused)
        {
            if(chain.Count > 0)
            {
                KeyValuePair<int, PlayerAction> current = chain.First();

                //ACTIVATE ACTION - > PASS ID
                current.Value(current.Key);

                inactivityTimer.ResetTimer();
                maxGameInactivityTimer.ResetTimer();

                waitingID = current.Key;
                isChainBusy = true;
            }
        }
    }

    public void CompleteAction(int actionID)
    {
        if(chain.ContainsKey(actionID))
        {
            // Debug.Log("CompleteAction: "+labels[actionID]);
            // Debug.Log("-----------------------------------------------");
            labels.Remove(actionID);
            chain.Remove(actionID);
        }
        
        isChainBusy = false;

        if(chain.Count > 0)
        {
            ResolveChain();
        }
    }
}