// this script should be attached to the 'setBoardController' to control the player to set the board
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class PlayerSetBoard : MonoBehaviourPun
    {
        private System.DateTime utcDate;
        private GameObject ownerPlayer;
        private List<Texture2D> ImagesSelected;

        void Start()
        {
            ownerPlayer = PlayerManager.LocalPlayerInstance;
            ImagesSelected = new List<Texture2D>();
        }

        void OnEnable()
        {
            // subscribe the event
            EventManager_Board.OnClickedSet += Trigger;
            EventManager_Board.OnClickedSelectImage += OnSelectedImage;
        }

        void OnDisable()
        {
            EventManager_Board.OnClickedSet -= Trigger;
            EventManager_Board.OnClickedSelectImage -= OnSelectedImage;
        }

        // trigger is to trigger the set board
        void Trigger()
        {
            string line = InputFieldManager.InputFieldTransform.GetChild(1).GetComponent<Text>().text;
            List<Texture2D> Images = ImagesSelected;
            SetBoardPanelManager.setBoardPanelManager.HidePanel();
            placeBoard(line, Images);
        }

        void OnSelectedImage()
        {
            ImagesSelected = App_MediaService.instance.currentImages;
            SelectImageButtonManager.instance.HideButton(); // hide the add button, can be modified later for importing more images
            // show the images
            int size = ImagesSelected.Count;
            for (int i = 0; i < size; i++)
            {
                ImageShowManager.imagesTransform.GetChild(i).gameObject.SetActive(true);
                ImageShowManager.imagesTransform.GetChild(i).GetComponent<Image>().sprite =
                    Sprite.Create(ImagesSelected[i], new Rect(0, 0, ImagesSelected[i].width, ImagesSelected[i].height), new Vector2(0.5f, 0.5f));
            }
        }


        /// <summary>
        /// the function to set the board (share the moments)
        /// </summary>
        /// <param name="input">the text the player want to share</param>
        /// <param name="Images">the images the player want to share</param>
        private void placeBoard(string input, List<Texture2D>Images)
        {
            
            Quaternion rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
            Vector3 position = PlayerManager.LocalPlayerInstance.transform.position;
            utcDate = System.DateTime.UtcNow;

            object[] instanceData = new object[5];

            // instance[0]: the set time of the board
            int[] dateInfo = {utcDate.Year, utcDate.Month, utcDate.Day, utcDate.Hour, utcDate.Minute};
            instanceData[0] = dateInfo;

            // instance[1]: the ownerPlayer information
            object[] ownerInfo = new object[2];
            ownerInfo[0] = ownerPlayer.name;
            ownerInfo[1] = null; // the ownerInfo[1] should be the ownerPlayer's portrait
            //ownerInfo[1] = ownerPlayer.portrait;
            instanceData[1] = ownerInfo;

            // instance[2]: the words to be input
            instanceData[2] = input;

            // instance[3]: the images to be shared
            instanceData[3] = Images;

            // instance[4]: the future comments to be added
            instanceData[4] = "";

            // instantiate the board
            if (!(PhotonNetwork.IsMasterClient)) 
            {
                photonView.RPC("masterClientInstantiate", RpcTarget.MasterClient, position, rotation, instanceData);
            } else 
                PhotonNetwork.InstantiateRoomObject("Board1", position + rotation * (new Vector3(0f, 0f, 2f)), rotation, 0, instanceData);
        }

        [PunRPC]
        void masterClientInstantiate(Vector3 pos, Quaternion rot, object[] pass_data) // set the scene object by the master client
        {
            PhotonNetwork.InstantiateRoomObject("Board1", pos + rot * (new Vector3(0f, 0f, 2f)), rot, 0, pass_data);
        }
        
    }
}