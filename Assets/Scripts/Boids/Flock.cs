using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public Boid boidPrefab;
    public int numberOfBoids;
    public float followWeight;
    public float alignmentWeight;
    public float cohesionWeight;
    public float separationWeight;

    public Boid leader { get; private set; }
    private NavfieldManager navfieldManager;

    private Boid[] boids;


    void Start()
    {
        navfieldManager = GameObject.FindObjectOfType<NavfieldManager>();

        boids = new Boid[numberOfBoids];

        for (int i = 0; i < numberOfBoids; i++)
        {
            boids[i] = Instantiate(boidPrefab, transform.position, Quaternion.identity) as Boid;
            boids[i].transform.parent = transform;
        }
        leader = boids[Random.Range(0, boids.Length)];
    }


    void FixedUpdate()
    {
        for (int i = 0; i < numberOfBoids; i++)
        {
            Boid boid = boids[i];
            if (boid != null && boid.thisRigidbody != null)
            {
                //Vector3 follow = follow(boid) * followWeight * Time.deltaTime;
                Vector3 alignment = align(boid) * alignmentWeight * Time.deltaTime;
                Vector3 cohesion = cohere(boid) * cohesionWeight * Time.deltaTime;
                Vector3 separation = separate(boid) * separationWeight * Time.deltaTime;
                if (GameManager.instance.debugBoids)
                {
                    boid.showAlignmentDebug(alignment);
                    boid.showCohesionDebug(cohesion);
                    boid.showSeparationDebug(separation);
                }

                boid.thisRigidbody.velocity += (alignment + cohesion + separation);
                //boid.thisRigidbody.AddForce(align(boid) * alignmentWeight);
                //boid.thisRigidbody.AddForce(cohere(boid) * cohesionWeight);
                //boid.thisRigidbody.AddForce(separate(boid) * separationWeight);

                // Navfields
                Navfield navfield = navfieldManager.getNavfield(boid.transform.position);
                if (navfield != null)
                {
                    boid.applyNavfieldBehavior(navfield);
                }
            }
        }
    }

    private Vector3 follow(Boid boid)
    {
        Vector3 velocity = Vector3.zero;
        int count = 0;
        for (int i = 0; i < numberOfBoids; i++)
        {
            float distance = Vector3.Distance(boids[i].transform.localPosition, boid.transform.localPosition);
            if (distance > 0 && distance < boid.neighborRadius)
            {
                velocity += boids[i].thisRigidbody.velocity;
                count++;
            }
        }
        if (count > 0)
        {
            return (velocity / (numberOfBoids - 1)).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private Vector3 align(Boid boid)
    {
        Vector3 velocity = Vector3.zero;
        int count = 0;
        for (int i = 0; i < numberOfBoids; i++)
        {
            float distance = Vector3.Distance(boids[i].transform.localPosition, boid.transform.localPosition);
            if (distance > 0 && distance < boid.neighborRadius)
            {
                velocity += boids[i].thisRigidbody.velocity;
                count++;
            }
        }
        if (count > 0)
        {
            return (velocity / (numberOfBoids - 1)).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private Vector3 cohere(Boid boid)
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;
        for (int i = 0; i < numberOfBoids; i++)
        {
            float distance = Vector3.Distance(boids[i].transform.localPosition, boid.transform.localPosition);
            if (distance > 0 && distance < boid.neighborRadius)
            {
                centerOfMass += boids[i].transform.localPosition;
                count++;
            }
        }
        if (count > 0)
        {
            return ((centerOfMass / (numberOfBoids - 1)) - boid.transform.localPosition).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private Vector3 separate(Boid boid)
    {
        Vector3 velocity = Vector3.zero;
        int count = 0;
        for (int i = 0; i < numberOfBoids; i++)
        {
            float distance = Vector3.Distance(boids[i].transform.localPosition, boid.transform.localPosition);
            if (distance > 0 && distance < boid.desiredSeparation)
            {
                velocity -= (boids[i].transform.localPosition - boid.transform.localPosition).normalized / distance;
                count++;
            }
        }
        if (count > 0)
        {
            return (velocity / (numberOfBoids - 1)).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public void transmissionListener(int[] idLeaders, int [] idGrids) {
        string idL = ""; string idG = "";
        foreach (var item in idLeaders) {
            idL += item + ",";
        }
        foreach (var item in idGrids) {
            idG += item + ",";
        }

        Debug.Log("idLeaders "+idL + " idGrids " + idG);
    }

    public int[] getLeadersArray() {
        return new int[] { 1, 2, 3 };
    }

    public int[] getGridsArray() {
        return new int[] {2,3,4,5};
    }

    public Boid getLeader ()
    {
        return leader;
    }
}
