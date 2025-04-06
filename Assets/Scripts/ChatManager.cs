using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IOnEventCallback
{
    public TMP_InputField inputText;
    public GameObject messageTextPrefab;

    public Transform Content;
    
    public PhotonView photonView;
    public GameObject player;
    
    public const byte MESSAGE_EVENT = 1;

    private void Update()
    {
        // Al pulsar Enter se abre el input o se cierra y se muestra el mensaje
        if (Input.GetKeyUp(KeyCode.Return) && photonView.IsMine)
        {
            if (inputText.IsActive())
            {
                player.GetComponent<PlayerMovement>().enabled = true;
                if (inputText.text.Length > 0)
                {
                    CreateMessageText();
                }
                inputText.gameObject.SetActive(false);
            }
            else
            {
                player.GetComponent<PlayerMovement>().enabled = false;
                inputText.gameObject.SetActive(true);
                inputText.Select();
                inputText.ActivateInputField();
            }
        }
    }

    public void CreateMessageText()
    {
        RaiseEventOptions raiseOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        SendOptions sendOptions = new SendOptions {Reliability = true};

        PhotonNetwork.RaiseEvent(MESSAGE_EVENT, inputText.text, raiseOptions, sendOptions);
        inputText.text = "";
    }
    
    public void GetMessage(string text)
    {
        GameObject setMessage = Instantiate(messageTextPrefab, Vector3.zero, Quaternion.identity, Content);
        setMessage.transform.localPosition = Vector3.zero;
        setMessage.transform.localRotation = new Quaternion(0,0,0,0);
        setMessage.GetComponent<TextMeshProUGUI>().text = text;
        setMessage.transform.SetAsFirstSibling();
    }
    
    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == MESSAGE_EVENT)
        {
            string messageText = photonEvent.CustomData.ToString();
            GetMessage(messageText);
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
