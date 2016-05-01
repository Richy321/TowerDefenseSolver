using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    private float camMoveSpeed = 10.0f;
    private float zoomSpd = 10.0f;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    Vector3 move = new Vector3();

        if(Input.GetKey(KeyCode.W))
	        move.y += camMoveSpeed * Time.unscaledDeltaTime;

        if (Input.GetKey(KeyCode.S))
            move.y -= camMoveSpeed * Time.unscaledDeltaTime;

        if (Input.GetKey(KeyCode.A))
            move.x -= camMoveSpeed * Time.unscaledDeltaTime;

        if (Input.GetKey(KeyCode.D))
            move.x += camMoveSpeed * Time.unscaledDeltaTime;

        if (Input.GetKey(KeyCode.Q))
            move.z += zoomSpd * Time.unscaledDeltaTime;

        if (Input.GetKey(KeyCode.E))
            move.z -= zoomSpd * Time.unscaledDeltaTime;
        /*
        float xAxisValue = Input.GetAxis("Horizontal");//* camMoveSpeed * Time.unscaledDeltaTime;
	    float yAxisValue = Input.GetAxis("Vertical");// * camMoveSpeed * Time.unscaledDeltaTime;
	    float zAxisValue = Input.GetAxis("Zoom");// * camMoveSpeed * Time.unscaledDeltaTime;
        Debug.Log("cam move:" + xAxisValue + "," + yAxisValue + "," + zAxisValue);
        if (Camera.current != null)
        {
            Camera.current.transform.Translate(new Vector3(xAxisValue, yAxisValue, zAxisValue));
        }
        */
        Camera.main.transform.Translate(move);

    }
}
