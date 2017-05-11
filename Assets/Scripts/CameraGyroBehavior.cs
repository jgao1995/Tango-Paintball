using UnityEngine;
using System.Collections;

/// <summary>
/// This class sync camera using data from gyro
/// </summary>
public class CameraGyroBehavior : MonoBehaviour {

    Quaternion qRefCam;
    Quaternion qRefGyro = Quaternion.identity;
    bool gyroTracking = false;
    Gyroscope gyro;
    Compass compass;

    bool init_tracked = false;


    // Use this for initialization
    void Start()
    {
        gyro = Input.gyro;
        gyro.enabled = true;
        gyro.updateInterval = 0.01f;
        qRefCam = transform.rotation;

        // Enabling compass
        compass = Input.compass;
        compass.enabled = true;
    }

    void getCorrectedGyroOrientation()
    {

    }

    void OnGUI()
    {
        GUILayout.Label("");
        GUILayout.Label("Gyroscope attitude : " + gyro.attitude);
        GUILayout.Label("Gyroscope gravity : " + gyro.gravity);
        GUILayout.Label("Gyroscope rotationRate : " + gyro.rotationRate);
        GUILayout.Label("Gyroscope rotationRateUnbiased : " + gyro.rotationRateUnbiased);
        GUILayout.Label("Gyroscope updateInterval : " + gyro.updateInterval);
        GUILayout.Label("Gyroscope userAcceleration : " + gyro.userAcceleration);
        GUILayout.Label("Ref camera rotation:" + qRefCam);
        GUILayout.Label("Ref gyro attitude:" + qRefGyro);
    }
    
    // Converts the rotation from right handed to left handed. Different device may be different
    private static Quaternion ConvertRotation(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }


    // Update is called once per frame
    void Update()
    {
        if (gyroTracking)
        {
            var delta_rotation = Quaternion.Inverse(qRefGyro) * ConvertRotation(Input.gyro.attitude);
            transform.rotation = qRefCam * delta_rotation; // apply local rotation to camera
        }
    }

    public void ResetOrientation()
    {
        qRefCam = transform.rotation;
        qRefGyro = ConvertRotation(Input.gyro.attitude);
        init_tracked = true;


    }

    public void UpdateOrientation(float deltatime)
    {
        if (!init_tracked)
        {
            ResetOrientation();
        }
        else
        {
            float smooth = 1f;
            qRefCam = Quaternion.Slerp(qRefCam, transform.rotation, smooth * deltatime);
            qRefGyro = Quaternion.Slerp(qRefGyro, ConvertRotation(gyro.attitude), smooth * deltatime);
        }
    }

    public void StartGyroTracking()
    {
        if (gyroTracking) return;

        //camera_origin_quat = transform.rotation;
        //begin_quat = Input.gyro.attitude;
        gyroTracking = true;
    }

    public void StopGyroTracking()
    {
        gyroTracking = false;
    }


}
