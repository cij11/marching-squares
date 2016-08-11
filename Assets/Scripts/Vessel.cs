using UnityEngine;
using System.Collections;

public class Vessel : Hull {
	public GameObject topographyPrefab;

	private Topography topography;

	public override void Initialise(float size){
		body = GetComponent<Rigidbody> () as Rigidbody;

		this.hullWorldPosition = this.body.position;
		this.hullWorldRotation = this.body.rotation;

		GameObject faceGO = Instantiate (facePrefab, this.body.position, Quaternion.identity) as GameObject;
		face = faceGO.GetComponent<Face> () as Face;
		face.Initialise (this, this.size);

		layerManager = GameObject.FindGameObjectWithTag ("LayerManager").GetComponent<LayerManager> () as LayerManager;
		interiorZLayer = layerManager.GetLayer ();

		this.size = size;
		GetComponent<SphereCollider> ().radius = size / 2f;

		GameObject envelopeGO = Instantiate (envelopePrefab, this.body.position, Quaternion.identity) as GameObject;
		envelope = envelopeGO.GetComponent<Envelope> () as Envelope;
		envelope.Initialise (this, this.size);

		GameObject topographyGO = Instantiate (topographyPrefab, this.body.position, Quaternion.identity) as GameObject;
		topography = topographyGO.GetComponent<Topography> () as Topography;
		topography.Initialise (this, this.face, this.interiorZLayer, this.size);
	}
}