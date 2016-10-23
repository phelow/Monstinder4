using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFadeIn : MonoBehaviour {
	// Use this for initialization
	void Start () {
		Fader.Instance.FadeOut (3.0f);
	}
}
