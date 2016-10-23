using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReturnToMainMenuOnClick : MonoBehaviour {

    [SerializeField]
    private UnityEngine.UI.Button m_button;

	void Start(){

		Fader.Instance.FadeOut (3.0f);
	}

	public void r(){
        m_button.interactable = false;
		Fader.Instance.FadeIn(.1f).LoadLevel( "Main Menu" );
	}
}
