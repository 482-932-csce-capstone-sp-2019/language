using UnityEngine;
using System.Collections;
using System;


public class FollowPlayer : MonoBehaviour
{

    private GameObject player;       //Public variable to store a reference to the player game object
    public GameObject boundary;
    private int minx, maxx, miny, maxy;
    public float zofsett = -100f;


    private Vector3 offset;         //Private variable to store the offset distance between the player and camera

    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        this.player = GameObject.FindGameObjectWithTag("Player");
        offset.x = 0;
        offset.y = 0;
        offset.z = zofsett;

        //PlayerController player = obj.GetComponent<PlayerController>();
        minx = (int)boundary.transform.position.x;
        miny = (int)boundary.transform.position.y;
        maxx = (int)boundary.transform.localScale.x;

        maxy = (int)boundary.transform.localScale.y;

    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        Vector3 pos = player.transform.position + offset;
        pos.x = Math.Min(pos.x, maxx);
        pos.x = Math.Max(pos.x, minx);
        pos.y = Math.Min(pos.y, maxy);
        pos.y = Math.Max(pos.y, miny);
        transform.position = pos;
    }

    public void reset()
    {
        //Gets called whenever scene is changed
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        this.player = GameObject.FindGameObjectWithTag("Player");
        offset.x = 0;
        offset.y = 0;

        //PlayerController player = obj.GetComponent<PlayerController>();
        minx = (int)boundary.transform.position.x;
        miny = (int)boundary.transform.position.y;
        maxx = (int)boundary.transform.localScale.x;

        maxy = (int)boundary.transform.localScale.y;
        //Debug.Log("offset: " + offset.x + ", " + offset.y + ", " + offset.z);
    }
}