using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {
	bool launched = false;
	public void Start(){
		Fader.Instance.FadeOut (3);
	}

	// Use this for initialization
	public void LaunchGame () {
		if (launched == false) {
			launched = true;
			Fader.Instance.FadeIn ().LoadLevel ("DialogBeforeCharacterCustomization").FadeOut ();
		}
	}	
}
