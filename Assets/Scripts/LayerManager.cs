using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Allocates space on the z-axis for vessel interiors.
//Todo: spread out collision layers to speed collision detection

public class LayerManager : MonoBehaviour {
	private float layerSeparation = 100f;
	private float highestLayer = 0f;
	private Stack<float> layerStack = new Stack<float>();

	// Use this for initialization
	void Start () {
	
	}

	//Allocate z space to the newly instantiated vessel
	public float GetLayer(){
		if (layerStack.Count > 0)
			return layerStack.Pop ();
		else {
			highestLayer += layerSeparation;
			return highestLayer;
		}
	}

	//Add the z-space back to the stack, freeing it for future vessels
	public void FreeLayer(int layer){
		layerStack.Push (layer);
	}
}
