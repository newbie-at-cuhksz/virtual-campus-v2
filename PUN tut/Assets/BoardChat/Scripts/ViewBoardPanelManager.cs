using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class ViewBoardPanelManager : MonoBehaviourPun
    {
        private Transform panelTransform;

        private void Start()
        {
            panelTransform = this.transform.GetChild(0).transform;
        }

        // triggered by the quit button
        public void QuitView()
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }

        void OnEnable()
        {
            EventManager_Board.OnClickBoard += ShowBoard;
            EventManager_Board.OnCommentBoard += Comment;
        }

        void OnDisable()
        {
            EventManager_Board.OnClickBoard -= ShowBoard;
            EventManager_Board.OnCommentBoard -= Comment;
        }

        void ShowBoard()
        {
            Debug.Log("the board content should be shown");
            this.transform.GetChild(0).gameObject.SetActive(true);
            PhotonView PV = InfoPanelOnClick_Board.BoardPhotonView;
            object[] data = PV.InstantiationData;
            DisplayShareText((string)data[2]);
            DisplayImages((object[])data[3]);
            DisplayComments((string)data[4]);
        }

        void Comment()
        { 
            
        }

        private void DisplayShareText(string shareText)
        {
            Debug.Log("the share text should be shown");
            panelTransform.GetChild(0).GetComponent<Text>().text = shareText;
        }

        private void DisplayImages(object[] imagesPNG)
        {
            Debug.Log("the images should be shown");
            int size = (int)imagesPNG[0];
            Debug.Log("size can be cast, the size is: " + size);
            for (int i = 0; i < size; i++)
            {
                Texture2D image = new Texture2D(2, 2); // the size of the texure can be further fixed
                image.LoadImage((byte[])imagesPNG[i+1]);
                panelTransform.GetChild(i+1).gameObject.SetActive(true);
                panelTransform.GetChild(i+1).gameObject.GetComponent<Image>().sprite =
                    Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            }
        }

        private void DisplayUserInfo(string userName, byte[] portraitPNG)
        {
            Debug.Log("the user info should be shown");
            panelTransform.GetChild(8).transform.GetChild(0).GetComponent<Text>().text = userName;
            // the portrait will be set later
        }

        private void DisplayComments(string Comments)
        {
            Debug.Log("the comments should be shown");
            panelTransform.GetChild(7).GetComponent<Text>().text = Comments;
        }

    }
}
