using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{

    public float zoomSpd = 2.0f;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float yAxisValue = Input.GetAxis("Vertical");
	    float zAxisValue = Input.GetAxis("Zoom");

        if (Camera.current != null)
        {
            Camera.current.transform.Translate(new Vector3(xAxisValue, yAxisValue, zAxisValue));
        }
    }
}
