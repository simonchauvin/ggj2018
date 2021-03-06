﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    #region Parameters
    public bool leader;
    public float neighborRadius;
    public float desiredSeparation;
    #endregion

    #region Components
    public Rigidbody thisRigidbody { get; private set; }
    private Transform neighborArea;
    private Transform separationArea;
    #endregion

    private Flock flock;
    public Boid[] neighbors { get; private set; }


    void Start()
    {
        // Components
        thisRigidbody = GetComponent<Rigidbody>();
        neighborArea = transform.Find("NeighborArea");
        separationArea = transform.Find("SeparationArea");

        // Flock
        flock = GetComponentInParent<Flock>();

        // Init
        GetComponent<Animator>().SetFloat("speedMultiplier", Random.Range(flock.minAnimSpeed, flock.maxAnimSpeed));
        transform.position = new Vector3(Random.value * 10f, Random.value * 10f, Random.value * 10f);
        //transform.position = new Vector3(Random.value * 10f, 0f, Random.value * 10f);
        thisRigidbody.velocity = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1, Random.value * 2 - 1);
        //thisRigidbody.velocity = new Vector3(Random.value * 2 - 1, 0f, Random.value * 2 - 1);

        neighborArea.localScale = new Vector3(neighborRadius * 2, neighborRadius * 2, neighborRadius * 2);
        separationArea.localScale = new Vector3(desiredSeparation * 2, desiredSeparation * 2, desiredSeparation * 2);
        neighborArea.gameObject.SetActive(false);
        separationArea.gameObject.SetActive(false);
    }

    public void init()
    {
        if (GameManager.instance.debugNavfields && leader)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.cyan;
        }
    }

    public void setNeighbors(Boid[] neighbors)
    {
        this.neighbors = new Boid[neighbors.Length];
        for (int i = 0; i < neighbors.Length; i++)
        {
            this.neighbors[i] = neighbors[i];
        }
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(thisRigidbody.velocity);

        if (GameManager.instance.debugBoids)
        {
            neighborArea.gameObject.SetActive(true);
            //separationArea.gameObject.SetActive(true);
        }
        else
        {
            neighborArea.gameObject.SetActive(false);
            //separationArea.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        /*foreach (Collider collider in Physics.OverlapSphere(transform.localPosition, distance, LayerMask.GetMask("Boid")))
        {
        }*/

        boundPosition();
        //warpPosition();
    }

    private void boundPosition()
    {
        if (Vector3.Distance(transform.localPosition, Vector3.zero) > flock.boundRadius)
        {
            //thisRigidbody.AddForce((Vector3.zero - transform.localPosition) * 0.01f);
            thisRigidbody.velocity += (Vector3.zero - transform.localPosition) * 0.1f * Time.deltaTime;
        }
    }

    private void warpPosition()
    {
        if (Vector3.Distance(transform.localPosition, Vector3.zero) > flock.boundRadius)
        {
            transform.localPosition = -transform.localPosition + (transform.localPosition - Vector3.zero).normalized;
        }
    }

    public void applyNavfieldBehavior(Navfield navfield)
    {
        thisRigidbody.velocity += navfield.getForce(transform.position) * Time.deltaTime;
    }

    public void showFollowingDebug(Vector3 velocity)
    {
        Debug.DrawRay(transform.localPosition, velocity, Color.yellow);
    }

    public void showAlignmentDebug(Vector3 velocity)
    {
        Debug.DrawRay(transform.localPosition, velocity, Color.blue);
    }

    public void showCohesionDebug(Vector3 velocity)
    {
        Debug.DrawRay(transform.localPosition, velocity, Color.green);
    }

    public void showSeparationDebug(Vector3 velocity)
    {
        Debug.DrawRay(transform.localPosition, velocity, Color.red);
    }
}
