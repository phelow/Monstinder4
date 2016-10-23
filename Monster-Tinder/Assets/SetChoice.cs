using UnityEngine;
using System.Collections;

public class SetChoice : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetChoiceOnDialogSystem()
    {
        this.GetComponent<UnityEngine.UI.Text>().color = Color.white;
        DialogSystem.SetChoice(this.GetComponent<UnityEngine.UI.Text>().text);

    }
}
