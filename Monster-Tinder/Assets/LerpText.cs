using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LerpText : MonoBehaviour {
	[SerializeField]private Text m_text;
	[SerializeField]private float lerpTime = .3f;

	// Use this for initialization
	void Start () {
		StartCoroutine (Lerp (m_text));
	}

	private IEnumerator Lerp(Text text){
		Color origColor = text.color;

		while (true) {
			float t = 0.0f;

			while (t < lerpTime) {
				m_text.color = Color.Lerp (origColor, Color.blue, t / lerpTime);
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame ();
			}

			t = 0.0f;

			while (t < lerpTime) {
				m_text.color = Color.Lerp ( Color.blue, origColor, t / lerpTime);
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame ();
			}

		}
	}
}
