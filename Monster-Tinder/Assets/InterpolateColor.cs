using UnityEngine;
using System.Collections;

public class InterpolateColor : MonoBehaviour {
	[SerializeField]private MeshRenderer m_meshRenderer;
	[SerializeField]private float m_lerpTime = .1f;

	// Use this for initialization
	void Start () {
		StartCoroutine (LerpColor ());	
	}

	private IEnumerator LerpColor(){
		Color startingColor = m_meshRenderer.material.color;

		while (true) {
			float t = 0.0f;

			while (t < m_lerpTime) {
				t += Time.deltaTime;

				m_meshRenderer.material.color = Color.Lerp (startingColor, Color.clear, t / m_lerpTime);
				yield return new WaitForEndOfFrame ();
			}
			
			t = 0.0f;

			while (t < m_lerpTime) {
				t += Time.deltaTime;

				m_meshRenderer.material.color = Color.Lerp (Color.clear, startingColor, t / m_lerpTime);
				yield return new WaitForEndOfFrame ();
			}
		}
	}

}
