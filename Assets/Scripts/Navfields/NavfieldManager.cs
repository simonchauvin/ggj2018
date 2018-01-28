using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavfieldManager : MonoBehaviour
{
    public Transform debugPrefab;
    public int minSize = 32;
    public float minCellSize = 0.5f;
    public float force = 15f;

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
                        debugNavfields[i].gameObject.SetActive(false);
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
            debugNavfields[debugNavfields.Count - 1].position = navfields[navfields.Count - 1].origin;
            debugNavfields[debugNavfields.Count - 1].localScale = new Vector3(navfields[navfields.Count - 1].size * navfields[navfields.Count - 1].cellSize, navfields[navfields.Count - 1].size * navfields[navfields.Count - 1].cellSize, navfields[navfields.Count - 1].size * navfields[navfields.Count - 1].cellSize);
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
