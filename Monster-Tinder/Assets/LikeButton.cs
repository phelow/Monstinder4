using UnityEngine;
using System.Collections;

public class LikeButton : Button {
	[SerializeField]GameObject m_match;
	[SerializeField]private AudioClip m_noMatchLikeClip;
	[SerializeField]private AudioClip m_matchLikeClip;
    [SerializeField]
    private GameObject mp_heart;
    // Use this for initialization
    void Start () {
		this.m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerProfile>();

    }
	
	// Update is called once per frame
	void Update () {
        
    }

	public void ButtonPress(){
		if (Button.ms_active) {
			//Spawn a new Match
			//TODO: clean this up
			GameObject go = MatchProfile.ms_currentMatch.NextMatchPostion();
            Vector3 position = go.transform.position;
			Quaternion rotation = go.transform.rotation;

			Debug.Log ("Like");

            Profile.StopHighlightingParts(m_player, MatchProfile.ms_currentMatch);
            bool isMatch;

			if (m_player.CheckForMatch (MatchProfile.ms_currentMatch)) {
                isMatch = true;

                int numHearts = Random.Range(5, 20);
                float range = .3f;

                for (int i = 0; i < numHearts; i++)
                {
                    (GameObject.Instantiate(mp_heart, PlayerProfile.GetPlayer().gameObject.transform.position + new Vector3(Random.Range(-range, range), Random.Range(-range, range), 1.0f), PlayerProfile.GetPlayer().gameObject.transform.rotation, null) as GameObject).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 300.0f)));
                }


                numHearts = Random.Range(5, 20);
                for (int i = 0; i < numHearts; i++)
                {
                    (GameObject.Instantiate(mp_heart, MatchProfile.ms_currentMatch.transform.position + new Vector3(Random.Range(-range, range), Random.Range(-range, range), 1.0f), MatchProfile.ms_currentMatch.transform.rotation, null) as GameObject).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 300.0f))); ;
                }
                //Profile.HighLightMatchingParts (m_player, MatchProfile.ms_currentMatch);
                this.m_audioSource.PlayOneShot (m_matchLikeClip);
				PlayerProfile.AddMatch ();
                MatchManager.SaveMatch(MatchProfile.ms_currentMatch.gameObject);

                MatchManager.InstantiateNewMatch(true);
            } else
            {
                this.m_player.DropMatchCorrections(MatchProfile.ms_currentMatch);
                isMatch = false;

                this.m_audioSource.PlayOneShot (m_noMatchLikeClip);
				//Profile.HighLightConflicts (m_player, MatchProfile.ms_currentMatch);
				PlayerProfile.RemoveMatch ();
                MatchManager.SaveReject(MatchProfile.ms_currentMatch.gameObject);
                MatchManager.ShowWhyYouFailed(false);
            }
        }

	}
}
