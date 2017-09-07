using System;
using UnityEngine;

public class MainCameraControl : MonoBehaviour {

    public GameObject player;       //Public variable to store a reference to the player game object

    public float MainCameraXRotateDegrees; //from 70 to 0, radians = (Math.PI / 180) * degrees

    public float Distance; // from 40 to 10?

    private double radians;
    private Vector3 offset;         //Private variable to store the offset distance between the player and camera

    // Use this for initialization
    void OnEnable()
    {
        OnChangeRotateDegreesOrDistance();
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = player.transform.position + offset;
    }

    void OnChangeRotateDegreesOrDistance()
    {
        radians = (Math.PI / 180) * MainCameraXRotateDegrees;
        offset.z = (float) (-Distance * Math.Cos(radians));
        offset.y = (float) (Distance * Math.Sin(radians));
        offset.x = 0f;
        transform.SetPositionAndRotation(player.transform.position + offset, Quaternion.Euler(MainCameraXRotateDegrees,0,0));
    }
}
