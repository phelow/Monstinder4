using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenerateText : MonoBehaviour {
	[SerializeField]private Text m_text;
	// Use this for initialization
	void Start () {
		int curDifficulty = PlayerPrefs.GetInt ("LastDifficultyPlayed");

		m_text.text = "Success! Level " + (curDifficulty+1)+ " has been unlocked. Click to return to main menu.\n";

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
