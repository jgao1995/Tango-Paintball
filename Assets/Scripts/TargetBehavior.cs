using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Vuforia;
using System;

public class TargetBehavior : MonoBehaviour, ITrackableEventHandler
{

    public Button TrackButton;
    public Button ShotTopButton;
    public CameraGyroBehavior CameraGyro;
    bool tracked = false;

    //private float tracking_timer = 2;
    //private bool tracking = true;

    //void ResetTrackingTimer()
    //{
    //    tracking_timer = 2;
    //}

    void ResumeTracking()
    {
        //ResetTrackingTimer();
        Tracker imageTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        imageTracker.Start();
    }

    void PauseTracking()
    {
        Tracker imageTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        imageTracker.Stop();

        // use only gyroscope after looking at target for 2 seconds
        // to prevent camera looking at edge of object before shooting and get a bad tracking result
    }

    // Use this for initialization
    void Start () {

        var mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }

        //ResetTrackingTimer();
        TrackButton.onClick.AddListener(ResumeTracking);
        //CameraGyro.StartGyroTracking();

        //ShotTopButton.onClick.AddListener(ResumeTracking);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //if (tracking)
        //{
        //    tracking_timer -= Time.deltaTime;
        //    if (tracking_timer <= 0)
        //    {
        //        tracking = false;
        //        PauseTracking();
        //        TrackButton.image.color = new Color(1, 1, 1, 0.5f);
        //    }
        //}

        if (tracked)
        {
            CameraGyro.UpdateOrientation(Time.fixedDeltaTime);
        }
    }
    
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        switch (newStatus)
        {
            case TrackableBehaviour.Status.TRACKED:
                
                tracked = true;
                CameraGyro.ResetOrientation();
                CameraGyro.StopGyroTracking();
                //ResetTrackingTimer();
                //tracking = true;
                TrackButton.image.color = new Color(0.4f, 1, 0.1f, 0.5f);

                break;
            case TrackableBehaviour.Status.EXTENDED_TRACKED:
                tracked = false;
                CameraGyro.StopGyroTracking();
                //ResetTrackingTimer();
                //tracking = true;
                TrackButton.image.color = new Color(0.7f, 0.5f, 0.1f, 0.5f);
                break;
            default:
                tracked = false;
                CameraGyro.StartGyroTracking();
                //tracking = false;
                TrackButton.image.color = new Color(1, 0.1f, 0.1f, 0.5f);
                break;
        }
        
    }
}
