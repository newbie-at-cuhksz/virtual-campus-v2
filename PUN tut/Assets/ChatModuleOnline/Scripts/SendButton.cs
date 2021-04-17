/*
 * This script should be attach to ChatInputField > SendButton
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendButton : MonoBehaviour
{
    public bool ToSendBubble = false;

    public void SetToSend()
    {
        if (!ToSendBubble)
        {
            ToSendBubble = true;

            StartCoroutine("DisableToSend");
        }
    }

    IEnumerator DisableToSend()
    {
        yield return new WaitForSeconds(0.1f);
        ToSendBubble = false;
    }
}
