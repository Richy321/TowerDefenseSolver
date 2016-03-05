using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public float speed = 10;
    public float health = 10;
    public List<Vector3> path;
    private int targetIndex;
    public EnemyType type;
    public EnemyTargettingType targettingType;

    public Action<GameObject> onReachedEnd;
    public Action<GameObject> onDied;

    public int resourceReward = 2;
    public int lifeDeduction = 1;

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
        while (isActiveAndEnabled)
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
            
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    void Stop()
    {
        if (isActiveAndEnabled && onReachedEnd != null)
            onReachedEnd(gameObject);
    }

    /// <summary>
    /// Standard application of damage
    /// </summary>
    /// <param name="amount">Damage to Take</param>
    /// <returns>Returns false on death</returns>
    public bool TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            if (onDied != null)
                onDied(gameObject);
            return false;
        }
        return true;
    }

    public void Reset()
    {
        StopCoroutine("FollowPath");
        targetIndex = 0;
    }
}
