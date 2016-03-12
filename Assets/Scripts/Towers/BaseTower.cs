using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using Random = UnityEngine.Random;

public class BaseTower : MonoBehaviour 
{
    public GameObject projectile;
    public GameObject muzzleEffect;
    public Transform target;
    public Transform[] muzzlePositions;
    public Transform turretBall;

    public float rateOfFire = 1f;
    public float angularVelocity = 10f;
    public float errorAmount = 0.001f;
    public float firePauseTime = 0.25f;
    public bool applyInstantDamage = true;
    public float instantDamage = 5f;

    public int resourceCost = 10;

    protected float nextFireTime;
    protected float nextMoveTime;
    protected float aimError;

    public SphereCollider rangeCollider;

    public SphereCollider RangeCollider
    {
        get
        {
            if (!rangeCollider)
                rangeCollider = GetComponent<SphereCollider>();
            return rangeCollider;
        }
        set
        {
            if (!rangeCollider)
                rangeCollider = GetComponent<SphereCollider>();
            rangeCollider = value;
        }
    }

    public List<EnemyTargettingType> targettingTypes;

    [ExposeProperty]
    public float Range
    {
        get { return RangeCollider.radius; }
        set { RangeCollider.radius = value; }
    }

    public TowerType type = TowerType.SingleDamage;


	// Use this for initialization
	public virtual void Start ()
	{
	}
	
	// Update is called once per frame
	public virtual void Update ()
    {
        if (target && target.gameObject.activeInHierarchy)
        {
            if (Time.time >= nextMoveTime)
            {
                
				Vector3 aimPoint = new Vector3(target.position.x + aimError, target.position.y + aimError, target.position.z + aimError);
				
				//TODO - find better way to do with without 2x rotations.
				Quaternion origRot = turretBall.rotation;
				turretBall.LookAt(aimPoint);
				Quaternion newRot = turretBall.rotation;
				turretBall.rotation = origRot;
				

				turretBall.rotation = Quaternion.Lerp(origRot, newRot, Time.deltaTime * angularVelocity); 
				//turretBall.rotation = Quaternion.Lerp(turretBall.rotation, desiredRotation, Time.deltaTime * angularVelocity);
				Debug.DrawLine(turretBall.position, target.position);	

            }

            if (Time.time >= nextFireTime)
            {
                FireProjectile();
                if (applyInstantDamage)
                    ApplyInstantDamageToTarget();

            }
        }
	}

    protected virtual void OnTriggerStay(Collider other)
    {
		if(!target || !target.gameObject.activeInHierarchy)
		{
            Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
            if(enemy && targettingTypes.Contains(enemy.targettingType))
	        {
	            target = other.gameObject.transform;
	        }
		}
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform == target)
        {
            target = null;
        }
    }

    protected virtual void FireProjectile()
    {
		if(GetComponent<AudioSource>())
        	GetComponent<AudioSource>().Play();
        nextFireTime = Time.time + rateOfFire;
        nextMoveTime = Time.time + firePauseTime;
        CalculateAimError();

        foreach (Transform muz in muzzlePositions)
        {
			if(projectile)
            	Instantiate(projectile, muz.position, muz.rotation);
			if(muzzleEffect)
            	Instantiate(muzzleEffect, muz.position, muz.rotation);
        }
    }

    protected virtual void ApplyInstantDamageToTarget()
    {
        Enemy enemy = target.gameObject.GetComponentInParent<Enemy>();
        if (!enemy.TakeDamage(instantDamage))
            target = null;
    }


    protected Quaternion CalculateAimPosition(Vector3 targetPos)
    {
        Vector3 aimPoint = new Vector3(targetPos.x + aimError, targetPos.y + aimError, targetPos.z + aimError);
		return Quaternion.LookRotation(aimPoint);
    }

    protected void CalculateAimError()
    {
        aimError = Random.Range(-errorAmount, errorAmount);
    }

}
