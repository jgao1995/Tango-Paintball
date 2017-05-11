using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MobileShooter : MonoBehaviour {

    //public GameObject Ball;
    public GameObject ARCamera;
    public Button ShootFrontButton;

    // Tango stuff 
    public GameObject TangoCamera;
    public GameObject TangoPositionController;
    private PositionController positionController;
    string shotFrom = "SHOOT TO";
    string position = "";
    string minBBPos = "minBB not set";
    string maxBBPos = "maxBB not set";
    public Vector3 currPos;
    Tango.TangoCoordinateFramePair fp;

    private Vector3 minBB, maxBB;
    private float actualImageSize = 10;


    bool started = false;
    //float swipespeed_max = 5; // 0.2s cross screen height
    float swipespeed_min = 1; // 1s cross screen height
    //float ballspeed_max = 25f;
    //float ballspeed_min = 2f;
    Vector3 mousedown_pos;
    float mousedowned_time;

    bool bMouseDown = false;
    float ballSpeedFixed = 25f;

    // Use this for initialization
    void Start()
    {
        ShootFrontButton.enabled = false;

        // resolving TangoPositionController for motion tracking, handled via PositionController script
        positionController = TangoPositionController.GetComponent("PositionController") as PositionController;

        // defining coordinate frame pair
        fp.baseFrame = Tango.TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_START_OF_SERVICE;
        fp.targetFrame = Tango.TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_DEVICE;
    }

    public void OnGUI()
    {
        // styleing
        GUIStyle textStyle = new GUIStyle();
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 25;
        textStyle.fontSize = 50;

        // Labels
        GUI.Label(new Rect(Screen.width / 2.0f - 350.0f, Screen.height / 2.0f - 350.0f, 200, 100), position, textStyle);
        GUI.Label(new Rect(Screen.width/2.0f - 100.0f, Screen.height/2.0f - 200.0f, 200, 100), shotFrom, textStyle);
        GUI.Label(new Rect(5.0f, 300.0f, 200, 100), minBBPos, textStyle);
        GUI.Label(new Rect(5.0f, 450.0f, 200, 100), maxBBPos, textStyle);

        // Buttons
        if (GUI.Button(new Rect(Screen.width - 310.0f, 300, 300, 100), "Reset Motion Tracking", buttonStyle))
        {
            resetTracking();
        }
        if (GUI.Button(new Rect(Screen.width - 310.0f, 450, 300, 100), "Enemy Fire", buttonStyle))
        {
            ReverseShootBall(ballSpeedFixed * TangoCamera.transform.forward);
        }
        if (GUI.Button(new Rect(Screen.width - 310.0f, 600, 300, 100), "Set Min BB", buttonStyle))
        {
            setMinBB();
        }
        if (GUI.Button(new Rect(Screen.width - 310.0f, 750, 300, 100), "Set Max BB", buttonStyle))
        {
            setMaxBB();
        }
    }

    public void Activate()
    {
        ShootFrontButton.enabled = true;
        ShootFrontButton.onClick.AddListener(ShootBallFront);
        started = true;
    }

    // shoot ball on swipe
    void Update()
    {
        if (!started) return;

        if (bMouseDown)
        {
            mousedowned_time += Time.deltaTime;
        }

        if (!bMouseDown && Input.GetMouseButtonDown(0))
        {
            mousedown_pos = Input.mousePosition;
            mousedowned_time = 0;
            bMouseDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!bMouseDown || mousedowned_time <= 0.05f) return;


            Vector3 mouseup_pos = Input.mousePosition;
            Vector3 delta = (mouseup_pos - mousedown_pos) / Screen.height;

            Vector3 swipe_vel = delta / mousedowned_time;
            //float swipe_speed = swipe_vel.magnitude;
            //if (swipe_speed > swipespeed_max) swipe_vel = swipe_vel.normalized * swipespeed_max;
            //if (swipe_speed < swipespeed_min) swipe_vel = swipe_vel.normalized * swipespeed_min;

            //Vector3 ball_vel = swipe_vel.normalized * ballspeed_min * swipe_speed / swipespeed_min;
            //Vector3 velocity = ARCamera.transform.right * ball_vel.x + ARCamera.transform.up * ball_vel.y;

            //ShootBall(velocity);

            if (swipe_vel.y > swipespeed_min)
            {
                ShootBallUp();
            }


            bMouseDown = false;
            mousedowned_time = 0;
        }

        updatePosition();
    }

    public void updatePosition()
    {
        Tango.TangoPoseData data = new Tango.TangoPoseData();
        Tango.PoseProvider.GetPoseAtTime(data, 0.0, fp);
        Matrix4x4 T_ss_d = data.ToMatrix4x4();

        //Vector3 tangoTranslation = new Vector3((float) data.translation.x, (float) data.translation.y, (float) data.translation.z);
        Matrix4x4 T_uw_ss = new Matrix4x4();
        T_uw_ss.SetRow(0, new Vector4(1f, 0f, 0f, 0f));
        T_uw_ss.SetRow(1, new Vector4(0f, 0f, 1f, 0f));
        T_uw_ss.SetRow(2, new Vector4(0f, 1f, 0f, 0f));
        T_uw_ss.SetRow(3, new Vector4(0f, 0f, 0f, 1f));

        Matrix4x4 T_d_uc = new Matrix4x4();
        T_d_uc.SetRow(0, new Vector4(1f, 0f, 0f, 0f));
        T_d_uc.SetRow(1, new Vector4(0f, 1f, 0f, 0f));
        T_d_uc.SetRow(2, new Vector4(0f, 0f, -1f, 0f));
        T_d_uc.SetRow(3, new Vector4(0f, 0f, 0f, 1f));

        Matrix4x4 T_uw_uc = (T_uw_ss * T_ss_d) * T_d_uc;
        currPos = T_uw_uc.GetColumn(3);
        position = "x: " + System.Math.Round(currPos.x,3) + ", y: " + System.Math.Round(currPos.y,3) + ", z: " + System.Math.Round(currPos.z,3);

    }

    // resets positional tracking for Tango. Extremely important you use this AT the center of the image for CALIBRATION.
    public void resetTracking()
    {
        Tango.PoseProvider.ResetMotionTracking();
    }

    // functions for finding image size based on what screen we are using
    public void setMinBB()
    {
        minBB = positionController.ReadReticulePosition();
        minBBPos = "minBB: (" + System.Math.Round(minBB.x, 3) + "," + System.Math.Round(minBB.y, 3) + ")";

    }

    public void setMaxBB()
    {
        maxBB = positionController.ReadReticulePosition();
        maxBBPos = "minBB: (" + System.Math.Round(maxBB.x, 3) + "," + System.Math.Round(maxBB.y, 3) + ")";

    }

    // shoots a ball and spanws it across the photon network
    public void ShootBall(Vector3 velocity)
    {
        GetComponent<AudioSource>().Play();
        Vector3 relativePosition = positionController.ReadRelativePosition(currPos);
        float scale = actualImageSize / (maxBB.x - minBB.x);
        Vector3 scaledPosition = new Vector3(relativePosition.x * scale, relativePosition.y * scale, relativePosition.z);
        shotFrom = "x: " + System.Math.Round(scaledPosition.x, 3) + ", y: " + System.Math.Round(scaledPosition.y, 3);
        var ball = PhotonNetwork.Instantiate("ball", scaledPosition, Quaternion.identity, 0);
        Color color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f, 1f, 1f);
        Vector3 color_v = new Vector3(color.r, color.g, color.b); // Since UnityEngine.Color is not Serializable by Photon in Unity 5.3.5
  
        if (ball != null)
        {

            ball.GetComponent<PhotonView>().RPC("RPCInitialize", PhotonTargets.All, velocity, color_v);
        }
    }

    public void ReverseShootBall(Vector3 velocity)
    {
        var ball = PhotonNetwork.Instantiate("ball", new Vector3(0, 0, 18), Quaternion.identity, 0);
        Color color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f, 1f, 1f);
        Vector3 color_v = new Vector3(color.r, color.g, color.b); // Since UnityEngine.Color is not Serializable by Photon in Unity 5.3.5

        if (ball != null)
        {
            ball.GetComponent<PhotonView>().RPC("RPCInitialize", PhotonTargets.All, -1 * velocity, color_v);
        }
    }

    public void ShootBallFront()
    {
        ShootBall(ballSpeedFixed * TangoCamera.transform.forward);
    }

    public void ShootBallUp()
    {
        ShootBall(ballSpeedFixed * TangoCamera.transform.up);
    }
}
