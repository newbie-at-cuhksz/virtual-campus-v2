using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChatPanelUiControl : MonoBehaviour
{
    #region UI control

    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        this.gameObject.SetActive(false);
    }

    #endregion
}
