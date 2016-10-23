using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenerateFailureText : MonoBehaviour {
	[SerializeField]private Text m_text;
	// Use this for initialization
	void Start () {

		m_text.text = "You have failed. Click to return to main menu.";

		if (PlayerPrefs.GetInt ("HighScoreBeaten") > 0) {
			m_text.text += "High Score Broken\n";
		}
		PlayerPrefs.SetInt ("HighScoreBeaten", 0);
		m_text.text += "\nHigh Score: " + PlayerPrefs.GetInt ("HighScore");
		m_text.text += "\nCorrect Choices: " + PlayerPrefs.GetInt ("LastCorrectChoices");
		m_text.text += "\nIncorrect Choices: " + PlayerPrefs.GetInt ("LastIncorrectChoices");
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
