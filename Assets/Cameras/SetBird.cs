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

            ObjectTrackedByCamera.position = Vector3.Lerp(ObjectTrackedByCamera.position, FlockSystem.GetComponent<Flock>().getBarycenter(), 0.5f);

    }
}
