using UnityEngine;
using System.Collections;

public class Undestroyable : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}