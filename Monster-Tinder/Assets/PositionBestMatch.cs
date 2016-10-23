using UnityEngine;
using System.Collections;

public class PositionBestMatch : MonoBehaviour {

	// Use this for initialization
	void Start () {

        GameObject.Find("BestMatch").transform.position = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
