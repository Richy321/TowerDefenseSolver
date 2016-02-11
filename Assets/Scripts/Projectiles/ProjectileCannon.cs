using UnityEngine;

public class ProjectileCannon : MonoBehaviour 
{
    public float speed = 10;
    public float range = 10;
    public  BaseTower owner;

    private float distance;
	// Update is called once per frame

	void Update () 
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        distance += Time.deltaTime * speed;

        if (distance >= range)
            Destroy(gameObject);
	}
}
