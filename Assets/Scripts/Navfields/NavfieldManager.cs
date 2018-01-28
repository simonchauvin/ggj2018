using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavfieldManager : MonoBehaviour
{
    public Transform debugPrefab;
    public int size;
    public float cellSize;
    public float force;

    private List<Navfield> navfields;

    // Debug
    private List<Transform> debugNavfields;


    private void Awake()
    {
        navfields = new List<Navfield>();
        if (GameManager.instance.debugNavfields)
        {
            debugNavfields = new List<Transform>();
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
        for (int i = 0; i < navfields.Count; i++)
        {
            if (navfields[i] != null)
            {
                if (GameManager.instance.debugNavfields)
                {
                    debugNavfields[i].gameObject.SetActive(true);
                }
                
                navfields[i].updateRemaingTime(Time.deltaTime);
                if (navfields[i].getRemainingTime() <= 0f)
                {
                    navfields.RemoveAt(i);
                    if (GameManager.instance.debugNavfields)
                    {
                        debugNavfields.RemoveAt(i);
                    }
                }
            }
        }
    }

    public void addNavfield(Flock flock, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        navfields.Add(new Navfield(this, flock, orientation, primitive, duration));
        if (GameManager.instance.debugNavfields)
        {
            debugNavfields.Add(Instantiate<Transform>(debugPrefab));
            debugNavfields[debugNavfields.Count - 1].position = flock.getLeader().transform.position;
            debugNavfields[debugNavfields.Count - 1].localScale = new Vector3(size, size, size);
        }
    }

    public void addNavfield(Boid leader, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        navfields.Add(new Navfield(this, leader, orientation, primitive, duration));
        if (GameManager.instance.debugNavfields)
        {
            debugNavfields.Add(Instantiate<Transform>(debugPrefab));
            debugNavfields[debugNavfields.Count - 1].localScale = new Vector3(size, size, size);
        }
    }

    public void addNavfield(Vector3 position, Quaternion orientation, NavFieldPrimitives primitive, float duration)
    {
        navfields.Add(new Navfield(this, position, orientation, primitive, duration));
        if (GameManager.instance.debugNavfields)
        {
            debugNavfields.Add(Instantiate<Transform>(debugPrefab));
            debugNavfields[debugNavfields.Count - 1].localScale = new Vector3(size, size, size);
        }
    }

    public Navfield getNavfield(Vector3 position)
    {
        for (int i = 0; i < navfields.Count; i++)
        {
            if (navfields[i] != null && navfields[i].isInside(position))
            {
                return navfields[i];
            }
        }
        return null;
    }
}
