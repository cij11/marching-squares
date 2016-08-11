using UnityEngine;
using System.Collections;

public class Unit : Hull {

	public override void Initialise(float size){
		body = GetComponent<Rigidbody> () as Rigidbody;

		this.hullWorldPosition = this.body.position;
		this.hullWorldRotation = this.body.rotation;

		GameObject faceGO = Instantiate (facePrefab, this.transform.position, Quaternion.identity) as GameObject;
		face = faceGO.GetComponent<Face> () as Face;
		face.Initialise (this, this.size);

		GameObject envelopeGO = Instantiate (envelopePrefab, this.body.position, Quaternion.identity) as GameObject;
		envelope = envelopeGO.GetComponent<Envelope> () as Envelope;
		envelope.Initialise (this, this.size);
	}

	private void OrientToWorld(){
		//Get a vector to point feet at. This might be the center of a hull for a round world, or down for a ship
		//with artificial gravity. Give this body position as arguement to calculate down or radial orientation position
		Vector3 pointToOrientTo = mount.GetPassengerFloor(body.position);

		Vector3 vectorToMountCenter = pointToOrientTo - body.position;
		vectorToMountCenter.Normalize ();
		relativeDown = vectorToMountCenter;

	//	Vector3 rightAngleToDown = new Vector3 (relativeDown.y, -relativeDown.x, body.position.z);

		Vector3 lookAtVector = this.body.position + new Vector3 (0f, 0f, 1f);

		//Look at a spot immediately left of the unit, then look intot the scene.
	//	body.transform.LookAt (lookAtVector, -relativeDown);
		body.transform.rotation = Quaternion.LookRotation (Vector3.forward, -relativeDown);

		//Trying with fromto rotations.
	//	Quaternion referentialDownShift = Quaternion.FromToRotation(-body.transform.up, relativeDown); 
	//	body.rotation = referentialDownShift;

	//	Quaternion referentialInShift = Quaternion.FromToRotation(body.transform.forward, new Vector3(0f, 0f, 1f)); 
	//	body.rotation = referentialInShift;
	}


	public override void MountHull(Hull hitHull){
		//Store a reference to the object embarked upon
		mount = hitHull;	
		GameObject mountGO = hitHull.gameObject;// collision.gameObject;

		//Find the position of the passenger relative to the hull in the hull's space
		//Vector3 relativePosition = body.position - collision.rigidbody.position;
		Vector3 relativePosition = mount.GetBody().transform.InverseTransformPoint(body.position);
		hullLocationZLayer = mount.GetZLayer ();
		//relativePosition.z = destinationZLayer;

		//Unlock the zconstraint, move the player, then lock the zconstraint.
		//	body.constraints &= ~RigidbodyConstraints.FreezePositionZ;
		relativePosition = new Vector3(relativePosition.x, relativePosition.y, hullLocationZLayer + 0.5f);
		//If immediately reset constraints, will be unable to change the z coordinate. Reapply after a delay.
		//	Invoke("EnableZConstraint", 0.01f);

		body.position = relativePosition;
		//	this.transform.position = new Vector3(0f, 0f, 0f);
		//Convert the inbound velocity into local velocity
		Vector3 relativeVelocity = mount.GetBody().transform.InverseTransformVector(body.velocity);
		body.velocity = relativeVelocity;

		OrientToWorld ();
	//	CalculateTransform();


		//Place in the interior layer
		this.gameObject.layer = 8;
	}

	// Update is called once per frame
	void Update () {
		HullUpdate ();

		float movementForce = 16.0f;
		float rotationTorque = 0.25f;

		//If mounted, ground controls
		if (mount != null) {
			if (Input.GetKey ("w"))
				body.AddRelativeForce (Vector3.up * movementForce);
			if (Input.GetKey ("s"))
				body.AddRelativeForce (Vector3.down * movementForce);
			if (Input.GetKey ("a"))
				body.AddRelativeForce (Vector3.left * movementForce);
			if (Input.GetKey ("d"))
				body.AddRelativeForce (Vector3.right * movementForce);
		} else  {	//Else relative/plane controls
			if (Input.GetKey ("w"))
				body.AddRelativeForce (Vector3.up * movementForce);
			if (Input.GetKey ("s"))
				body.AddRelativeForce (Vector3.down * movementForce);
			if (Input.GetKey ("a"))
				body.AddTorque (new Vector3 (0f, 0f, rotationTorque));
			if (Input.GetKey ("d"))
				body.AddTorque (new Vector3 (0f, 0f, -rotationTorque));
		}
			
		//If mounted, steer the mount
		if (mount != null) {
			Rigidbody mountBody = mount.GetComponent<Rigidbody> () as Rigidbody;
			float mountThrust = 100f;

			if (Input.GetKey ("i"))
				mountBody.AddRelativeForce (Vector3.up * movementForce);
			if (Input.GetKey ("k"))
				mountBody.AddRelativeForce (Vector3.down * movementForce);
			if (Input.GetKey ("j"))
				mountBody.AddTorque(new Vector3(0f, 0f, mountThrust));
			if (Input.GetKey ("l"))
				mountBody.AddTorque(new Vector3(0f, 0f, -mountThrust));
		}

		CalculateTransform ();
		//If mounted, units feet face the ground

		if (mount != null) {
			//Face the unit's feet towards the local gravity focus
			OrientToWorld ();
		}
	}

	void OnCollisionEnter(Collision collision) {
		this.body.angularVelocity.Set (0f, 0f, 0f);
	}
}
