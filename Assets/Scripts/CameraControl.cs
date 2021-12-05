using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	public GameObject mainCamera;
    public float cameraVelocity = 5;
    public float rotateVelocity = 5;

    public float maxCameraVelocity = 50;
    public float maxRotateVelocity = 12;

    public float minZoom = 1;
    public float maxZoom = 20;

    public float zoomSpeed = 5;
    
	public float smoothTime = 0.06f;
	private Vector3 scrollVelocity = Vector3.zero;

    private float screenWidth;
    private float screenHeight;

    private int intBoundary = 0;

    // Start is called before the first frame update
    void Start()
    {
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		mainCamera.transform.LookAt(gameObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
		// Height Scroll Down
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (mainCamera.transform.localPosition.y > minZoom)
			{
				Vector3 temp = mainCamera.transform.localPosition;
				temp.y = Mathf.Clamp(mainCamera.transform.localPosition.y-zoomSpeed, minZoom, maxZoom);
				temp.z = Mathf.Clamp(mainCamera.transform.localPosition.z+(zoomSpeed/3f), -20f, -6f);
				Vector3 cameraTarget = temp;
				mainCamera.transform.localPosition = Vector3.SmoothDamp(mainCamera.transform.localPosition, cameraTarget, ref scrollVelocity, smoothTime);
				mainCamera.transform.LookAt(gameObject.transform.position);
			}

		}
		// Height Scroll Up
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			if (mainCamera.transform.localPosition.y < maxZoom)
			{
				Vector3 temp = mainCamera.transform.localPosition;
				temp.y = Mathf.Clamp(mainCamera.transform.localPosition.y+zoomSpeed, minZoom, maxZoom);
				temp.z = Mathf.Clamp(mainCamera.transform.localPosition.z-(zoomSpeed*3f), -20f, -6f);
				Vector3 cameraTarget = temp;
				mainCamera.transform.localPosition = Vector3.SmoothDamp(mainCamera.transform.localPosition, cameraTarget, ref scrollVelocity, smoothTime);
				mainCamera.transform.LookAt(gameObject.transform.position);
			}
		}

        
    }

    private void FixedUpdate() {
        // Camera side movement
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x < screenWidth*0.01f + intBoundary)
		{
			GetComponent<Rigidbody>().AddForce(-transform.right*cameraVelocity);
		}
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y > screenHeight*0.99f - intBoundary)
		{
			GetComponent<Rigidbody>().AddForce(transform.forward*cameraVelocity);
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y < screenHeight*0.01f + intBoundary)
		{
			GetComponent<Rigidbody>().AddForce(-transform.forward*cameraVelocity);
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x > screenWidth*0.99f - intBoundary)
		{
			GetComponent<Rigidbody>().AddForce(transform.right*cameraVelocity);
		}
		if (Input.GetKey(KeyCode.Q))
		{
			GetComponent<Rigidbody>().AddTorque(-transform.up*rotateVelocity);
		}
		if (Input.GetKey(KeyCode.E))
		{
			GetComponent<Rigidbody>().AddTorque(transform.up*rotateVelocity);
		}
		gameObject.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(gameObject.GetComponent<Rigidbody>().velocity, maxCameraVelocity);
		gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.ClampMagnitude(gameObject.GetComponent<Rigidbody>().angularVelocity, maxRotateVelocity);
    }
}
