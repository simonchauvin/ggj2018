using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavfieldManager : MonoBehaviour
{
    private List<Navfield> navfields;


    private void Awake()
    {
        navfields = new List<Navfield>();
        navfields.Add(new Navfield(Vector3.zero, Quaternion.identity, NavFieldPrimitives.gathering, 50f));
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
                navfields[i].updateRemaingTime(Time.deltaTime);
                if (navfields[i].getRemainingTime() <= 0f)
                {
                    navfields.RemoveAt(i);
                }
            }
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
