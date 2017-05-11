using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PositionController : MonoBehaviour {
    public GameObject triad;
    private TangoPointCloud m_pointCloud;
    string text = "ORIGIN NOT SET";
    string debug_text = "DEBUG INFO";
    string reticule = "+";
    string uw_pos = "UNITY WORLD POS";
    bool axis_set = false;
    bool reticule_on = false;
    private GameObject triad_instance;
    public Vector3 new_origin_pos;

    // GUI space has (0, 0) based on top-left, Input.GetTouch based on bot-left
    public void OnGUI()
    {

        // STYLING
        GUIStyle textStyle = new GUIStyle();
        GUIStyle reticuleStyle = new GUIStyle();
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        textStyle.fontSize = 40;
        reticuleStyle.fontSize = 100;
        buttonStyle.fontSize = 25;

        // LABELS
        GUI.Label(new Rect(Screen.width / 2.0f, Screen.height / 2.0f, 10, 10), reticule, reticuleStyle);

        // BUTTONS
        if (GUI.Button(new Rect(Screen.width - 310.0f, 0, 300, 100), "Toggle Reticule", buttonStyle))
        {
            ToggleReticule();
        }
        if (GUI.Button(new Rect(Screen.width - 310.0f, 150, 300, 100), "Set Origin", buttonStyle))
        {
            PlaceTriad(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f - 50.0f));
        }
    }
    // Use this for initialization
    void Start()
    {
        m_pointCloud = FindObjectOfType<TangoPointCloud>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Ended)
            {
                // t.position returns the pixel coordinate space of the screen
            }
        }
    }

    void ToggleReticule()
    {
        if (reticule_on)
        {
            reticule_on = false;
            reticule = "";
        }
        else
        {
            reticule_on = true;
            reticule = "+";
        }
    }

    // deprecated function i made to read depth
    //void ReadDepth(Vector2 touchPosition)
    //{
    //    Camera cam = Camera.main;
    //    Vector3 colorCameraPoint;

    //    m_pointCloud.EstimateDepthOnScreen(cam, touchPosition, out colorCameraPoint);
    //    text = Convert.ToString(colorCameraPoint.z);
    //    debug_text = touchPosition.ToString();
    //}

    void PlaceTriad(Vector2 touchPosition)
    {
        // Find the plane.
        Camera cam = Camera.main;
        Vector3 planeCenter;
        Plane plane;
        if (!m_pointCloud.FindPlane(cam, touchPosition, out planeCenter, out plane))
        {
            Debug.Log("cannot find plane.");
            text = "cannot find plane.";
            return;
        }
        // remove previous axis if it's already been set
        if (axis_set)
        {
            Destroy(triad_instance);
            axis_set = false;
        }
        // if normal faces towards us, reorient triad
        if (Vector3.Angle(plane.normal, Vector3.up) > 60.0f && Vector3.Angle(plane.normal, Vector3.up) < 120.0f)
        {
            text = "placing reoriented";
            Vector3 up = Vector3.up;
            Vector3 forward = plane.normal;
            //Vector3 right = Vector3.Cross(up, forward).normalized;
            triad_instance = Instantiate(triad, planeCenter, Quaternion.LookRotation(forward, up)) as GameObject;
            triad_instance.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
        }
        // else if normal faces positive gravitiy, place normally 
        else
        {
            text = "normal placement";
            Vector3 up = plane.normal;
            Vector3 right = Vector3.Cross(plane.normal, cam.transform.forward).normalized;
            Vector3 forward = Vector3.Cross(right, plane.normal).normalized;
            triad_instance = Instantiate(triad, planeCenter, Quaternion.LookRotation(-forward, up)) as GameObject;
            triad_instance.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
        }
        axis_set = true;

        // *** spawning new triad across network ***
        var board_triad = PhotonNetwork.Instantiate("triad", new Vector3(0, 0, 18.60f), Quaternion.identity, 0);
        board_triad.GetComponent<PhotonView>().RPC("SpawnTriad", PhotonTargets.All);
        // *** still not working ***

        new_origin_pos = planeCenter;
    }

    public Vector3 ReadRelativePosition(Vector3 pos)
    {   
        if (axis_set)
        {
            return new Vector3(pos.x - new_origin_pos.x, pos.y - new_origin_pos.y, pos.z - new_origin_pos.z);
        }
        else
        {
            return pos;
        }
    }

    public Vector3 ReadReticulePosition()
    {
        Camera cam = Camera.main;
        Vector3 planeCenter;
        Plane plane;
        m_pointCloud.FindPlane(cam, new Vector2(Screen.width / 2.0f, Screen.height / 2.0f - 50.0f), out planeCenter, out plane);
        return planeCenter;
    }
}
