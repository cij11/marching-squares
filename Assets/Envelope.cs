using UnityEngine;
using System.Collections;

public class Envelope : Face {
	public float envelopeBuffer = 1.0f;

	public override void Initialise(Hull parentHull, float size){
		this.parentHull = parentHull;
		//	this.transform.localScale = new Vector3 (size, size, 0.1f);
		SphereCollider sphereCollider = GetComponent<SphereCollider>() as SphereCollider;
		sphereCollider.radius = (size / 2f) + envelopeBuffer;
	}
	
	// Update is called once per frame
	void Update () {
		PlaceFace ();
	}

	void OnTriggerEnter(Collider collider) {
		//If this collision was with another hull
		Envelope hitEnvelope = collider.gameObject.GetComponent<Envelope> () as Envelope;
		if (hitEnvelope != null) {
			Hull hitHull = hitEnvelope.GetParentHull ();
		

			if (this.parentHull.GetSize () * this.parentHull.GetBoardingRatio () < hitHull.GetSize ()) {
				//If this hull is not mounted, board the hull belonging to the triggering envelope
				if (this.parentHull.mounted == false)
					this.parentHull.MountHull (hitHull);
				else {
					//Otherwise, check to see if hull this hull is mounted to is anywhere inside the collided with
					//envelope's parent hierachy.
					if (this.parentHull.GetMount () == hitHull.GetMount ()) {//Just checks the layer above. Need a recurvsive check.
						this.parentHull.MountHull (hitHull);
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider collider){
		Envelope hitEnvelope = collider.gameObject.GetComponent<Envelope> () as Envelope;
		if (hitEnvelope != null) {
			Hull hitHull = hitEnvelope.GetParentHull ();
			//If this is the hull that we are mounted to
			if (GetParentHull ().GetMount() == hitHull)
				GetParentHull ().DismountHull ();
		}
	}
}