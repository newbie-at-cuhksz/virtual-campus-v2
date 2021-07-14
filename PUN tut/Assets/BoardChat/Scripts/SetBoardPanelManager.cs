// this script should be attached to the 'Board Inputfield' in under the canvas of the Room1 scence
// to control the board content input, need to be improved
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBoardPanelManager : MonoBehaviour
{
    public static SetBoardPanelManager setBoardPanelManager;
    private static InputField shareInput;

    private void Start()
    {
        setBoardPanelManager = this; // create a singleton to reference the panel
        shareInput = this.transform.GetChild(0).gameObject.GetComponent<InputField>();
    }

    public void ActivatePanel()
    {
        
        this.gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        // clear the text of the input field, input field is the child with index 0
        shareInput.Select();
        shareInput.text = "";
        // clear the images selected

        this.gameObject.SetActive(false);
    }
}
