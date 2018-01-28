using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navfield
{
    public static int SIZE = 32;
    public static float CELL_SIZE = 1;
    public static float FORCE = 1f;

    public Vector3 origin;
    public Quaternion rotation;
    private Vector3[,,] forces;
    private float duration;
    private float time;


    public Navfield(Flock flock, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        Create(flock.getLeader().transform.position, orientation, primitive, duration);
    }

    public Navfield(Boid leader, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        Create(leader.transform.position, orientation, primitive, duration);
    }

    public Navfield(Vector3 position, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        Create(position, orientation, primitive, duration);
    }

    private void Create(Vector3 position, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        origin = position;
        rotation = orientation;
        forces = new Vector3[Navfield.SIZE, Navfield.SIZE, Navfield.SIZE];
        this.duration = duration;
        time = 0f;

        switch (primitive)
        {
            case NavFieldPrimitives.dispersal:
                for (int i = 0; i < Navfield.SIZE; i++)
                {
                    for (int j = 0; j < Navfield.SIZE; j++)
                    {
                        for (int k = 0; k < Navfield.SIZE; k++)
                        {
                            forces[i, j, k] = Navfield.FORCE * new Vector3((i - (Navfield.SIZE * 0.5f)) / (Navfield.SIZE * 0.5f), (j - (Navfield.SIZE * 0.5f)) / (Navfield.SIZE * 0.5f), (k - (Navfield.SIZE * 0.5f)) / (Navfield.SIZE * 0.5f));
                        }
                    }
                }
                break;
            case NavFieldPrimitives.gathering:
                for (int i = 0; i < Navfield.SIZE; i++)
                {
                    for (int j = 0; j < Navfield.SIZE; j++)
                    {
                        for (int k = 0; k < Navfield.SIZE; k++)
                        {
                            forces[i, j, k] = Navfield.FORCE * -new Vector3((i - (Navfield.SIZE * 0.5f)) / (Navfield.SIZE * 0.5f), (j - (Navfield.SIZE * 0.5f)) / (Navfield.SIZE * 0.5f), (k - (Navfield.SIZE * 0.5f)) / (Navfield.SIZE * 0.5f));
                        }
                    }
                }
                break;
        }
    }

    private int[] getIndices (Vector3 position)
    {
        position -= origin;
        return new int[3]
        {
            Mathf.FloorToInt((position.x / Navfield.CELL_SIZE) + (Navfield.SIZE * 0.5f)),
            Mathf.FloorToInt((position.y / Navfield.CELL_SIZE) + (Navfield.SIZE * 0.5f)),
            Mathf.FloorToInt((position.z / Navfield.CELL_SIZE) + (Navfield.SIZE * 0.5f))
        };
    }

    public void updateRemaingTime(float dt)
    {
        time += dt;
    }

    public float getRemainingTime()
    {
        return duration - time;
    }

    public bool isInside(Vector3 pos)
    {
        if (pos.x > origin.x - (Navfield.SIZE * 0.5f * Navfield.CELL_SIZE)
            && pos.x < origin.x + (Navfield.SIZE * 0.5f * Navfield.CELL_SIZE)
            && pos.y > origin.y - (Navfield.SIZE * 0.5f * Navfield.CELL_SIZE)
            && pos.y < origin.y + (Navfield.SIZE * 0.5f * Navfield.CELL_SIZE)
            && pos.z > origin.z - (Navfield.SIZE * 0.5f * Navfield.CELL_SIZE)
            && pos.z < origin.z + (Navfield.SIZE * 0.5f * Navfield.CELL_SIZE))
        {
            return true;
        }
        return false;
    }

    public Vector3 getForce(Vector3 position)
    {
        int[] indices = getIndices(position);
        return forces[indices[0],indices[1],indices[2]];
    }
}
