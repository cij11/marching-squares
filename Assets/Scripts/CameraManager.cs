using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	private GameObject renderFocus = null;
	private float smoothTime = 0.05f;
	private Vector3 smoothVelocity = Vector3.zero;

	private float zoom = 50f;
	private float zoomIncrement = 0.05f;
	private float maxZoomOut = 250f;
	private float maxZoomIn = 30f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float trackingSpeed = 5.0f;
		if (renderFocus == null) {
			renderFocus = GameObject.FindGameObjectWithTag ("RenderFocus");
		} else {
			Vector3 targetPosition = renderFocus.transform.position - new Vector3 (0f, 0f, zoom);
			Quaternion targetRotation = renderFocus.transform.rotation;
			this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, Time.deltaTime * trackingSpeed);
			this.transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothVelocity, smoothTime);
		}

		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			if (zoom > maxZoomIn) {
				zoom -= (zoomIncrement * zoom);
				Camera.main.orthographicSize = zoom;
			}
		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			if (zoom < maxZoomOut) {
				zoom += (zoomIncrement * zoom);
				Camera.main.orthographicSize = zoom;
			}
		}
	}
}
