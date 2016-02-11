using UnityEngine;

public class ProjectileMissile : MonoBehaviour 
{
	public GameObject explosion;
	public Transform target;
	public float speed = 10;
    public float range = 10;

	
	private float distance;
	
	public float damage = 25;
	public ParticleSystem particleSystem = null;
	 
	// Update is called once per frame
	void Update () 
	{
		transform.Translate(Vector3.forward * Time.deltaTime * speed);
        distance += Time.deltaTime * speed;

        if (distance >= range)
            Explode();
		
		if(target)
		{
			transform.LookAt(target); //follow target;
		}
		else
		{
			//explode if no target.
			//Could find new target. closest?
			Explode();
		}
	}
	
	
	void Explode()
	{
		if(particleSystem)
			particleSystem.Stop();
		if(explosion)
			Instantiate(explosion);
		Destroy(gameObject);
		//TODO apply damage to target;
	}
	
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "AirEnemy")
		{
			//if we hit something other than the target, set that to the target for dmg to be applied correctly
			//if(other.gameObject.transform != target)
			//	target = other.gameObject.transform;
			//other.gameObject.SendMessage(BaseEnemy.msgRcvFunctions.TakeDamage, SendMessageOptions.DontRequireReceiver);
			Explode();
		}
	}
	
	void SetTarget(Transform pTarget)
	{
		target = pTarget;	
	}
}
