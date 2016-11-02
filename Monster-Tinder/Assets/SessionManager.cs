using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class SessionManager : MonoBehaviour {
	private const int mc_startingTime = 30;
	private const int mc_timePerLevel = 10;

    public static int ms_matchesNeeded = 0;

    [SerializeField]private Text m_timerText;
	[SerializeField]private AudioSource m_audioSource;
	[SerializeField]private AudioClip m_alarmClip;

	[SerializeField]private GameObject m_highscoreSpawnPosition;

	[SerializeField]private GameObject [] m_endGameDestruction;

    [SerializeField]
    private AudioSource m_gameplayMusic;

    [SerializeField]
    private int time;

	// Use this for initialization
	void Start () {
        //calculate time
        int curDifficulty = PlayerPrefs.GetInt ("Level",0);
		time = (int) ((curDifficulty) * 10 + mc_startingTime);

		//Kick off timer coroutine
		StartCoroutine(TimeLevel());
	}

    public static List<BodyPart.ElementType> AvailableTypes() {
        List<BodyPart.ElementType> availableElements = new List<BodyPart.ElementType>();
        int curDifficulty = PlayerPrefs.GetInt("Level", 0);


        if (curDifficulty == 0) //Introduce fire and water
        {
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Water);
        }
        else if (curDifficulty == 1) //Introduce earth
        {
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Earth);
        }
        else if (curDifficulty == 2) //Introduce plant
        {
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Earth);
            availableElements.Add(BodyPart.ElementType.Plant);
        }
        else if (curDifficulty == 3) //introduce glitch
        {
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Earth);
            availableElements.Add(BodyPart.ElementType.Plant);
            availableElements.Add(BodyPart.ElementType.Glitch);
        }
        else if (curDifficulty == 4) //introduce ice
        {
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Earth);
            availableElements.Add(BodyPart.ElementType.Ice);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Plant);
        }
        else if (curDifficulty == 5) //introduce Light
        {
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Ice);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Light);
        }
        else if (curDifficulty == 6) //introduce dark
        {
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Ice);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Light);
            availableElements.Add(BodyPart.ElementType.Dark);
        }
        else if (curDifficulty == 7) //introduce spirit
        {
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Ice);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Light);
            availableElements.Add(BodyPart.ElementType.Dark);
            availableElements.Add(BodyPart.ElementType.Spirit);
        }
        else if (curDifficulty == 8) //introduce Dragon
        {
            availableElements.Add(BodyPart.ElementType.Ice);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Light);
            availableElements.Add(BodyPart.ElementType.Dark);
            availableElements.Add(BodyPart.ElementType.Spirit);
            availableElements.Add(BodyPart.ElementType.Dragon);
        }
        else if (curDifficulty == 9)//introduce spirit
        {
            availableElements.Add(BodyPart.ElementType.Light);
            availableElements.Add(BodyPart.ElementType.Dark);
            availableElements.Add(BodyPart.ElementType.Dragon);
            availableElements.Add(BodyPart.ElementType.Spirit);
        }
        else if (curDifficulty == 10)//introduce poison
        {
            availableElements.Add(BodyPart.ElementType.Light);
            availableElements.Add(BodyPart.ElementType.Dark);
            availableElements.Add(BodyPart.ElementType.Dragon);
            availableElements.Add(BodyPart.ElementType.Spirit);
            availableElements.Add(BodyPart.ElementType.Poison);
        }
        else if (curDifficulty == 11)
        {
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Earth);
            availableElements.Add(BodyPart.ElementType.Plant);
            availableElements.Add(BodyPart.ElementType.Dark);
            availableElements.Add(BodyPart.ElementType.Dragon);
            availableElements.Add(BodyPart.ElementType.Spirit);
        }
        else if (curDifficulty == 12) // introduce metal
        {
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Earth);
            availableElements.Add(BodyPart.ElementType.Earth);
            availableElements.Add(BodyPart.ElementType.Dark);
            availableElements.Add(BodyPart.ElementType.Dragon);
            availableElements.Add(BodyPart.ElementType.Metal);
        }
        else if (curDifficulty == 13) // introduce bug
        {
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Earth);
            availableElements.Add(BodyPart.ElementType.Plant);
            availableElements.Add(BodyPart.ElementType.Bug);
        }
        else if (curDifficulty == 14)
        {
            availableElements.Add(BodyPart.ElementType.Plant);
            availableElements.Add(BodyPart.ElementType.Poison);
            availableElements.Add(BodyPart.ElementType.Bug);
            availableElements.Add(BodyPart.ElementType.Metal);
            availableElements.Add(BodyPart.ElementType.Ice);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Earth);
        }
        else if (curDifficulty == 15)
        {
            availableElements.Add(BodyPart.ElementType.Plant);
            availableElements.Add(BodyPart.ElementType.Light);
            availableElements.Add(BodyPart.ElementType.Bug);
            availableElements.Add(BodyPart.ElementType.Metal);
            availableElements.Add(BodyPart.ElementType.Dragon);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Glitch);
        }
        else if (curDifficulty == 16)
        {
            availableElements.Add(BodyPart.ElementType.Plant);
            availableElements.Add(BodyPart.ElementType.Light);
            availableElements.Add(BodyPart.ElementType.Bug);
            availableElements.Add(BodyPart.ElementType.Metal);
            availableElements.Add(BodyPart.ElementType.Dragon);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Glitch);
            availableElements.Add(BodyPart.ElementType.Psychic);
        }
        else if (curDifficulty == 17)
        {
            availableElements.Add(BodyPart.ElementType.Plant);
            availableElements.Add(BodyPart.ElementType.Light);
            availableElements.Add(BodyPart.ElementType.Bug);
            availableElements.Add(BodyPart.ElementType.Metal);
            availableElements.Add(BodyPart.ElementType.Dragon);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Glitch);
            availableElements.Add(BodyPart.ElementType.Psychic);
            availableElements.Add(BodyPart.ElementType.Ice);
        }
        else
        {
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Plant);
            availableElements.Add(BodyPart.ElementType.Earth);
            availableElements.Add(BodyPart.ElementType.Dark);
            availableElements.Add(BodyPart.ElementType.Spirit);
            availableElements.Add(BodyPart.ElementType.Poison);
            availableElements.Add(BodyPart.ElementType.Light);
            availableElements.Add(BodyPart.ElementType.Bug);
            availableElements.Add(BodyPart.ElementType.Metal);
            availableElements.Add(BodyPart.ElementType.Dragon);
            availableElements.Add(BodyPart.ElementType.Glitch);
            availableElements.Add(BodyPart.ElementType.Psychic);
            availableElements.Add(BodyPart.ElementType.Fire);
            availableElements.Add(BodyPart.ElementType.Water);
            availableElements.Add(BodyPart.ElementType.Plant);
            availableElements.Add(BodyPart.ElementType.Ice);

        }

        return availableElements;

	}


	private IEnumerator TimeLevel(){
		//Check for level unlocked
		int curDifficulty = PlayerPrefs.GetInt ("Level",0);
		int maxLevelUnlocked = PlayerPrefs.GetInt ("MaxLevel", 0);
        ms_matchesNeeded = curDifficulty  + 5 / Mathf.Max ((5 - curDifficulty), 1) + 2;
        PlayerProfile.SetMatchesNeeded();

        while (time > 0) {
			yield return new WaitForSeconds (1.0f);
			time--;
			m_timerText.text = "Time Left:" + time;
		}

        float lerpAudio = 1.0f;

        while(lerpAudio > 0.0f)
        {
            lerpAudio -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            this.m_gameplayMusic.volume = Mathf.Lerp(0.0f,1.0f, lerpAudio);
        }
        
		//end the level
		m_audioSource.PlayOneShot(m_alarmClip);
		yield return new WaitForSeconds (3.0f);
        
		//Check for high score, save if you have one
		int previousHighscore =  PlayerPrefs.GetInt ("HighScore",0);
		int curScore = PlayerProfile.GetScore () * curDifficulty;
        PlayerPrefs.SetInt("Score", PlayerProfile.GetScore());
        PlayerPrefs.SetInt("HighScoreBeaten", 0);

        if (curScore > previousHighscore) {
			PlayerPrefs.SetInt ("HighScore",curScore );
			PlayerPrefs.SetInt ("HighScoreBeaten", 1);
		}

		PlayerPrefs.SetInt ("LastDifficultyPlayed",curDifficulty);

		PlayerPrefs.SetInt ("LastCorrectChoices",PlayerProfile.GetCorrectChoices());
		PlayerPrefs.SetInt ("LastIncorrectChoices",PlayerProfile.GetIncorrectChoices());
        
        //if unlocked return to main menu
        if ( PlayerProfile.GetScore ()  >= ms_matchesNeeded) {
            if (maxLevelUnlocked < curDifficulty + 1)
            {
                PlayerPrefs.SetInt("MaxLevel", curDifficulty + 1);
            }

            if (PlayerPrefs.GetInt("Level", 0) > 1)
            {
                
                Fader.Instance.FadeIn().LoadLevel("MatchRejects").FadeOut();
            }
            else
            {
                Fader.Instance.FadeIn().LoadLevel("Success").FadeOut();
            }
		} else {
			Fader.Instance.FadeIn().LoadLevel( "Failure" ).FadeOut();
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
