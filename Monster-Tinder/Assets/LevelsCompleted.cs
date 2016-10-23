using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelsCompleted : MonoBehaviour {
    [SerializeField]
    private UnityEngine.UI.Text m_text;
	// Use this for initialization
	void Start () {
        m_text.text = "";
        
        for(int i = 0; i < 100; i++)
        {
            if(PlayerPrefs.GetInt(i + "completed",0) > 0)
            {
                m_text.text += "\n" + i + ":" + PlayerPrefs.GetInt(i + "completed", 0);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
