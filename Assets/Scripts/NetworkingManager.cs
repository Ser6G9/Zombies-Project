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
        // Connexión con el servidor
        PhotonNetwork.ConnectUsingSettings();
        
        multiplayerButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Unirse a un Lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("A punto para jugar!");
        multiplayerButton.interactable = true;
    }

    public void FindMatch()
    {
        Debug.Log("Buscando sala...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MakeRoom();
    }

    public void MakeRoom()
    {
        int randomRoomName = Random.Range(0, 5000);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 6, PublishUserId = true};
        PhotonNetwork.CreateRoom($"RoomName_{randomRoomName}", roomOptions);
        Debug.Log($"Sala creada: {randomRoomName}");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(3);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
