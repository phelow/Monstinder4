using UnityEngine;
using System.Collections;

public class ManagePersistantSound : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void OnLevelWasLoaded () {
		GameObject go = GameObject.Find ("Persistant Sound").gameObject;
		if (go != this.gameObject) {
			Destroy (go);
		}
	}
}
