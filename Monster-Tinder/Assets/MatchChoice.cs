using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MatchChoice : MonoBehaviour {
    private GameObject m_monster;
    private GameObject m_monsterReference;
    public Profile m_monsterProfile;
    private static PlayerProfile ms_playerProfile;

    [SerializeField]
    private UnityEngine.UI.Button m_button;

    void Awake()
    {
        m_button = this.GetComponentInChildren<UnityEngine.UI.Button>();
        m_button.enabled = true;
    }

	// Use this for initialization
	void OnLevelWasLoaded () {
        ms_playerProfile = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerProfile>();
	}
	
    public Profile GetMonsterProfile()
    {
        return m_monsterProfile;
    }

	// Update is called once per frame
	void Update () {
	
	}

    public UnityEngine.UI.Button GetButton()
    {
        return m_button;
    }


    public void HideCharacter()
    {
        m_monster.GetComponent<MatchProfile>().HidePolaroid();
        StartCoroutine(m_monsterProfile.LerpToClear());

        m_monsterReference.transform.SetParent(null);
    }

    public void ExplodeCharacter()
    {
        foreach (SpriteRenderer sr in m_monsterReference.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = Color.white;
            Rigidbody2D rb = sr.gameObject.AddComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(Random.Range(-500, 500), Random.Range(-500, 500)));
            rb.gravityScale = 1.0f;
        }


        m_monsterReference.transform.SetParent(null);
    }

    public void SetPlayerChoice()
    {
        Tutorializer.SetPlayerChoice(this);
    }

    public void SetMonster(GameObject choice)
    {
        m_monsterReference = choice;

        foreach(BoxCollider2D collider in m_monsterReference.GetComponentsInChildren<BoxCollider2D>())
        {
            Destroy(collider);
        }
        foreach (SpriteRenderer sr in m_monsterReference.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = Color.white;
        }

        m_monsterProfile = m_monsterReference.GetComponent<Profile>();
        m_monster = choice;
        choice.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - .1f);

        m_monster.GetComponent<MatchProfile>().HidePolaroid();
    }

    public bool GetIsBetterChoiceThan(MatchChoice otherChoice)
    {
        return GetSamePartsAsPlayerCount() - GetDifferentPartsFromPlayerCount() >=
            otherChoice.GetSamePartsAsPlayerCount() - otherChoice.GetDifferentPartsFromPlayerCount();
    }

    public int GetSamePartsAsPlayerCount()
    {
        return ms_playerProfile.GetSameParts(m_monsterProfile);
    }

    public int GetDifferentPartsFromPlayerCount()
    {
        return ms_playerProfile.GetDifferentParts(m_monsterProfile);
    }

    public GameObject GetMonster()
    {
        return m_monsterReference;
    }
}
