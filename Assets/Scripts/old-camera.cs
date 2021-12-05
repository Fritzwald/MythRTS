using UnityEngine;
using System.Collections;
using System.Reflection;

public class CameraMovement : MonoBehaviour {

	public float cameraVelocity;
	public float maxVelocity;
	public float rotateVelocity;
	public float maxRotateVelocity;
	public float zoomSpeed;
	public float smoothTime;
	public int maxZoom;
	public int minZoom;
	public GameObject mainCamera;
	private int intBoundary = 1;
	private int screenWidth;
	private int screenHeight;
	private Vector3 scrollVelocity;

	void Start() {
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		mainCamera.transform.LookAt(gameObject.transform.position);
	}

	void Update() {
		// Height Scroll Down
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (mainCamera.transform.localPosition.y > minZoom)
			{
				Vector3 scrollVelocity = Vector3.zero;
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

	/* Old Height Scroll
	void Update() {
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			Vector3 temp = mainCamera.transform.localPosition;
			temp.y = Mathf.Clamp(mainCamera.transform.localPosition.y-zoomSpeed, minZoom, maxZoom);
			temp.z = Mathf.Clamp(mainCamera.transform.localPosition.z+(zoomSpeed/2.5f), -6f, -2f);
			mainCamera.transform.localPosition = temp;
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			Vector3 temp = mainCamera.transform.localPosition;
			temp.y = Mathf.Clamp(mainCamera.transform.localPosition.y+zoomSpeed, minZoom, maxZoom);
			temp.z = Mathf.Clamp(mainCamera.transform.localPosition.z-(zoomSpeed/2.5f), -6f, -2f);
			mainCamera.transform.localPosition = temp;
		}
	}
	*/

	void FixedUpdate() {
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
		gameObject.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(gameObject.GetComponent<Rigidbody>().velocity, maxVelocity);
		gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.ClampMagnitude(gameObject.GetComponent<Rigidbody>().angularVelocity, maxRotateVelocity);
	}
}
