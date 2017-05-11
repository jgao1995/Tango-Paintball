using UnityEngine;
using System.Collections;

public class TriadBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [PunRPC]
    public void SpawnTriad()
    {
        if (PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RequestOwnership();
        }
        var triad_instance = Instantiate(this.gameObject, new Vector3(0, 0, 18.60f), Quaternion.identity) as GameObject;
    }
}
