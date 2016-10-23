using UnityEngine;
using System.Collections;

public class BodyPartSlot : MonoBehaviour {

	[SerializeField]private BodyPartSlot.BodyPartType m_slotType;
	public BodyPart m_parentPart;
	public int m_depth;

	public enum BodyPartType{
		Arm,
		RightArm,
		LeftArm,
		Ear,
		RightEar,
		LeftEar,
		Leg,
		RightLeg,
		LeftLeg,
		Head,
		Body
	}

	public BodyPartSlot.BodyPartType GetBodyPartType(){
		return m_slotType;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public BodyPart AddPart(BodyPart bp, BodyPart parentPart, float minRotation = 0.0f, float maxRotation = 0.0f){
		
		BodyPart part;
		if (minRotation != 0.0f || maxRotation != 0.0f) {
			part = (GameObject.Instantiate (bp.gameObject, this.transform.position,transform.rotation) as GameObject).GetComponent (typeof(BodyPart)) as BodyPart;
			part.transform.Rotate(new Vector3(0,0,Random.Range(minRotation,maxRotation)));
		}
		else{
			part = (GameObject.Instantiate (bp.gameObject, this.transform.position, bp.transform.localRotation) as GameObject).GetComponent (typeof(BodyPart)) as BodyPart;
		}
		if (parentPart != null) {
			parentPart.AddChildPart (part);
		}

        part.transform.position = new Vector3(part.transform.position.x, part.transform.position.y, part.transform.position.z - 1.0f);



        part.transform.parent = this.transform;
		part.transform.localScale = part.transform.localScale * 4.0f;


		return part;
	}
}
