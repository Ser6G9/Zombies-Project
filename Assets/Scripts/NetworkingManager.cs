using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    public Button multiplayerButton;
    
    void Start()
    {
        Debug.Log("Connexió a un servidor");
        // Connexió amb el servidor
        PhotonNetwork.ConnectUsingSettings();
        
        multiplayerButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Unir-mos a un Lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Estam apunt per jugar!");
        multiplayerButton.interactable = true;
    }

    public void FindMatch()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);
        MakeRoom();
    }

    public void MakeRoom()
    {
        int randomRoomName = Random.Range(0, 5000);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 6, PublishUserId = true};
        PhotonNetwork.CreateRoom($"RoomName_{randomRoomName}", roomOptions);
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        PhotonNetwork.LoadLevel(2);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
