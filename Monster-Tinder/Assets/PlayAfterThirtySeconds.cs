using UnityEngine;
using System.Collections;

public class PlayAfterThirtySeconds : MonoBehaviour {
	[SerializeField]private AudioSource m_audioSource;
	// Use this for initialization
	void Start () {
		StartCoroutine (PlayLater());
	}

	private IEnumerator PlayLater(){
		yield return new WaitForSeconds (30.0f);

		m_audioSource.Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
