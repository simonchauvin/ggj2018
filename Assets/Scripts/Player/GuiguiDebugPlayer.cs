using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiguiDebugPlayer : MonoBehaviour {
    public Flock flock;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame 
	void Update () {
        if(Input.GetButtonDown("Fire1")) 
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.dispersal,3.0f);
        if (Input.GetButtonDown("Fire2"))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.gathering, 3.0f);

    }
}
