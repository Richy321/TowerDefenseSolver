using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public float speed = 5;
    public int health = 10;
    public List<Vector3> path;
    private int targetIndex;
    public EnemyType type;

    public Action<GameObject> onReachedEnd; 

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Go()
    {
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Count)
                {
                    Stop();
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed);
            yield return null;
        }
    }

    void Stop()
    {
        if (onReachedEnd != null)
            onReachedEnd(gameObject);
    }


}
