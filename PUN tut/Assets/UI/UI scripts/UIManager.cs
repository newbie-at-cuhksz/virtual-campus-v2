using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region
    public GameObject ChatPanel;
    public GameObject StopPanel;
    public GameObject FriendPanel;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ChatPanel.SetActive(false);
        StopPanel.SetActive(false);
        FriendPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
