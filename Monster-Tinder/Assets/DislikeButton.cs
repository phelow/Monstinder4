using UnityEngine;
using System.Collections;

public class DislikeButton : Button {
	[SerializeField]private AudioClip m_noMatchDislikeClip;
	[SerializeField]private AudioClip m_matchDislikeClip;

	// Use this for initialization
	void Start () {
		this.m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerProfile>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ButtonPress();
        }
	}

	public void ButtonPress(){
		if (Button.ms_active) {
            Profile.StopHighlightingParts(m_player, MatchProfile.ms_currentMatch);

            //if not match play tearing bubble wrap sound
            if (this.m_player.CheckForMatch(MatchProfile.ms_currentMatch)) {
                //Profile.HighLightMatchingParts(m_player, MatchProfile.ms_currentMatch);
                PlayerProfile.RemoveMatch();
                this.m_audioSource.PlayOneShot(m_matchDislikeClip);
                MatchManager.SaveMatch(MatchProfile.ms_currentMatch.gameObject);
                //this.m_player.DropNoMatchCorrections(MatchProfile.ms_currentMatch);
                MatchManager.ShowWhyYouFailed(true);
            } else {
                //Profile.HighLightConflicts(m_player, MatchProfile.ms_currentMatch);
                //else if it wasn't a match play popping noise
                this.m_audioSource.PlayOneShot(m_noMatchDislikeClip);
                MatchManager.SaveReject(MatchProfile.ms_currentMatch.gameObject);
                MatchManager.InstantiateNewMatch(false);

            }
        }
    }
}
