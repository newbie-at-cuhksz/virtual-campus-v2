// this script should be attached to the board prefab to control the behavior of the board
// the behavior of the board: 1. destroy itself when it has been placed for 1 day
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class BoardManager : MonoBehaviourPun
    {
        private System.DateTime utcDate;
        private int[] initDate;

        private void Start()
        {
            PhotonView PV = transform.GetComponent<PhotonView>(); // get the photon view of the board
            initDate = (int[])PV.InstantiationData[1];
            StartCoroutine(updateOnFiveMinutes(300)); // update every 5 minutes
        }

        IEnumerator updateOnFiveMinutes(float waitTime)
        {
            while (true)
            {
                yield return new WaitForSeconds(waitTime);
                UpdateBoard(); // check if the board needs to be destroyed.
            }
        }

        private void UpdateBoard() // check if the board has reached the time limit
        {
            utcDate = System.DateTime.Now; // get the time now
            int[] nowDate = { utcDate.Year, utcDate.Month, utcDate.Day, utcDate.Hour, utcDate.Minute };

            if (CheckTerminate(initDate, nowDate))
            {
                // destroy the board
                photonView.RPC("TerminateBoard", RpcTarget.MasterClient);
            } // else preserve the board in the scene
        }

        /// <summary>
        /// the function is to check if the interval between 2 time is greater than one day
        /// </summary>
        /// <param name="initDate">the before time</param>
        /// <param name="nowDate">the after time</param>
        /// <returns></returns>
        private bool CheckTerminate(int[] initDate, int[] nowDate)
        {
            if (nowDate[0] == initDate[0] && nowDate[1] == initDate[1] && nowDate[2] == initDate[2]) // when on the same day
            {
                if (nowDate[3] * 60 + nowDate[4] >= initDate[3] * 60 + nowDate[4]) return false; // do not destroy
                else return true;
            }
            else if (nowDate[2] > initDate[2]) //day different
            {
                if (((24 - initDate[3] + nowDate[3]) < 24 || ((initDate[3] == nowDate[3]) && nowDate[4] < initDate[4]))) return false; // less than one day
                else return true;
            }
            else if (nowDate[1] > initDate[1]) // month different
            {
                if (nowDate[1] - initDate[1] == 1)
                {
                    if (nowDate[1] == 1)
                    {
                        if (initDate[1] == 1 || initDate[1] == 3 || initDate[1] == 5 || initDate[1] == 7 || initDate[1] == 8 || initDate[1] == 10 || initDate[1] == 12)
                        {
                            if (((24 - initDate[3] + nowDate[3]) < 24 || (((24 - initDate[3] + nowDate[3]) == 24) && nowDate[4] < initDate[4]))) return false;
                            else return true;
                        }
                        else
                        {
                            if (((24 - initDate[3] + nowDate[3]) < 24 || (((24 - initDate[3] + nowDate[3]) == 24) && nowDate[4] < initDate[4]))) return false;
                            else return true;
                        }
                    }
                    else return true;
                }
                else if (nowDate[0] > initDate[0]) // year different
            {
                if (initDate[1] == 12 && nowDate[1] == 1 && initDate[2] == 31 && nowDate[2] == 1 &&
                    ((24 - initDate[3] + nowDate[3]) < 24 || (((24 - initDate[3] + nowDate[3]) == 24) && nowDate[4] < initDate[4])))
                    return false; // do not destroy
                else return true;
            }
                else return false;
            }
            else return true;
        }

        [PunRPC]
        void TerminateBoard()
        {
            PhotonNetwork.Destroy(transform.GetComponent<PhotonView>());
        }
    }
}
