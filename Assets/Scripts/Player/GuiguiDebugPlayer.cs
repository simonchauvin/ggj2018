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
        if(Input.GetKeyDown(KeyCode.A)) 
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.dispersal,3.0f);
        if (Input.GetKeyDown(KeyCode.Z))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.gathering, 3.0f);

        if (Input.GetKeyDown(KeyCode.E))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.ascension, 3.0f);
        if (Input.GetKeyDown(KeyCode.R))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.descent, 3.0f);
        if (Input.GetKeyDown(KeyCode.T))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.tube, 3.0f);
        if (Input.GetKeyDown(KeyCode.Y))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.horizontal_compressor, 3.0f);
        if (Input.GetKeyDown(KeyCode.U))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.vertical_compressor, 3.0f);
    }
}
