using UnityEngine;
using System.Collections;

public class Hull : MonoBehaviour {
	protected enum GravityType{
		Center, Down, Radial
	}

	public GameObject facePrefab;
	public GameObject envelopePrefab;
	public bool mounted = false;

	protected Rigidbody body;
	protected Envelope envelope;
	protected Hull mount = null;
	protected Face face = null;

	protected Transform worldTransform;
	protected Vector3 hullWorldPosition;	//Use for determining where the face should be rendered
	protected Vector3 hullLocalPosition;
	protected Quaternion hullWorldRotation;	//Use for determine the rotation of the face
	protected Quaternion hullLocalRotation;

	protected LayerManager layerManager;

	protected float size = 1f;	//Two objects of similar size will bounce off each other. If one object is much smaller, it 
						//will board it.
	public float boardingRatio = 2.0f; //This object will board rather than collide with objects that are twice its size.
	public float interiorZLayer = 0f;
	public float hullLocationZLayer = 0f;

	protected GravityType gravityType = GravityType.Center;

	protected Vector3 relativeDown = Vector3.down;

//	protected int mountDismountFaceDelay = 3;	//Face teleporting prevented with distance check instead.
//	protected int mountDismountFaceTimer = 0;

	// Use this for initialization
	protected void Start () {

	}

	public virtual void Initialise(float size){
		body = GetComponent<Rigidbody> () as Rigidbody;

		this.hullWorldPosition = this.body.position;
		this.hullWorldRotation = this.body.rotation;

		GameObject faceGO = Instantiate (facePrefab, this.body.position, Quaternion.identity) as GameObject;
		face = faceGO.GetComponent<Face> () as Face;
		face.Initialise (this, this.size);

		GameObject envelopeGO = Instantiate (envelopePrefab, this.body.position, Quaternion.identity) as GameObject;
		envelope = envelopeGO.GetComponent<Envelope> () as Envelope;
		envelope.Initialise (this, this.size);

		layerManager = GameObject.FindGameObjectWithTag ("LayerManager").GetComponent<LayerManager> () as LayerManager;
		interiorZLayer = layerManager.GetLayer ();

		this.size = size;
		GetComponent<SphereCollider> ().radius = size / 2f;
	}



	//Calculate the world coordinate of this hull, given its relative posiion to its parent, the rotation of
	//its parent, an the world position of its parent.
	protected void CalculateTransform(){

		if (mount == null) {
			hullWorldPosition = this.body.position;
			hullWorldRotation = this.body.rotation;
		} 
		//Else this object is mounted. It's world coordinates are the distance from the center of its mount.
		//Its world rotation is equal to its mount's rotation plus it's own rotation.
		//It's world position is equal to its mounts world position, plus its mounts rotation * it's own world position.
		else {
			hullWorldPosition = mount.GetWorldPosition () + mount.GetWorldRotation () * GetInteriorPosition ();
			hullWorldRotation = mount.GetWorldRotation () * this.body.rotation;
		}
	}

	//Sets the world transform of the face to the appropriate position and rotation for this hull
	public void PlaceFace(){
		Vector3 proposedFacePosition = new Vector3 (0f, 0f, 0f);
		Vector3 faceCurrentPosition = face.transform.position;

		proposedFacePosition = hullWorldPosition;

		//Ignore large eronious positions.
	//	if (Vector3.Distance (proposedFacePosition, faceCurrentPosition) < 5f) {

		//Interpolate to prevent stuttering of the face
			face.transform.position = Vector3.Lerp(face.transform.position, hullWorldPosition, Time.deltaTime * 30f);
		face.transform.rotation = Quaternion.Lerp (face.transform.rotation, hullWorldRotation, Time.deltaTime * 30f);

		//Maybe a better solution would be to set the face as a child, just offset by some distance and rotation?
		//Or Vector3.SmoothDamp

	}

	protected Vector3 GetInteriorPosition(){
		return new Vector3 (this.body.position.x, this.body.position.y, 0f);
	}

	public float GetZLayer(){
		return this.interiorZLayer;
	}

	public Quaternion GetWorldRotation(){
		return hullWorldRotation;
	}

	public Vector3 GetWorldPosition(){
		return hullWorldPosition;
	}

	public void GetTransform(){


	}

	public float GetSize(){
		return this.size;
	}

	public Hull GetMount(){
		return this.mount;
	}

	public Vector3 GetPassengerFloor(Vector3 passengerPosition){
		switch (gravityType) {
		case GravityType.Center:
			return new Vector3 (0f, 0f, 0f);
		}
		return new Vector3 (0f, 0f, 0f);
	}

	//Can get some transient warped translations on the frame where move between layers. Do not update the
	//rendered face during this frame.
	protected void HullUpdate(){
		CalculateTransform ();

		//Dismount vessel if outside of hull
		if (mount != null) {
			mounted = true;

			//Emulate locking z position
			if (Mathf.Abs (body.position.z - (hullLocationZLayer + 0.5f)) > 0.1f)
				body.position = new Vector3(body.position.x, body.position.y, hullLocationZLayer + 0.5f); 
			Vector2 posVec2d = new Vector2 (body.position.x, body.position.y);
			//If the embarked body's interior position is outside the radius of the vessel plus the radius of this hull plus some small margin to prevent immediately re-embarking
		//	if (posVec2d.magnitude > (mount.GetSize () / 2f) + this.size / 2f + 20f) {
		//		DismountHull ();
		//	}
		} else {
			mounted = false;
			if (Mathf.Abs (body.position.z -  0.5f) > 0.1f)	//Keep the hull within its collision layer
				body.position = new Vector3(body.position.x, body.position.y, 0.5f);
		}
	}

	void Update () {
		HullUpdate ();
	}

	public Rigidbody GetBody(){
		return body;
	}

	public float GetBoardingRatio(){
		return boardingRatio;
	}

	public virtual void MountHull(Hull hitHull){
		//Store a reference to the object embarked upon
		mount = hitHull;	
		GameObject mountGO = hitHull.gameObject;// collision.gameObject;

		//Find the position of the passenger relative to the hull in the hull's space
		//Vector3 relativePosition = body.position - collision.rigidbody.position;
		Vector3 relativePosition = mount.body.transform.InverseTransformPoint(body.position);
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
		Vector3 relativeVelocity = mount.body.transform.InverseTransformVector(body.velocity);
		body.velocity = relativeVelocity;

	//	CalculateTransform();
		//Place in the interior layer
		this.gameObject.layer = 8;
	}

	protected void EnableZConstraint(){
		body.constraints |= RigidbodyConstraints.FreezePositionZ;
	}

	//At the moment, just dismount to world space.
	public void DismountHull(){
		
	//	body.constraints &= ~RigidbodyConstraints.FreezePositionZ;

	//	Vector3 facePosition = face.transform.position;
	//	facePosition.z = 0f; //Need to make sure this is zero, otherwise when face updates position to hull, it will be in a different zlayer, and the hull will fail to move to 0 on the z axis.
		//Note that zlayers are different to collision layers.
		Vector3 disembarkPosition = mount.GetWorldPosition() + mount.GetWorldRotation() * body.position;

		//If the mount is in the world layer, disembark to the zlayer zero and switch to default collision layer
		if (mount.GetMount () == null) {
			hullLocationZLayer = 0.0f;
			this.gameObject.layer = 0;
		} else {	//Otherwise, enter the same zlayer that the mount is parented to, and stay in the interior collision layer
			hullLocationZLayer = mount.GetMount().GetZLayer();
		}
		disembarkPosition.z = hullLocationZLayer;
		//disembarkPosition.z = mount.GetZLayer();
		body.MovePosition(disembarkPosition);

	//	Quaternion disembarkRotation = Quaternion.LookRotation(Vector3.forward, body.transform.up);
		Quaternion disembarkRotation = mount.GetWorldRotation() * body.rotation;
		body.rotation = disembarkRotation;
	//	body.position = new Vector3 (0f, 0f, 0f);
	//	Invoke("EnableZConstraint", 0.01f);


		//Transform will change when dismount. Calculate now, rather than wait for update.
	//	CalculateTransform();
		//Update the hullWorldPosition manually, as may not be entered into physics simulation yet.
	//	hullWorldPosition = disembarkPosition;

		if (mount.GetMount () != null)
			MountHull (mount.GetMount());
		else
			mount = null;
		
	//	PlaceFace ();
	}

/*	void OnCollisionEnter(Collision collision) {
		//If this collision was with another hull
		Hull hitHull = collision.gameObject.GetComponent<Hull> () as Hull;
		if (hitHull != null) {
			//If the size of the hit hull is large enough
			if (this.size * this.boardingRatio < hitHull.GetSize ()) {
				//Store a reference to this hull, and prevent null mount (eg, solar) updates from occuring
				MountHull(hitHull);


				//	}
			}
		}
	}*/
}
