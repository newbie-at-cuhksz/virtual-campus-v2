// the script should be attached to the "Board Events"
// the script creates events which are associated with the board
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager_Board : MonoBehaviour
{
    public static EventManager_Board instance;
    public delegate void SetBoardAction();
    public static event SetBoardAction OnClickedSet;
    public static event SetBoardAction OnClickedSelectImage;

    private void Start()
    {
        instance = this;
    }

    public void PressSet()
    {
        if (OnClickedSet != null)
        {
            OnClickedSet();
        }
    }

    public void PressedSelectImage()
    {
        if (OnClickedSelectImage != null)
        {
            OnClickedSelectImage();
        }
    }
}
