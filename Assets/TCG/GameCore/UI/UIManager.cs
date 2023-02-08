using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public GameObject baseLayer;
    public GameObject popupLayer;
    public GameObject tooltipsLayer;

    public Button endTurnButton;

    public TextMeshProUGUI debugTurn;

    public void OnGameStarted()
    {
        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(
            ()=>{
                SetEndTurnButtonEnable(false);
                TCGGameManager.Instance.CMDEndTurn(NetworkClient.localPlayer.netId);
            }
        );
    }

    public void SetEndTurnButtonEnable(bool isEnable)
    {
        endTurnButton.interactable = isEnable;
    }
    
    public void SetDebugText(string text)
    {
        debugTurn.text = text;
    }
}
