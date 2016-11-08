using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AdvanceOnTimer : MonoBehaviour {
    public float m_curTime = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        m_curTime += Time.deltaTime;

        if(m_curTime > 45.0f)
        {
            SceneManager.LoadScene("Main Menu");
        }

    }
}
