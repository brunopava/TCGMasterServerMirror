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
                TCGGameManager.Instance.CMDEndTurn(NetworkClient.localPlayer.netId);
            }
        );
    }
}
