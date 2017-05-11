using UnityEngine;
using System.Collections;

public class PCNetwork : Photon.PunBehaviour
{
    string roomName;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
        roomName = GenerateRoomName();
    }

    static string GenerateRoomName()
    {
        const string characters = "a"; 

        string result = "";

        int charAmount = Random.Range(1, 2); 
        for (int i = 0; i < charAmount; i++)
        {
            result += characters[Random.Range(0, characters.Length)];
        }

        return result;
    }

    void OnGUI()
    {
        GUI.contentColor = Color.red;
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString() + " Room Name: " + roomName);

    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        base.OnPhotonJoinRoomFailed(codeAndMsg);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }
}