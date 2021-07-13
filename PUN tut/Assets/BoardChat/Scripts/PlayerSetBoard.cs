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
            placeBoard(line, Images);
            SetBoardPanelManager.setBoardPanelManager.HidePanel();
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
            Debug.Log("the plaxeBoard() is called");
            Quaternion rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
            Vector3 position = PlayerManager.LocalPlayerInstance.transform.position;
            utcDate = System.DateTime.UtcNow;

            object[] instanceData = new object[5];

            // instance[0]: the set time of the board
            int[] dateInfo = {utcDate.Year, utcDate.Month, utcDate.Day, utcDate.Hour, utcDate.Minute};
            instanceData[0] = dateInfo;
            Debug.Log("add date");

            // instance[1]: the ownerPlayer information
            object[] ownerInfo = new object[2];
            ownerInfo[0] = PlayerManager.LocalPlayerInstance.GetPhotonView().ViewID;
            ownerInfo[1] = null; // the ownerInfo[1] should be the ownerPlayer's portrait
            //ownerInfo[1] = ownerPlayer.portrait;
            instanceData[1] = ownerInfo;
            Debug.Log("add user");

            // instance[2]: the words to be input
            instanceData[2] = input;
            Debug.Log("add input line");

            // instance[3]: the images to be shared, note that the index 0 is the size of the image
            object[] imagesData = new object[7];
            imagesData[0] = Images.Count;
            for (int i = 0; i < Images.Count; i++)
            {
                imagesData[i + 1] = duplicateTexture(Images[i]).EncodeToPNG();
            }
            for (int j = Images.Count; j < 7; j++)
            {
                imagesData[j] = null; // fill the rest space as null
            }
            instanceData[3] = imagesData;

            // instance[4]: the future comments to be added
            instanceData[4] = "";
            Debug.Log("add comments");

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

        // the following function is to make the image readable, reference website: https://stackoverflow.com/questions/44733841/how-to-make-texture2d-readable-via-script
        Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
    }
}