using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class BoardOwnershipTransfer : MonoBehaviourPun
    {
        public void onSetBoard()
        {
            base.photonView.RequestOwnership();
        }
    }
}
