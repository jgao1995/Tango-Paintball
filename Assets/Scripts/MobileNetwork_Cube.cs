using UnityEngine;

public class MobileNetwork_Cube : Photon.PunBehaviour
{
    public Canvas Canvas;
    public UnityEngine.UI.Text RoomNameInput;
    public UnityEngine.UI.Button EnterButton;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void JoinRoom()
    {
        PhotonNetwork.JoinRoom(RoomNameInput.text);
    }

    public override void OnJoinedLobby()
    {
        //PhotonNetwork.JoinRoom("testroom");
        //PhotonNetwork.JoinRandomRoom();
        EnterButton.onClick.AddListener(JoinRoom);
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        base.OnPhotonJoinRoomFailed(codeAndMsg);
    }

    public override void OnJoinedRoom()
    {
        var cube = PhotonNetwork.Instantiate("PhoneCube", Vector3.zero, Quaternion.identity, 0);
        GetComponent<GyroController>().Cube = cube;

        Destroy(Canvas);
    }


}
