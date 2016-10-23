using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScore : MonoBehaviour {
	[SerializeField]private Text m_text;
	// Use this for initialization
	void Start () {
		m_text.text = "High Score:" + PlayerPrefs.GetInt ("HighScore",0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
