using UnityEngine;
using System.Collections;

public class PositionBestMatch : MonoBehaviour {

	// Use this for initialization
	void Start () {
        try {
            GameObject.Find("BestMatch").transform.position = transform.position;
        }
        catch
        {

        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
