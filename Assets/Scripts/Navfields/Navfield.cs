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

    private NavfieldManager manager;


    public Navfield(NavfieldManager manager, Flock flock, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        Create(manager, flock.getBarycenter(), orientation, primitive, duration);
    }

    public Navfield(NavfieldManager manager, Vector3 position, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        Create(manager, position, orientation, primitive, duration);
    }

    private void Create(NavfieldManager manager, Vector3 position, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        this.manager = manager;
        origin = position;
        rotation = orientation;
        forces = new Vector3[manager.size, manager.size, manager.size];
        this.duration = duration;
        time = 0f;

        switch (primitive)
        {
            case NavFieldPrimitives.dispersal:
                for (int i = 0; i < manager.size; i++)
                {
                    for (int j = 0; j < manager.size; j++)
                    {
                        for (int k = 0; k < manager.size; k++)
                        {
                            forces[i, j, k] = manager.force * new Vector3((i - (manager.size * 0.5f)) / (manager.size * 0.5f), (j - (manager.size * 0.5f)) / (manager.size * 0.5f), (k - (manager.size * 0.5f)) / (manager.size * 0.5f));
                        }
                    }
                }
                break;
            case NavFieldPrimitives.gathering:
                for (int i = 0; i < manager.size; i++)
                {
                    for (int j = 0; j < manager.size; j++)
                    {
                        for (int k = 0; k < manager.size; k++)
                        {
                            forces[i, j, k] = manager.force * -new Vector3((i - (manager.size * 0.5f)) / (manager.size * 0.5f), (j - (manager.size * 0.5f)) / (manager.size * 0.5f), (k - (manager.size * 0.5f)) / (manager.size * 0.5f));
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
            Mathf.FloorToInt((position.x / manager.cellSize) + (manager.size * 0.5f)),
            Mathf.FloorToInt((position.y / manager.cellSize) + (manager.size * 0.5f)),
            Mathf.FloorToInt((position.z / manager.cellSize) + (manager.size * 0.5f))
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
        if (pos.x > origin.x - (manager.size * 0.5f * manager.cellSize)
            && pos.x < origin.x + (manager.size * 0.5f * manager.cellSize)
            && pos.y > origin.y - (manager.size * 0.5f * manager.cellSize)
            && pos.y < origin.y + (manager.size * 0.5f * manager.cellSize)
            && pos.z > origin.z - (manager.size * 0.5f * manager.cellSize)
            && pos.z < origin.z + (manager.size * 0.5f * manager.cellSize))
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
