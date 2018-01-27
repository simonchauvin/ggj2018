using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public Flock flock;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("a")) {
            Debug.Log("A");
            sendBirdCommand(new int[] { 1, 3, 5, 7, 9 }, new int[] { 2 });
        }
            //Instantiate(projectile, transform.position, transform.rotation);

        if (Input.GetKeyDown("z")) {
            Debug.Log("up");
        }

        if (Input.GetKeyDown("e")) {
            Debug.Log("down");
        }
        //Instantiate(projectile, transform.position, transform.rotation);
    }

    void sendBirdCommand(int[] idLeaders, int[] idGrids) {
        flock.transmissionListener(idLeaders, idGrids);

    }
}
