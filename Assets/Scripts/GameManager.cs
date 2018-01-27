using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    public bool debugBoids;
    public bool debugNavfields;


    private void Awake()
    {
       
    }
    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}
}
