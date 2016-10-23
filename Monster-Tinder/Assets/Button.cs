using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
	[SerializeField]protected PlayerProfile m_player;
	[SerializeField]protected AudioSource m_audioSource;

	protected static bool ms_active = true;

	public static void SetActive(bool state){
		ms_active = state;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected IEnumerator DestroyMatch(GameObject go){
		Button.SetActive (false);
		Rigidbody2D rb = go.GetComponent<Rigidbody2D> ();
		BoxCollider2D [] colliders = go.GetComponentsInChildren<BoxCollider2D> ();

		for(int i = 0; i < colliders.Length; i++){
			Destroy (colliders[i]);
		}

		yield return new WaitForSeconds (1.0f);

		rb.isKinematic = false;
		rb.AddForce (new Vector2(Random.Range(-500.0f,500.0f),Random.Range(-500.0f,500.0f)));
		rb.gravityScale = 10.0f;
		Button.SetActive (true);
		yield return new WaitForSeconds (10.0f);
		Destroy (go);
	}
}
