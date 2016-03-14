using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Enemy : MonoBehaviour
{
    public float startSpeed = 1;
    public float speed = 1;
    public float waveStartSpeed = 1;

    public float startHealth = 15;
    public float health = 15;

    public List<Vector3> path;
    private int targetIndex;
    public EnemyType type;
    public EnemyTargettingType targettingType;

    public Action<GameObject> onReachedEnd;
    public Action<GameObject> onDied;

    public int startResourceReward = 2;
    public int resourceReward = 2;

    public int lifeDeduction = 1;

    public Animator anim;
    private int isDeadHash = Animator.StringToHash("IsDead");
    private int speedHash = Animator.StringToHash("Speed");

    private float angularVelocity = 10.0f;

    public enum State
    {
        Alive,
        Dying,
        Dead
    }

    public State state = State.Alive;

    // Use this for initialization
    void Start()
    {
        if (!anim)
            anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Go()
    {
        state = State.Alive;
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
            Vector3 dir = currentWaypoint - transform.position;
            Quaternion facingDir = Quaternion.LookRotation(dir);
            transform.rotation = facingDir;//Quaternion.Slerp(transform.rotation, facingDir, angularVelocity * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed*Time.deltaTime);
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
            if (state == State.Alive)
            {
                state = State.Dying;
                StopCoroutine("FollowPath");
                if (anim)
                    StartCoroutine("WaitForDeathAnimationComplete");
                else
                {
                    if (onDied != null)
                        onDied(gameObject);
                    else
                        gameObject.transform.position = ObjectPool.offscreenHoldingPoint;

                    state = State.Dead;
                }
            }
            return false;
        }
        return true;
    }

    public void Slow(float amount)
    {
        speed = waveStartSpeed*(1.0f - amount);
    }

    public void UnSlow()
    {
        speed = waveStartSpeed;
    }

    public void Reset()
    {
        state = State.Alive;
        if (anim)
        {
            anim.SetBool(isDeadHash, false);
            anim.SetFloat(speedHash, speed);
        }
        StopCoroutine("FollowPath");
        targetIndex = 0;
        health = startHealth;
        speed = startSpeed;
        resourceReward = startResourceReward;
    }

    IEnumerator WaitForDeathAnimationComplete()
    {
        Debug.Log("WaitForDeathAnimationComplete");
        anim.SetBool(isDeadHash, true);
        yield return null;
        Debug.Log("wait for next frame complete");

        yield return new WaitForSeconds(3.0f); //GetAnimationState and Clip info do not seem to be returning correct values, fix this to 3.of for time being
        Debug.Log("Death Animation complete!");

        if (onDied != null)
            onDied(gameObject);
        else
            gameObject.transform.position = ObjectPool.offscreenHoldingPoint;

        state = State.Dead;
    }
}
