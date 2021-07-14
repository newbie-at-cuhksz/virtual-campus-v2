using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Panels {ChatPanel, StopPanel,FriendPanel, InventoryPanel,StorePanel };
public class UIManager : MonoBehaviour
{
    #region
    public GameObject ChatPanel;
    public GameObject StopPanel;
    public GameObject FriendPanel;
    public GameObject InventoryPanel;
    public GameObject StorePanel;
    public GameObject AuctionPanel;
    public GameObject GoodsPanel;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ChatPanel.SetActive(false);
        StopPanel.SetActive(false);
        FriendPanel.SetActive(false);
        InventoryPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TurnOnPanel(int panelName)
    {
        
        switch (panelName)
        {   
            case (int)Panels.ChatPanel:
                ChatPanel.SetActive(true);
                break;

            case (int)Panels.FriendPanel:
                FriendPanel.SetActive(true);
                break;

            case (int)Panels.InventoryPanel:
                InventoryPanel.SetActive(true);
                break;

            case (int)Panels.StopPanel:
                StopPanel.SetActive(true);
                break;

            case (int)Panels.StorePanel:
                if (StorePanel.activeSelf==true) StorePanel.SetActive(false);
                else StorePanel.SetActive(true);
                break;


        }     
    }

    public void OnDropDownChanged(int value)
    {
        switch (value)
        {
            case 0:
                GoodsPanel.SetActive(false);
                AuctionPanel.SetActive(true);
                break;


            case 1:
                GoodsPanel.SetActive(true);
                AuctionPanel.SetActive(false);
                break; 
        }
    }


}
