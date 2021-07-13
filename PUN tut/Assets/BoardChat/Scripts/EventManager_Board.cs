// the script should be attached to the "Board Events"
// the script creates events which are associated with the board
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager_Board : MonoBehaviour
{
    public static EventManager_Board instance;
    public delegate void SetBoardAction();
    public delegate void ViewBoardAction();
    public static event SetBoardAction OnClickedSet;
    public static event SetBoardAction OnClickedSelectImage;
    public static event ViewBoardAction OnCommentBoard;
    public static event ViewBoardAction OnClickBoard;

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

    public void PressComment()
    {
        if (OnCommentBoard != null)
        {
            OnCommentBoard();
        }
    }

    public void ClickBoard()
    {
        if (OnClickBoard != null)
        {
            OnClickBoard();
            Debug.Log("event onclick board is called");
        }
    }
}
