using UnityEngine;
using System.Collections;

public class SolarManager : MonoBehaviour {
	public GameObject hullPrefab;
	public int numPlanets = 2;
	public float planetSize = 20f;

	public GameObject playerPrefab;

	// Use this for initialization
	void Start () {
		GameObject hull;

		for (int i = 0; i < numPlanets; i++) {
			hull = Instantiate (hullPrefab, this.transform.position + new Vector3((planetSize+0.2f)*i, 0f, 0f), Quaternion.identity) as GameObject;
			hull.GetComponent<Hull> ().Initialise (planetSize);
		}

		//Small vessel to try nested boarding
		hull = Instantiate (hullPrefab, this.transform.position + new Vector3(0f, -planetSize/2f - planetSize/3f + 260f, 0f), Quaternion.identity) as GameObject;
		hull.GetComponent<Hull> ().Initialise (planetSize/8f);

		GameObject player = Instantiate (playerPrefab, new Vector3 (0f, -planetSize/2f - planetSize/2.5f +383f, 0f), Quaternion.identity) as GameObject;
		player.GetComponent<Unit> ().Initialise (1.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
