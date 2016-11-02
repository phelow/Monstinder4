using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReturnToMainMenuOnClick : MonoBehaviour {

    [SerializeField]
    private UnityEngine.UI.Button m_button;

	void Start(){

		Fader.Instance.FadeOut (.3f);
	}

	public void r(){
        m_button.interactable = false;
		Fader.Instance.FadeIn(.3f).LoadLevel( "Main Menu" ).FadeOut(.1f);
	}
}
