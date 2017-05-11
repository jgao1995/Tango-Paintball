using UnityEngine;
using System.Collections;

public class GyroController : MonoBehaviour
{
    public GameObject Cube
    {
        get { return cube; }
        set
        {
            cube = value;
            ResetOrientation();
        }
    }

    Quaternion qRefCube = Quaternion.identity;
    Quaternion qRefGyro = Quaternion.identity;
    Gyroscope gyro;
    Compass compass;

    bool init_tracked = false;

    private GameObject cube;
    


    // Use this for initialization
    void Start()
    {
        gyro = Input.gyro;
        gyro.enabled = true;
        gyro.updateInterval = 0.01f;
    }

    void OnGUI()
    {
        GUILayout.Label(".");
        GUILayout.Label(".");
        GUILayout.Label("Gyroscope attitude : " + gyro.attitude);
        GUILayout.Label("Gyroscope attitude (Euler) :" + gyro.attitude.eulerAngles);
        GUILayout.Label("Gyroscope gravity : " + gyro.gravity);
        GUILayout.Label("Gyroscope rotationRate : " + gyro.rotationRate);
        GUILayout.Label("Gyroscope rotationRateUnbiased : " + gyro.rotationRateUnbiased);
        GUILayout.Label("Gyroscope updateInterval : " + gyro.updateInterval);
        GUILayout.Label("Gyroscope userAcceleration : " + gyro.userAcceleration);
        GUILayout.Label("Ref camera rotation:" + qRefCube);
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
        if (cube != null)
        {
            var delta_rotation = Quaternion.Inverse(qRefGyro) * ConvertRotation(Input.gyro.attitude);
            cube.transform.rotation = qRefCube * delta_rotation; // apply local rotation to camera
        }
    }

    public void ResetOrientation()
    {
        if (cube == null)
        {
            return;
        }
        qRefCube = cube.transform.rotation;
        qRefGyro = ConvertRotation(Input.gyro.attitude);
        init_tracked = true;
    }

}
