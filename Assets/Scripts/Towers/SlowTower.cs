using UnityEngine;
using System.Collections;

public class SlowTower : BaseTower
{
    public float slowPercentage = 0.1f;

	void Start () 
	{
	    
	}

    public override void Update () 
	{
	    //do nothing, triggers will handle functionality
	}

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
        if (enemy.isActiveAndEnabled && enemy && targettingTypes.Contains(enemy.targettingType))
        {
            enemy.Slow(slowPercentage);
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
        if (enemy.isActiveAndEnabled && enemy && targettingTypes.Contains(enemy.targettingType))
        {
            enemy.Slow(slowPercentage);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
        if (enemy.isActiveAndEnabled && enemy && targettingTypes.Contains(enemy.targettingType))
        {
            enemy.UnSlow();
        }
    }
}
