using UnityEngine;
using System.Collections;

public class PCNetwork_Cube : Photon.PunBehaviour
{
    string roomName;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
        roomName = GenerateRoomName();
    }

    static string GenerateRoomName()
    {
        const string characters = "abcdefghijklmnopqrstuvwxyz0123456789"; //add the characters you want

        string result = "";

        int charAmount = Random.Range(4, 6); //set those to the minimum and maximum length of your string
        for (int i = 0; i < charAmount; i++)
        {
            result += characters[Random.Range(0, characters.Length)];
        }

        return result;
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Label("Room Name: " + roomName);
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