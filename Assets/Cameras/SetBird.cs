using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBird : MonoBehaviour {

    public Transform ObjectTrackedByCamera;
    public Transform FlockSystem;
    private Transform Leader;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Leader == null)
        {
            FlockSystem.GetComponent<Flock>().getLeader();
        }
        else
            ObjectTrackedByCamera.position = Leader.position;

    }
}
