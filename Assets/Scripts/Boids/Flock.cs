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
    public float minAnimSpeed = 0.75f;
    public float maxAnimSpeed = 1.25f;
    public float minTimeBetweenRandomWalk = 5;
    public float maxTimeBetweenRandomWalk = 15;
    public float boundRadius = 40f;

    private Boid leader;
    private NavfieldManager navfieldManager;

    public Boid[] boids { get; private set; }
    private Vector3 barycenter;


    private float randomWalkTime;
    private float returnTime;
    private float timeSinceLastRandomWalk;
    private float timeSinceLastReturn;
    private bool randomWalk;
    private bool returnWalk;
    private float timeSinceLastCheck;
    private Vector3 target;

    public float debug1;


    void Awake()
    {
        navfieldManager = GameObject.FindObjectOfType<NavfieldManager>();

        boids = new Boid[numberOfBoids];
        boids[0] = GetComponentInChildren<Boid>();
        boids[0].init();
        boids[0].setNeighbors(new Boid[0]);
        leader = boids[0];
        for (int i = 1; i < numberOfBoids; i++)
        {
            boids[i] = Instantiate(boidPrefab, transform.position, Quaternion.identity) as Boid;
            boids[i].transform.parent = transform;
            boids[i].init();
        }
        // Set neighbors
        int index = 0;
        int neighborIndexLeft = numberOfBoids - 1;
        Boid[] neighbors;
        List<Boid> neighborsLeft = new List<Boid>(boids);
        for (int i = 1; i < numberOfBoids; i++)
        {
            neighbors = new Boid[1];
            for (int j = 0; j < 1; j++)
            {
                index = Random.Range(1, neighborIndexLeft);
                neighbors[j] = neighborsLeft[index];
                neighborsLeft.RemoveAt(index);
                neighborIndexLeft--;
            }
            boids[i].setNeighbors(neighbors);
        }

        // Init
        barycenter = Vector3.zero;
        timeSinceLastCheck = 0f;
        randomWalkTime = 0f;
        returnTime = 0f;
        timeSinceLastRandomWalk = 0f;
        timeSinceLastReturn = 0f;
        randomWalk = false;
        returnWalk = false;
        target = Vector3.zero;
    }

    void FixedUpdate()
    {
        //int updateStart = (int)(Random.value * (boids.Length - numberOfBoids / 5 - 1));
        barycenter = Vector3.zero;
        for (int i = 0; i < numberOfBoids; i++)
        {
            Boid boid = boids[i];
            if (boid != null && boid.thisRigidbody != null)
            {
                if (boid.leader)
                {
                    // Random walks
                    if (randomWalk)
                    {
                        leader.thisRigidbody.velocity += (leader.transform.position - target).normalized * Time.deltaTime;
                        if (timeSinceLastRandomWalk >= randomWalkTime)
                        {
                            checkLeaderWalk();
                        }
                        timeSinceLastRandomWalk += Time.deltaTime;
                    }
                    else if (returnWalk)
                    {
                        leader.thisRigidbody.velocity += (leader.transform.position - target).normalized * Time.deltaTime;
                        if (timeSinceLastReturn >= returnTime)
                        {
                            checkLeaderWalk();
                        }
                        timeSinceLastReturn += Time.deltaTime;
                    }
                    else
                    {
                        if (timeSinceLastCheck >= 5f)
                        {
                            checkLeaderWalk();
                            timeSinceLastReturn = 0f;
                        }
                        timeSinceLastCheck += Time.deltaTime;
                    }
                }
                else
                {
                    Vector3 following = follow(boid) * followWeight * Time.deltaTime;
                    Vector3 alignment = align(boid) * alignmentWeight * Time.deltaTime;
                    Vector3 cohesion = cohere(boid) * cohesionWeight * Time.deltaTime;
                    Vector3 separation = separate(boid) * separationWeight * Time.deltaTime;
                    if (GameManager.instance.debugBoids)
                    {
                        boid.showFollowingDebug(following);
                        boid.showAlignmentDebug(alignment);
                        boid.showCohesionDebug(cohesion);
                        boid.showSeparationDebug(separation);
                    }

                    boid.thisRigidbody.velocity += (following + alignment + cohesion + separation);
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

                //Guigui Fix
                float peurDuSol = Mathf.Pow(8.0f - Mathf.Clamp(boid.transform.position.y, 0.0f, 8.0f), 2.0f);

                float vely = boid.thisRigidbody.velocity.y + Time.fixedDeltaTime * peurDuSol * Mathf.Abs(Mathf.Min(0, boid.thisRigidbody.velocity.y));
                if (peurDuSol > 0 && vely < 5.0f)
                    vely += 15.0f * Time.fixedDeltaTime;
                float speed = boid.thisRigidbody.velocity.magnitude;
                boid.thisRigidbody.velocity = new Vector3(boid.thisRigidbody.velocity.x, vely, boid.thisRigidbody.velocity.z);

                if (speed > 10.0f)
                    speed = 10.0f;

                boid.thisRigidbody.velocity = boid.thisRigidbody.velocity.normalized * speed;

                barycenter += boid.transform.position;
            }
        }
        barycenter = barycenter / numberOfBoids;
    }

    private void checkLeaderWalk()
    {
        float alea = Random.value;
        if (alea > 0.5f)
        {
            if (alea > 0.75f)
            {
                randomWalk = true;

                Vector3 pos = Random.onUnitSphere * boundRadius;
                if (pos.y < 0)
                {
                    pos.y = -pos.y;
                }

                target = pos;
                randomWalkTime = Random.Range(minTimeBetweenRandomWalk, maxTimeBetweenRandomWalk);
                timeSinceLastRandomWalk = 0f;

                returnWalk = false;
            }
            else
            {
                returnWalk = true;
                target = Vector3.zero;
                returnTime = Random.Range(minTimeBetweenRandomWalk, maxTimeBetweenRandomWalk);
                timeSinceLastReturn = 0f;

                randomWalk = false;
            }
        }
        else
        {
            randomWalk = false;
            returnWalk = false;
        }
    }

    private Vector3 follow(Boid boid)
    {
        return leader.thisRigidbody.velocity;
    }

    private Vector3 align(Boid boid)
    {
        Vector3 velocity = Vector3.zero;
        int count = 0;
        for (int i = 0; i < boid.neighbors.Length; i++)
        {
            //float distance = Vector3.Distance(boids[i].transform.localPosition, boid.transform.localPosition);
            //if (distance > 0 && distance < boid.neighborRadius)
            //{
                velocity += boids[i].thisRigidbody.velocity;
                count++;
            //}
        }
        if (count > 0)
        {
            //return (velocity / (boid.neighbors.Length - 1)).normalized;
            return (velocity / (boid.neighbors.Length)).normalized;
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
        for (int i = 0; i < boid.neighbors.Length; i++)
        {
            //float distance = Vector3.Distance(boids[i].transform.localPosition, boid.transform.localPosition);
            //if (distance > 0 && distance < boid.neighborRadius)
            //{
                centerOfMass += boids[i].transform.localPosition;
                count++;
            //}
        }
        if (count > 0)
        {
            //return ((centerOfMass / (boid.neighbors.Length - 1)) - boid.transform.localPosition).normalized;
            return ((centerOfMass / (boid.neighbors.Length)) - boid.transform.localPosition).normalized;
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
        for (int i = 0; i < boid.neighbors.Length; i++)
        {
            float distance = Vector3.Distance(boids[i].transform.localPosition, boid.transform.localPosition);
            //if (distance > 0 && distance < boid.desiredSeparation)
            //{
                velocity -= (boids[i].transform.localPosition - boid.transform.localPosition).normalized / distance;
                count++;
            //}
        }
        if (count > 0)
        {
            //return (velocity / (boid.neighbors.Length - 1)).normalized;
            return (velocity / (boid.neighbors.Length)).normalized;
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

    public Vector3 getBarycenter ()
    {
        return barycenter;
    }
}
