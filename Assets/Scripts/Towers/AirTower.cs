using UnityEngine;
using System.Collections;

public class AirTower : BaseTower 
{
	public Transform tiltObject;
	public Transform aim_Tilt;
	public Transform aim_Pan;

	// Use this for initialization
	override public void Start () 
	{
		
	}
	
	// Update is called once per frame
	override public void Update () 
	{
	    if (target)
        {

			
			/*
			Quaternion orig;
			Quaternion aim;
			
			orig = turretBall.rotation;
			turretBall.transform.LookAt(target);
			turretBall.transform.eulerAngles = new Vector3(0, aim_Pan.eulerAngles.y, 0);
			aim = turretBall.transform.rotation;
			turretBall.transform.rotation = orig;
			turretBall.rotation = Quaternion.Lerp(turretBall.rotation, aim, Time.deltaTime * angularVelocity);
			
			orig = tiltObject.rotation;
			tiltObject.transform.LookAt(target);
			//tiltObject.transform.eulerAngles = new Vector3(0, aim_Pan.eulerAngles.y, 0);
			aim = tiltObject.transform.rotation;
			tiltObject.transform.rotation = orig;
			tiltObject.rotation = Quaternion.Lerp(tiltObject.rotation, aim, Time.deltaTime * angularVelocity);
			*/
			
			if (Time.time >= nextMoveTime)
            {
			aim_Pan.LookAt(target);
			aim_Pan.eulerAngles = new Vector3(0, aim_Pan.eulerAngles.y, 0);
			aim_Tilt.LookAt(target);
			turretBall.rotation = Quaternion.Lerp(turretBall.rotation, aim_Pan.rotation, Time.deltaTime * angularVelocity);
			tiltObject.rotation = Quaternion.Lerp(tiltObject.rotation, aim_Tilt.rotation, Time.deltaTime * angularVelocity);
			
			

				/*
                
				//Vector3 aimPoint = new Vector3(target.position.x + aimError, target.position.y + aimError, target.position.z + aimError);
				
				
				//TODO - find better way to do with without 2x rotations.
				Quaternion origRot = turretBall.rotation;
				turretBall.LookAt(aimPoint);
				Quaternion newRot = turretBall.rotation;
				turretBall.rotation = origRot;
				
				//Y rotation only
				turretBall.rotation = Quaternion.Lerp(origRot, newRot, Time.deltaTime * angularVelocity); 
				//turretBall.rotation.Set(0.0f, turretBall.rotation.y, 0.0f, turretBall.rotation.w);
				turretBall.transform.eulerAngles = new Vector3(0, turretBall.transform.eulerAngles.y, 0);
				
				
				//X rotation only
				tiltObject.rotation = Quaternion.Lerp(origRot, newRot, Time.deltaTime * angularVelocity); 
				//tiltObject.rotation.Set(turretBall.rotation.x, 0.0f, 0.0f, turretBall.rotation.w);
				tiltObject.transform.eulerAngles = new Vector3(tiltObject.transform.eulerAngles.x, 0,0);
				*/
            }

            if (Time.time >= nextFireTime)
            {
                FireProjectile();
            }	
        } 
	}
	
	protected override void FireProjectile ()
	{
		if(GetComponent<AudioSource>())
        	GetComponent<AudioSource>().Play();
        nextFireTime = Time.time + rateOfFire;
        nextMoveTime = Time.time + firePauseTime;
        CalculateAimError();
		
		int rndMuz = Random.Range(0,6);
        
        
		if(projectile)
		{
        	GameObject missile = Instantiate(projectile, muzzlePositions[rndMuz].position, muzzlePositions[rndMuz].rotation) as GameObject;
			missile.GetComponentInChildren<ProjectileMissile>().target = target;
			//("SetTarget", target, SendMessageOptions.RequireReceiver);
		}
		
		if(muzzleEffect)
        	Instantiate(muzzleEffect, muzzlePositions[rndMuz].position, muzzlePositions[rndMuz].rotation);
        
		
	}
	protected override void OnTriggerStay (Collider other)
	{
		if(!target)
		{
	        if (other.gameObject.tag == "AirEnemy")
	        {
	            nextFireTime = Time.time + rateOfFire *0.5f;
	            target = other.gameObject.transform; //can set this to other.transform?
	        }
		}
	}
	
}
