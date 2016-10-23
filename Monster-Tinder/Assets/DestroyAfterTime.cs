using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {
	private float m_timeUntilDestroy = 5.0f;
    [SerializeField]
    private Rigidbody2D m_rigidbody;
    private bool activated = false;

	// Use this for initialization
	void Start () {
	}

	private IEnumerator DestroyLater(float time){
		yield return new WaitForSeconds (time);
		Destroy (this.gameObject);
	}

	// Update is called once per frame
	void Update () {
	    if(activated == false && (m_rigidbody == null || m_rigidbody.gravityScale != 0))
        {
            activated = true;
            StartCoroutine(DestroyLater(m_timeUntilDestroy));
        }
	}
}
