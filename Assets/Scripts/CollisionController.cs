using UnityEngine;
using System.Collections;

public class CollisionController : MonoBehaviour {


    private string hit = "<color=#ff0000>NOT HIT</color>";

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 50;
        textStyle.richText = true;
        GUI.Label(new Rect(Screen.width / 2.0f - 50.0f, 0, 200, 100), hit, textStyle);
    }


    void OnCollisionEnter(Collision collision)
    {
        hit = "YOU GOT HIT!";

    }
}
