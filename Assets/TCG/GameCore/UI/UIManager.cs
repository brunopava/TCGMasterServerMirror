using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using DevionGames.UIWidgets;
using ContextMenu = DevionGames.UIWidgets.ContextMenu;

public class UIManager : Singleton<UIManager>
{
    public GameObject baseLayer;
    public GameObject popupLayer;
    public GameObject tooltipsLayer;

    public Button endTurnButton;

    public Text debugTurn;

    public Sprite[] menuIcons;
    public string[] menu;

    private RadialMenu m_RadialMenu;
    private ContextMenu m_ContextMenu;

    public string title;
    [TextArea]
    public string text;
    public Sprite icon;
    public string[] options;

    private DialogBox m_DialogBox;

    //Reference to the MessageContainer in scene
    private Notification m_Notification;
    //Options to display containing information about text, icon, fading duration...
    public NotificationOptions[] notificationOptions;


    private void Start()
    {
        this.m_RadialMenu = WidgetUtility.Find<RadialMenu>("RadialMenu");
        this.m_ContextMenu = WidgetUtility.Find<ContextMenu>("ContextMenu");
        this.m_DialogBox = FindObjectOfType<DialogBox>();
        this.m_Notification = WidgetUtility.Find<Notification> ("Notification");
    }

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

    public void ShowRadialMenu(GameObject target)
    {
        this.m_RadialMenu.Show(gameObject, menuIcons, delegate (int index) { Debug.Log("Used index - " + index); }); 
    }

    public void OpenContextMenu()
    {
        this.m_ContextMenu.Clear();
        for (int i = 0; i < menu.Length; i++)
        {
            string menuItem = menu[i];
            m_ContextMenu.AddMenuItem(menuItem, delegate { Debug.Log("Used - " + menuItem); });
        }
        this.m_ContextMenu.Show();
    }

    public void ShowDialogBox() {
        m_DialogBox.Show(title, text, icon, null, options);
    }

    public void ShowWithCallback()
    {
        m_DialogBox.Show(title, text, icon, OnDialogResult, options);
    }

    private void OnDialogResult(int index)
    {
        m_DialogBox.Show("Result", "Callback Result: "+options[index], icon, null, "OK");
    }

    /// <summary>
    /// Called from a button OnClick event in the example
    /// </summary>
    public void AddRandomNotification(){
        //Get a random MessageOption from the array
        NotificationOptions option=notificationOptions[Random.Range(0,notificationOptions.Length)];
        //Add the message
        m_Notification.AddItem(option);
    }

    /// <summary>
    /// Called from a button OnClick event in the example
    /// </summary>
    public void AddNotification(InputField input){
        //Add a text message

        m_Notification.AddItem (input.text);
    }

    /// <summary>
    /// Called from a Slider OnValueChanged event in the example
    /// </summary>
    public void AddNotification(float index){
        //Round the index to int and get the option from options array.
        NotificationOptions option = notificationOptions [Mathf.RoundToInt (index)];
        //Add the message
        m_Notification.AddItem (option);
    }
}
