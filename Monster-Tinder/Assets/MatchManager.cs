using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MatchManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_match;
    private static MatchManager ms_instance;

    [SerializeField]private GameObject m_matchDockingPoint;
	[SerializeField]private GameObject m_dockedMatch;

    public List<GameObject> m_rejectedMatches;
    public List<GameObject> m_acceptedMatches;

    private const float mc_seekMagnitude = 1000.0f;
	private const float mc_distanceMaximum = 2.0f;

    [SerializeField]
    private GameObject m_originalMatch;

    private const float mc_thrownScaleFactor = 150.0f;

    private const float mc_randomFactor = 200.0f;

    [SerializeField]
    private UnityEngine.UI.Button m_matchButton;
    [SerializeField]
    private UnityEngine.UI.Button m_noButton;

    // Use this for initialization
    void Start () {
		ms_instance = this;
        DontDestroyOnLoad(this.gameObject);

        m_acceptedMatches = new List<GameObject>();
        m_rejectedMatches = new List<GameObject>();
        MatchProfile.ms_currentMatch = m_originalMatch.GetComponent<MatchProfile>();

    }

    public static int NumMatches()
    {
        if(ms_instance == null)
        {
            return -1;
        }

        return ms_instance.m_acceptedMatches.Count;
    }

    public static List<GameObject> GetMatches()
    {
        return ms_instance.m_acceptedMatches;
    }

    public static void SaveMatch(GameObject match)
    {

        GameObject copy = GameObject.Instantiate(match, ms_instance.gameObject.transform, false) as GameObject;

        copy.GetComponent<Profile>().TellNotToInit();
        ms_instance.m_acceptedMatches.Add(copy);
    }

    public static void InstantiateNewMatch(bool isMatch)
    {
        GameObject go = MatchProfile.ms_currentMatch.NextMatchPostion();
        Vector3 position = go.transform.position;
        Quaternion rotation = go.transform.rotation;

        MatchManager.DockMatch(MatchProfile.ms_currentMatch.gameObject, isMatch);
        MatchProfile.ms_currentMatch = (GameObject.Instantiate(ms_instance.m_match, position, rotation) as GameObject).GetComponent<MatchProfile>();
    }

    public void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().name == "Failure" || SceneManager.GetActiveScene().name == "Success")
        {
            Destroy(gameObject);
        }
    }

    public static void SaveReject(GameObject match)
    {
        GameObject copy = GameObject.Instantiate(match, ms_instance.gameObject.transform, false) as GameObject;

        copy.GetComponent<Profile>().TellNotToInit();
        ms_instance.m_rejectedMatches.Add(copy);
        //Flash issues
    }

    public static void ShowWhyYouFailed(bool isMatch)
    {
        ms_instance.StartCoroutine(ms_instance.TutorializeFailure(isMatch));
    }

    public IEnumerator TutorializeFailure(bool isMatch)
    {
        this.m_noButton.interactable = false;
        this.m_matchButton.interactable = false;
        UnityEngine.Cursor.visible = false;
        PlayerProfile.PrintMatchText();
        if (isMatch)
        {
            yield return Profile.HighlightMatchingPartsOneAtATime(PlayerProfile.GetPlayer(), MatchProfile.ms_currentMatch);

        }
        else {
            yield return Profile.HighlightClashingPartsOneAtATime(PlayerProfile.GetPlayer(), MatchProfile.ms_currentMatch);
        }
        Profile.StopHighlightingParts(PlayerProfile.GetPlayer(), MatchProfile.ms_currentMatch);

        if (isMatch)
        {
            Profile.HighLightMatchingParts(PlayerProfile.GetPlayer(), MatchProfile.ms_currentMatch);

        }
        else {
            Profile.HighLightConflicts(PlayerProfile.GetPlayer(), MatchProfile.ms_currentMatch);
        }
         new WaitForSeconds(3.0f);
        PlayerProfile.ClearMatchText();
        PlayerProfile.RemoveMatch();
        Profile.StopHighlightingParts(PlayerProfile.GetPlayer(), MatchProfile.ms_currentMatch);


        UnityEngine.Cursor.visible = true;
        InstantiateNewMatch(isMatch);
        this.m_noButton.interactable = true;
        this.m_matchButton.interactable = true;
    }

	private void ReleaseMatch(){
		if (m_dockedMatch == null) {
			return;
		}

        if (m_dockedMatch.GetComponent<MatchProfile>().GetIsMatch() == false)
        {
            StartCoroutine(DestroyMatch(m_dockedMatch));
        }
        else
        {
            StartCoroutine(HideMatch(m_dockedMatch));
        }
		m_dockedMatch = null;
	}

    private IEnumerator HideMatch(GameObject go)
    {

        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();


        SpriteRenderer[] sprites = go.GetComponentsInChildren<SpriteRenderer>();
        
        Color halfClear;

        halfClear = new Color(Color.white.r, Color.white.g, Color.white.b, Color.clear.a + Random.Range(0.3f, 0.7f));

        rb.isKinematic = false;
        rb.AddForce(new Vector2(Mathf.Clamp(Random.Range(-mc_randomFactor, mc_randomFactor),-mc_thrownScaleFactor, mc_thrownScaleFactor), Mathf.Clamp(Random.Range(-mc_randomFactor, mc_randomFactor), -mc_thrownScaleFactor, mc_thrownScaleFactor)));
                
        go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z - Random.Range(-1000.0f, -2000.0f));
        rb.gravityScale = 0.0f;
        Vector3 origScale = go.transform.localScale;
        Vector3 origPosition = go.transform.position;
        Vector3 shrunkScale = go.transform.localScale *= Random.Range(0.7f, 0.3f);

        float scaleTime = Random.Range(0.5f, 3.0f);
        float t = scaleTime;
        while(t > 0.0f)
        {
            rb.drag = Vector3.Distance(origPosition, transform.position)/ 850.0f;

            t -= Time.deltaTime;

            foreach(SpriteRenderer sprite in sprites)
            {
                sprite.color = Color.Lerp(halfClear, Color.white, t / scaleTime);
            }

            go.transform.localScale = Vector3.Lerp(shrunkScale, origScale, t/scaleTime);

            yield return new WaitForEndOfFrame();
        }


        yield return new WaitForSeconds(2.0f);
    }

    private IEnumerator DestroyMatch(GameObject go){

		Rigidbody2D rb = go.GetComponent<Rigidbody2D> ();

		rb.isKinematic = false;
		rb.AddForce (new Vector2(Random.Range(-500.0f,500.0f),Random.Range(-500.0f,500.0f)));
		rb.gravityScale = 10.0f;
		yield return new WaitForSeconds (2.0f);
		Destroy (go);
	}

    private IEnumerator ShrinkMatchCoroutine(GameObject match)
    {
        Vector3 origScale = match.transform.localScale;
        float shrinkTime = 0.4f;
        float t = 0.0f;
        while(t < shrinkTime)
        {
            t += Time.deltaTime;
            match.transform.localScale = Vector3.Lerp(origScale, origScale * .5f, t/shrinkTime);
            yield return new WaitForEndOfFrame();
        }
    }


    private IEnumerator DockMatchCoroutine(GameObject match,bool isMatch)
    {
        m_dockedMatch = match;
        Rigidbody2D rb = match.GetComponent<Rigidbody2D>();
        BoxCollider2D[] colliders = match.GetComponentsInChildren<BoxCollider2D>();

        for (int i = 0; i < colliders.Length; i++)
        {
            Destroy(colliders[i]);
        }
        //Add a random force to the match

        rb.isKinematic = false;
        rb.AddForce(new Vector2(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f)));
        match.transform.position = new Vector3(match.transform.position.x, match.transform.position.y, m_matchDockingPoint.transform.position.z);

        //seek the docking point until within a current range of it.
        while (match != null && Vector2.Distance(new Vector2(match.transform.position.x, match.transform.position.y),
            new Vector2(m_matchDockingPoint.transform.position.x, m_matchDockingPoint.transform.position.y)) > mc_distanceMaximum)
        {
            Vector2 force = (new Vector2(m_matchDockingPoint.transform.position.x, m_matchDockingPoint.transform.position.y)
                - new Vector2(match.transform.position.x, match.transform.position.y)).normalized * mc_seekMagnitude * Time.deltaTime;

            rb.AddForce(force);

            Debug.DrawRay(match.transform.position, force);
            yield return new WaitForEndOfFrame();
        }

        //Lock the position
        rb.isKinematic = true;

        if (isMatch == false)
        {
            StartCoroutine(DestroyMatch(match));
        }
        else
        {
            StartCoroutine(HideMatch(match));
        }
    }

	public static void DockMatch(GameObject match, bool isMatch){

        match.GetComponent<MatchProfile>().SetIsMatch(isMatch);


        //release previously docked match

		//Dock the new match
		ms_instance.StartCoroutine (ms_instance.DockMatchCoroutine (match, isMatch));
        ms_instance.StartCoroutine(ms_instance.ShrinkMatchCoroutine(match));

	}
}
