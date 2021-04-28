using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardInputFieldManager : MonoBehaviour
{
    public InputField inputField;

    #region Public Methods
    public void promptBoardInput()
    {
        
        this.gameObject.SetActive(true);
    }

    public void endBoardInput()
    {
        inputField.Select();
        inputField.text = "";
        this.gameObject.SetActive(false);
    }
    #endregion
}
