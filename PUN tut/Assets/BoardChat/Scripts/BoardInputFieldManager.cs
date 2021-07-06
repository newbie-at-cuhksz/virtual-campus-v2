// this script should be attached to the 'Board Inputfield' in under the canvas of the Room1 scence
// to control the board content input, need to be improved
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
