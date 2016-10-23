using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DifficultySystem : MonoBehaviour {
	[SerializeField]private Text m_text;

    [SerializeField]
    private UnityEngine.UI.Button m_minusButton;
    [SerializeField]
    private UnityEngine.UI.Button m_plusButton;

    // Use this for initialization
    void Start ()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("MaxLevel", 0));
        SetDifficulty ();
	}
	
	// Update is called once per frame
	void Update ()
    {
        int curDifficulty = PlayerPrefs.GetInt("Level", 0);
        
        if(curDifficulty == 0)
        {
            this.m_minusButton.interactable = false;
        }
        else
        {
            this.m_minusButton.interactable = true;
        }


        if (curDifficulty < PlayerPrefs.GetInt("MaxLevel", 0))
        {
            this.m_plusButton.interactable = true;
        }
        else
        {
            this.m_plusButton.interactable = false;
        }

    }

	private void SetDifficulty(){
		m_text.text = "Level: " + PlayerPrefs.GetInt ("Level", 0);

	}

	public void RaiseDifficulty(){
		int curDifficulty = PlayerPrefs.GetInt ("Level",0);
		int maxDifficultyUnlocked = PlayerPrefs.GetInt ("MaxLevel", 0);

		if (curDifficulty < maxDifficultyUnlocked) {
			PlayerPrefs.SetInt ("Level", curDifficulty + 1);
		}
		SetDifficulty ();
	}

	public void LowerDifficuty(){
		int curDifficulty = PlayerPrefs.GetInt ("Level",0);

		if (curDifficulty > 0) {
			PlayerPrefs.SetInt ("Level", curDifficulty - 1);
		}
		SetDifficulty ();
	}
}
