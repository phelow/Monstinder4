using UnityEngine;
using System.Collections;

public class MatchProfile : Profile {
	public static MatchProfile ms_currentMatch;
	[SerializeField]private GameObject m_nextMatch;

    [SerializeField]
    private SpriteRenderer m_polaroid;

    private bool m_isMatch;
    
    public bool GetIsMatch()
    {
        return m_isMatch;
    }

    public void SetIsMatch(bool matchStatus)
    {
        m_isMatch = matchStatus;
    }


    public void HidePolaroid()
    {
        if(m_polaroid == null)
        {
            return;
        }
        Destroy(m_polaroid);
    }

	protected override void CacheIfMatchProfile(){
		Debug.Log ("Caching");
	}

    public GameObject NextMatchPostion()
    {
        return m_nextMatch;
    }
}
