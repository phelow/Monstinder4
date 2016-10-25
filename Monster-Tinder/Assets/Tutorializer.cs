using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorializer : MonoBehaviour
{
    public static Tutorializer ms_instance;

    [SerializeField]
    private Text m_text;

    [SerializeField]
    private MatchChoice m_leftChoice;

    [SerializeField]
    private MatchChoice m_rightChoice;
    [SerializeField]
    private GameObject mp_heart;

    [SerializeField]
    private static MatchChoice ms_playerChoice;

    [SerializeField]
    private Text m_timerText;

    [SerializeField]
    private Text m_hintText;

    [SerializeField]
    private AudioSource m_source;

    [SerializeField]
    private AudioClip m_countingSound;
    [SerializeField]
    private AudioClip m_countingSoundTwo;
    [SerializeField]
    private AudioClip m_alarm;
    [SerializeField]
    private AudioClip m_tick;
    [SerializeField]
    private AudioClip m_match;
    [SerializeField]
    private AudioClip m_fail;
    [SerializeField]
    private AudioClip m_completeSound;

    [SerializeField]
    private Text m_TutorialText;
    private Text m_scoreText;

    private int score;
    public List<GameObject> matches; //temporarily public

    // Use this for initialization
    void Start()
    {
        PlayerProfile.GetPlayer().StartHighlighting();


        m_scoreText = GameObject.Find("CurrentScoreText").GetComponent<Text>();
        score = PlayerPrefs.GetInt("Score");

        m_scoreText.text = "" + score;

        ms_instance = this;
        StartCoroutine(Tutorialize());
    }

    private IEnumerator Tutorialize()
    {

        
        Fader.Instance.FadeOut(1.0f);
        m_text.text = "Congratulations, you've unlocked the next level.";
        yield return new WaitForSeconds(3.0f);
        Fader.Instance.FadeIn(.2f);

        m_text.text = " You got " + MatchManager.NumMatches() + " matches.";
        yield return new WaitForSeconds(3.0f);
        Fader.Instance.FadeIn(.2f);
        yield return new WaitForSeconds(2.0f);
        Fader.Instance.FadeOut(.2f);
        m_text.text = "But can you find true love?";
        yield return new WaitForSeconds(3.0f);
        Fader.Instance.FadeIn(.2f);
        yield return new WaitForSeconds(2.0f);
        m_text.text = "Pick the best match";
        Fader.Instance.FadeOut(.2f);
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(PickOne());
    }

    public static void SetPlayerChoice(MatchChoice choice)
    {
        Tutorializer.ms_playerChoice = choice;
    }

    private void AddPoint(int points)
    {
        score += points;
        PlayerPrefs.SetInt("Score", score);

        m_scoreText.text = "" + ++score;
    }

    private IEnumerator PickOne()
    {
        float interpolateTime = .1f;

        List<GameObject> tempMatches = MatchManager.GetMatches();
        int m;
        for (m =0; m < 3 && m < tempMatches.Count; m++)
        {
            matches.Add(tempMatches[m]);
            tempMatches.Remove(tempMatches[m]);
        }

        for(; m < tempMatches.Count/3; m++)
        {
            matches.Add(tempMatches[m]);
            tempMatches.Remove(tempMatches[m]);

        }
        


        while (matches.Count > 1)
        {
            m_text.text = "Pick the best match.\nMatches Left:" + matches.Count;
            foreach (SpriteRenderer sr in PlayerProfile.GetPlayer().GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.white;
            }
            //start a timer
            float timeLeft = 15.0f;

            //randomly choose two matches
            GameObject leftMatch = matches[Random.Range(0, matches.Count)];
            matches.Remove(leftMatch);
            GameObject rightMatch = matches[Random.Range(0, matches.Count)];
            matches.Remove(rightMatch);

            m_rightChoice.SetMonster(rightMatch);
            m_leftChoice.SetMonster(leftMatch);

            ms_playerChoice = null;
            //wait for player input
            EnableInput();
            while (ms_playerChoice == null && timeLeft > 0.0f)
            {
                if (timeLeft < 1.0f)
                {
                    this.m_source.PlayOneShot(this.m_alarm);
                }
                else
                {
                    this.m_source.PlayOneShot(this.m_tick);

                }

                timeLeft -= 1;
                m_timerText.text = "" + ((int)Mathf.Round(timeLeft));
                yield return new WaitForSeconds(1.0f);
            }

            DisableInput();

            bool success = true;
            //see if the player picked the best match
            bool m_rightBest = m_rightChoice.GetIsBetterChoiceThan(m_leftChoice);
            bool m_leftBest = m_leftChoice.GetIsBetterChoiceThan(m_rightChoice);

            if (ms_playerChoice == null)
            {
                //FAILURE
                TallyScore();
                Fader.Instance.FadeIn().LoadLevel("Failure").FadeOut();

            }

            if ((m_rightBest && m_leftBest) || (!m_rightBest && ms_playerChoice == m_leftChoice) || (m_rightBest && ms_playerChoice == m_rightChoice))
            {
                success = true;

                matches.Add(ms_playerChoice.GetMonster());
            }
            else
            {
                success = false;
            }

            MatchChoice otherChoice = m_rightChoice;

            if (otherChoice == ms_playerChoice)
            {
                otherChoice = m_leftChoice;
            }

            int sameCount = ms_playerChoice.GetSamePartsAsPlayerCount();
            int differentCount = ms_playerChoice.GetDifferentPartsFromPlayerCount();

            int p = sameCount + differentCount;

            m_hintText.text = "Matching Parts:" + 0;
            m_hintText.text += "\nClashing Parts:" + 0;

            List<KeyValuePair<SpriteRenderer, SpriteRenderer>> spritePairs = Profile.GetMatchingPairs(PlayerProfile.GetPlayer(), ms_playerChoice.GetMonster().GetComponent<Profile>());
            foreach (SpriteRenderer sr in PlayerProfile.GetPlayer().GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }
            foreach (SpriteRenderer sr in ms_playerChoice.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }
            foreach (SpriteRenderer sr in otherChoice.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }

            //Profile.HighLightMatchingParts(PlayerProfile.GetPlayer(), ms_playerChoice.GetMonster().GetComponent<Profile>());
            //compare the player and the choice made
            for (int i = 1; i <= sameCount; i++)
            {
                this.m_source.Stop();
                this.m_source.PlayOneShot(this.m_countingSoundTwo);
                m_hintText.text = "Matching Parts:" + i;
                m_hintText.text += "\nClashing Parts:" + 0;
                AddPoint(1);
                for (int j = 0; j < 3; j++)
                {
                    float tLeft = interpolateTime;
                    while (tLeft > 0.0f)
                    {
                        spritePairs[i - 1].Key.color = Color.Lerp(Color.white, Color.green, tLeft);
                        spritePairs[i - 1].Value.color = Color.Lerp(Color.white, Color.green, tLeft);
                        tLeft -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    tLeft = interpolateTime;
                    while (tLeft > 0.0f)
                    {
                        spritePairs[i - 1].Key.color = Color.Lerp(Color.green, Color.white, tLeft);
                        spritePairs[i - 1].Value.color = Color.Lerp(Color.green, Color.white, tLeft);
                        tLeft -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                }

                yield return new WaitForSeconds(Mathf.Lerp(0.1f, 1.0f, ((float)i) / sameCount));
            }
            this.m_source.PlayOneShot(this.m_completeSound);
            yield return new WaitForSeconds(1.0f);

            //Profile.HighLightConflicts(PlayerProfile.GetPlayer(), ms_playerChoice.GetMonster().GetComponent<Profile>());

            spritePairs = Profile.GetClashingPairs(PlayerProfile.GetPlayer(), ms_playerChoice.GetMonster().GetComponent<Profile>());
            foreach (SpriteRenderer sr in ms_playerChoice.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }
            foreach (SpriteRenderer sr in otherChoice.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }
            for (int i = 1; i <= differentCount; i++)
            {
                this.m_source.Stop();
                this.m_source.PlayOneShot(this.m_countingSoundTwo);
                m_hintText.text = "Matching Parts:" + sameCount;
                m_hintText.text += "\nClashing Parts:" + i;
                AddPoint(1);
                for (int j = 0; j < 3; j++)
                {
                    float tLeft = interpolateTime;
                    while (tLeft > 0.0f)
                    {
                        spritePairs[i - 1].Key.color = Color.Lerp(Color.white, Color.red, tLeft);
                        spritePairs[i - 1].Value.color = Color.Lerp(Color.white, Color.red, tLeft);
                        tLeft -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    tLeft = interpolateTime;
                    while (tLeft > 0.0f)
                    {
                        spritePairs[i - 1].Key.color = Color.Lerp(Color.red, Color.white, tLeft);
                        spritePairs[i - 1].Value.color = Color.Lerp(Color.red, Color.white, tLeft);
                        tLeft -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                }
                yield return new WaitForSeconds(Mathf.Lerp(0.1f, 1.0f, ((float)i) / differentCount));
            }

            this.m_source.PlayOneShot(this.m_completeSound);
            yield return new WaitForSeconds(1.0f);

            sameCount = otherChoice.GetSamePartsAsPlayerCount();
            differentCount = otherChoice.GetDifferentPartsFromPlayerCount();

            p += sameCount + differentCount;

            //Profile.HighLightMatchingParts(PlayerProfile.GetPlayer(), otherChoice.GetMonster().GetComponent<Profile>());
            spritePairs = Profile.GetMatchingPairs(PlayerProfile.GetPlayer(), otherChoice.GetMonster().GetComponent<Profile>());
            foreach (SpriteRenderer sr in PlayerProfile.GetPlayer().GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }
            foreach (SpriteRenderer sr in ms_playerChoice.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }
            foreach (SpriteRenderer sr in otherChoice.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }

            //compare the player and the choice made
            for (int i = 1; i <= sameCount; i++)
            {
                this.m_source.Stop();
                this.m_source.PlayOneShot(this.m_countingSoundTwo);
                m_hintText.text = "Matching Parts:" + i;
                m_hintText.text += "\nClashing Parts:" + 0;
                AddPoint(1);
                for (int j = 0; j < 3; j++)
                {
                    float tLeft = interpolateTime;
                    while (tLeft > 0.0f)
                    {
                        spritePairs[i - 1].Key.color = Color.Lerp(Color.grey, Color.green, tLeft);
                        spritePairs[i - 1].Value.color = Color.Lerp(Color.grey, Color.green, tLeft);
                        tLeft -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    tLeft = interpolateTime;
                    while (tLeft > 0.0f)
                    {
                        spritePairs[i - 1].Key.color = Color.Lerp(Color.green, Color.grey, tLeft);
                        spritePairs[i - 1].Value.color = Color.Lerp(Color.green, Color.grey, tLeft);
                        tLeft -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                }
                yield return new WaitForSeconds(Mathf.Lerp(0.1f, 1.0f, ((float)i) / sameCount));
            }
            this.m_source.PlayOneShot(this.m_completeSound);
            yield return new WaitForSeconds(1.0f);


            spritePairs = Profile.GetClashingPairs(PlayerProfile.GetPlayer(), otherChoice.GetMonster().GetComponent<Profile>());
            foreach (SpriteRenderer sr in PlayerProfile.GetPlayer().GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }
            foreach (SpriteRenderer sr in ms_playerChoice.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }
            foreach (SpriteRenderer sr in otherChoice.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.grey;
            }
            //Profile.HighLightConflicts(PlayerProfile.GetPlayer(), otherChoice.GetMonster().GetComponent<Profile>());
            for (int i = 1; i <= differentCount; i++)
            {
                this.m_source.Stop();
                this.m_source.PlayOneShot(this.m_countingSoundTwo);
                m_hintText.text = "Matching Parts:" + sameCount;
                m_hintText.text += "\nClashing Parts:" + i;
                AddPoint(1);
                for (int j = 0; j < 3; j++)
                {
                    float tLeft = interpolateTime;
                    while (tLeft > 0.0f)
                    {
                        spritePairs[i - 1].Key.color = Color.Lerp(Color.grey, Color.red, tLeft);
                        spritePairs[i - 1].Value.color = Color.Lerp(Color.grey, Color.red, tLeft);
                        tLeft -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    tLeft = interpolateTime;
                    while (tLeft > 0.0f)
                    {
                        spritePairs[i - 1].Key.color = Color.Lerp(Color.red, Color.grey, tLeft);
                        spritePairs[i - 1].Value.color = Color.Lerp(Color.red, Color.grey, tLeft);
                        tLeft -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                }
                yield return new WaitForSeconds(Mathf.Lerp(0.1f, 1.0f, ((float)i) / differentCount));
            }
            this.m_source.PlayOneShot(this.m_completeSound);
            yield return new WaitForSeconds(1.0f);

            if (success)
            {
                int numHearts = Random.Range(5, 20);
                float range = .3f;

                for (int i = 0; i < numHearts; i++)
                {
                    (GameObject.Instantiate(mp_heart, PlayerProfile.GetPlayer().gameObject.transform.position + new Vector3(Random.Range(-range, range), Random.Range(-range, range), 1.0f), PlayerProfile.GetPlayer().gameObject.transform.rotation, null) as GameObject).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 300.0f)));
                }
                numHearts = Random.Range(5, 20);

                for (int i = 0; i < numHearts; i++)
                {
                    (GameObject.Instantiate(mp_heart, ms_playerChoice.transform.position + new Vector3(Random.Range(-range, range), Random.Range(-range, range), 1.0f), ms_playerChoice.gameObject.transform.rotation, null) as GameObject).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 300.0f)));
                }
                AddPoint(p);
            }
            else
            {
                this.m_source.PlayOneShot(this.m_fail);
                yield return new WaitForSeconds(1.0f);
                //FAILURE
                TallyScore();
                EnableInput();
                Fader.Instance.FadeIn().LoadLevel("Failure").FadeOut();
            }
            this.m_source.PlayOneShot(this.m_match);
            yield return new WaitForSeconds(1.0f);

            ms_playerChoice.HideCharacter();
            otherChoice.ExplodeCharacter();
            yield return new WaitForSeconds(1.0f);
            //award points

            foreach (SpriteRenderer sr in PlayerProfile.GetPlayer().GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.white;
            }
            foreach (SpriteRenderer sr in ms_playerChoice.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.white;
            }
            foreach (SpriteRenderer sr in otherChoice.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.white;
            }
        }
        EnableInput();
        matches[0].transform.name = "BestMatch";
        DontDestroyOnLoad(matches[0]);

        TallyScore();
        foreach (SpriteRenderer sr in PlayerProfile.GetPlayer().GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = Color.white;
        }
        EnableInput();


        if(this.score > PlayerPrefs.GetInt(PlayerPrefs.GetInt("Level", 0) + "completed", 0))
        {
            PlayerPrefs.SetInt(PlayerPrefs.GetInt("Level", 0) + "completed", this.score);
        }
        Fader.Instance.FadeIn(.1f).LoadLevel("Success").FadeOut(.1f);
    }

   

    public void EnableInput()
    {
        UnityEngine.Cursor.visible = true;
        this.m_leftChoice.GetButton().enabled = true;
        this.m_rightChoice.GetButton().enabled = true;

        //lerp the buttons color
        StartCoroutine(CallAttentionToButtons());
    }

    private IEnumerator CallAttentionToButtons()
    {
        List<Image> colorAbleSprites = new List<Image>();

        colorAbleSprites.AddRange(this.m_leftChoice.GetButton().GetComponentsInChildren<Image>());
        colorAbleSprites.AddRange(this.m_rightChoice.GetButton().GetComponentsInChildren<Image>());

        float maxT = .5f;
        float t = maxT;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;

            foreach (Image image in colorAbleSprites)
            {
                image.color = Color.Lerp(Color.yellow, Color.white, t / maxT);
            }

            yield return new WaitForEndOfFrame();
        }

        t = maxT;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;

            foreach (Image image in colorAbleSprites)
            {
                image.color = Color.Lerp(Color.white, Color.yellow, t / maxT);
            }
            yield return new WaitForEndOfFrame();
        }

    }

    public void DisableInput()
    {
        m_TutorialText.text = "Pick the best match";
        UnityEngine.Cursor.visible = false;
        this.m_leftChoice.GetButton().enabled = false;
        this.m_rightChoice.GetButton().enabled = false;
    }

    public void TallyScore()
    {

        int previousHighscore = PlayerPrefs.GetInt("HighScore", 0);
        PlayerPrefs.SetInt("Score", score);

        if (score > previousHighscore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.SetInt("HighScoreBeaten", 1);
        }
        else
        {
            PlayerPrefs.SetInt("HighScoreBeaten", 0);

        }
    }

}
