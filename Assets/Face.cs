using UnityEngine;
using System.Collections;

public class Face : MonoBehaviour {

	protected Hull parentHull;
	private float smoothTime = 0.1f;
	private Vector3 smoothVelocity = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}

	public virtual void Initialise(Hull parentHull, float size){
		this.parentHull = parentHull;
	//	this.transform.localScale = new Vector3 (size, size, 0.1f);
	}

	//Ask the parent hull to correctly set the world position
	//and rotation of this face.
	public void PlaceFace(){
		Vector3 targetPosition = parentHull.GetWorldPosition ();
		Quaternion targetRotation = parentHull.GetWorldRotation ();


		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothVelocity, smoothTime);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * 30f);
		transform.rotation = Quaternion.LookRotation (Vector3.forward, this.transform.up);
	}

	public void SetFaceRotation(Quaternion rot){
		transform.rotation = rot;
	}

	public Hull GetParentHull(){
		return parentHull;
	}

	// Update is called once per frame
	void Update () {
		PlaceFace ();
	}
}
