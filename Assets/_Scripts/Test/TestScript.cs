using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered!");
    }

    //private void FixedUpdate()
    //{
    //    transform.position = new Vector3(transform.position.x+Time.deltaTime, 0, 0);
    //}
}
