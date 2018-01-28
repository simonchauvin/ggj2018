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
        if(Input.GetKeyDown(KeyCode.I)) 
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.dispersal,3.0f);
        if (Input.GetKeyDown(KeyCode.O))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.gathering, 3.0f);

        if (Input.GetKeyDown(KeyCode.P))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.ascension, 3.0f);
        if (Input.GetKeyDown(KeyCode.K))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.descent, 3.0f);
        if (Input.GetKeyDown(KeyCode.L))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.tube, 3.0f);
        if (Input.GetKeyDown(KeyCode.M))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.horizontal_compressor, 3.0f);
        if (Input.GetKeyDown(KeyCode.W))
            flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.vertical_compressor, 3.0f);
    }
}
