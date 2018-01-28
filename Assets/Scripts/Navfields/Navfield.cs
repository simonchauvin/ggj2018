using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navfield
{
    public Vector3 origin;
    public Quaternion rotation;
    private Vector3[,,] forces;
    private float duration;
    private float time;
    public int size { get; private set; }
    public float cellSize { get; private set; }

    private NavfieldManager manager;


    public Navfield(NavfieldManager manager, Flock flock, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        Create(manager, flock, orientation, primitive, duration);
    }

    private void Create(NavfieldManager manager, Flock flock, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        this.manager = manager;
        origin = flock.getBarycenter();
        rotation = orientation;
        size = manager.minSize;
        cellSize = manager.minCellSize;

        int count = 0;
        for (int i = 0; i < flock.numberOfBoids; i++)
        {
            if (isInside(flock.boids[i].transform.position))
            {
                count++;
            }
        }

        float ratio = (float)count / (float)flock.numberOfBoids,
            minRatio = (manager.minSize * manager.minCellSize) / flock.boundRadius;
        if (ratio >= minRatio)
        {
            cellSize = (float)manager.minCellSize / ratio;
        }
        else
        {
            cellSize = (float)manager.minCellSize / minRatio;
        }

        forces = new Vector3[size, size, size];
        this.duration = duration;
        time = 0f;

        switch (primitive)
        {
            case NavFieldPrimitives.dispersal:
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        for (int k = 0; k < size; k++)
                        {
                            forces[i, j, k] = manager.force * new Vector3((i - (size * 0.5f)) / (size * 0.5f), (j - (size * 0.5f)) / (size * 0.5f), (k - (size * 0.5f)) / (size * 0.5f));
                        }
                    }
                }
                break;
            case NavFieldPrimitives.gathering:
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        for (int k = 0; k < size; k++)
                        {
                            forces[i, j, k] = manager.force * -new Vector3((i - (size * 0.5f)) / (size * 0.5f), (j - (size * 0.5f)) / (size * 0.5f), (k - (size * 0.5f)) / (size * 0.5f));
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
            Mathf.FloorToInt((position.x / cellSize) + (size * 0.5f)),
            Mathf.FloorToInt((position.y / cellSize) + (size * 0.5f)),
            Mathf.FloorToInt((position.z / cellSize) + (size * 0.5f))
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
        if (pos.x > origin.x - (size * 0.5f * cellSize)
            && pos.x < origin.x + (size * 0.5f * cellSize)
            && pos.y > origin.y - (size * 0.5f * cellSize)
            && pos.y < origin.y + (size * 0.5f * cellSize)
            && pos.z > origin.z - (size * 0.5f * cellSize)
            && pos.z < origin.z + (size * 0.5f * cellSize))
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
