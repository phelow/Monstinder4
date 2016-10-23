using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BodyPart : MonoBehaviour {

	//TODO: refactor into two dictionaries of lists, this is rediculous
	//TODO: add other bodyparts
	private BodyPartSlot [] slots;
	private static Dictionary<BodyPartSlot.BodyPartType, List<GameObject>> bodyParts;

	public BoxCollider2D m_collider;

	private static Dictionary<BodyPartSlot.BodyPartType, List<GameObject>> usableBodyParts; //TODO: mae
	public static float ms_placementTolerance = .01f;

	[SerializeField]private List<BodyPart> theseBodyParts; //todo name
	[SerializeField]private ElementType m_type;
	[SerializeField]private float m_minRotation = -30;
	[SerializeField]private float m_maxRotation = 30;
	[SerializeField]private SpriteRenderer m_spriteRenderer;
	[SerializeField]private Color m_origColor;

    public Sprite m_setsprite = null;

	private const float ms_collisionTolerance = 0.01f;

	void Start(){
		m_origColor = m_spriteRenderer.color;
	}

    public void SetCollider(BoxCollider2D collider)
    {
        this.m_collider = collider;
    }


    public Color GetColor(){
		return m_origColor;
	}

	public SpriteRenderer GetSpriteRenderer(){
		return m_spriteRenderer;
	}


	[SerializeField]private BodyPartSlot.BodyPartType m_bodyType;

	public enum ElementType{
		Fire,
		Water,
		Plant,
		Earth,
		Spirit,
		Poison,
		Dark,
		Light,
		Bug,
		Metal,
        Dragon,
        Glitch,
        Psychic,
        Ice
	}

	public static string GetElementTypeString(ElementType et){
		return et.ToString ();
	}

	public float MinRotation(){
		return m_minRotation;
	}

	public float MaxRotation(){
		return m_maxRotation;
	}

	public enum Orientation{
		Left,
		Right,
		Neutral
	}

	public void AddChildPart(BodyPart bp){
		if (this.theseBodyParts == null) {
			this.theseBodyParts = new List<BodyPart> ();
		}

		this.theseBodyParts.Add (bp);
	}

	public HashSet<BodyPart.ElementType> CountTypes(ref HashSet<BodyPart.ElementType> types){
		if (types == null) {
			types = new HashSet<BodyPart.ElementType> ();
		}

		types.Add (this.m_type);

		foreach (BodyPart bp in this.theseBodyParts) {
			bp.CountTypes (ref types);
		}

		return types;
	}

	public void InitAndGenerateBody(){
		Init ();

		transform.localScale = new Vector3 (3.0f, 3.0f, .1f);
		GenerateBody ();
	}

	public List<BodyPart> GetAllBodyParts(){
		List<BodyPart> allParts = new List<BodyPart> ();

		allParts.AddRange (this.theseBodyParts);
		foreach (BodyPart bp in this.theseBodyParts) {
			allParts.AddRange (bp.GetAllBodyParts ());
		}

		return allParts;
	}

	// Use this for initialization
	public static void Init () {
		List<BodyPart.ElementType> usableElements = SessionManager.AvailableTypes ();

		if (bodyParts == null) {
			bodyParts = new Dictionary<BodyPartSlot.BodyPartType, List<GameObject>> ();
			for (int i = 0; i <=  (int)BodyPartSlot.BodyPartType.Body; i++) {
				bodyParts [(BodyPartSlot.BodyPartType)i] = Resources.LoadAll (((BodyPartSlot.BodyPartType)i).ToString(), typeof(GameObject)).Cast<GameObject>().ToList();
			}
		}
		usableBodyParts = new Dictionary<BodyPartSlot.BodyPartType, List<GameObject>> ();

		for (int i = 0; i <= (int)BodyPartSlot.BodyPartType.Body; i++) {
			List<GameObject> tmp = new List<GameObject> ();
			foreach (GameObject bp in bodyParts [(BodyPartSlot.BodyPartType)i]) {
				if (usableElements.Contains (bp.GetComponent<BodyPart>().GetElementType ())) {
					tmp.Add (bp);
				}
			}
			usableBodyParts.Add((BodyPartSlot.BodyPartType)i,tmp);

		}
	}

	public BodyPartSlot.BodyPartType GetBodyPartType(){
		return m_bodyType;
	}

	public ElementType GetElementType(){
		return m_type;
	}

	public BodyPartSlot []  GetSlots(){
		return this.GetComponentsInChildren<BodyPartSlot> ().ToArray();
	}

		
	public IEnumerator GenerateBodyCoroutine(int depth = 0, Orientation orientation = Orientation.Neutral){

		BodyPart.RemoveConflicts (this);
		if (orientation == Orientation.Neutral) {
			switch (this.GetBodyPartType ()) {
			case BodyPartSlot.BodyPartType.LeftArm:
				orientation = Orientation.Left;
				break;
			case BodyPartSlot.BodyPartType.LeftEar:
				orientation = Orientation.Left;
				break;
			case BodyPartSlot.BodyPartType.LeftLeg:
				orientation = Orientation.Left;
				break;
			case BodyPartSlot.BodyPartType.RightArm:
				orientation = Orientation.Right;
				break;
			case BodyPartSlot.BodyPartType.RightEar:
				orientation = Orientation.Right;
				break;
			case BodyPartSlot.BodyPartType.RightLeg:
				orientation = Orientation.Right;
				break;
			}
		}

		slots = this.GetComponentsInChildren<BodyPartSlot> ();

		if (slots == null) {
			yield break;
		}

		foreach (BodyPartSlot slot in slots) {
			bool isHead = slot.GetBodyPartType () == BodyPartSlot.BodyPartType.Head;

			if (Random.Range (0, depth) > Random.Range (2, 5) && isHead == false) {
				continue;
			}

			bool shouldContinue = false;
			if (isHead == false) {

				Collider2D [] hits = Physics2D.OverlapCircleAll(new Vector2(slot.transform.position.x,slot.transform.position.y),ms_collisionTolerance);
				Collider2D thisCollider = this.GetComponent<Collider2D> ();
				Collider2D spriteCollider = this.m_spriteRenderer.GetComponent<Collider2D> ();

				foreach (Collider2D hit in hits) {
					if (!((thisCollider != null && hit == thisCollider) || (spriteCollider != null && spriteCollider == hit))) {
						shouldContinue = true;
						break;
					}
				}

			}

			if (shouldContinue) {
				continue;
			}

			BodyPartSlot.BodyPartType bodyPartSlotType = slot.GetBodyPartType ();

			List<GameObject> parts = GetUsableParts(slot.GetBodyPartType ());

			if (parts == null) {
				yield break;
			}

			//TODO: remove from parts all parts that cause a weakness
			int partCount = parts.Count;

			if (partCount == 0) {
				continue;
			}

			BodyPart part = slot.AddPart (parts [Random.Range (0, partCount)].GetComponent<BodyPart>(),this,this.MinRotation(),this.MaxRotation());


			//if the slot is within a trigger do not create a new gameobjec
			part.GenerateBody (++depth, orientation);

		}

		yield return new WaitForEndOfFrame ();
	}

	public static List<GameObject> GetUsableParts(BodyPartSlot.BodyPartType bodyPartSlot,bool isPlayer = false, HashSet<BodyPart.ElementType> types = null){

        List<GameObject> parts = new List<GameObject>();
        parts.AddRange(usableBodyParts [bodyPartSlot]);

		if (bodyPartSlot == BodyPartSlot.BodyPartType.LeftArm || bodyPartSlot == BodyPartSlot.BodyPartType.RightArm) {
			parts.AddRange (usableBodyParts [BodyPartSlot.BodyPartType.Arm]);
		} else if (bodyPartSlot == BodyPartSlot.BodyPartType.LeftLeg || bodyPartSlot == BodyPartSlot.BodyPartType.RightLeg) {
			parts.AddRange (usableBodyParts [BodyPartSlot.BodyPartType.Leg]);
		} else if (bodyPartSlot == BodyPartSlot.BodyPartType.LeftEar || bodyPartSlot == BodyPartSlot.BodyPartType.RightEar) {
			parts.AddRange (usableBodyParts [BodyPartSlot.BodyPartType.Ear]);
		}

		if (isPlayer) {
			int minTypes = Mathf.Min(PlayerPrefs.GetInt ("Level", 0)/2, BodyPart.ElementType.GetNames(typeof(BodyPart.ElementType)).Length);

			if (types == null || minTypes < types.Count) {
				return parts;
			}

			foreach(BodyPart.ElementType type in types){
				parts = RemoveAllOfType(parts,type);
			}

            if(parts.Count == 0)
            {
                parts = usableBodyParts[bodyPartSlot];
                if (bodyPartSlot == BodyPartSlot.BodyPartType.LeftArm || bodyPartSlot == BodyPartSlot.BodyPartType.RightArm)
                {
                    parts.AddRange(usableBodyParts[BodyPartSlot.BodyPartType.Arm]);
                }
                else if (bodyPartSlot == BodyPartSlot.BodyPartType.LeftLeg || bodyPartSlot == BodyPartSlot.BodyPartType.RightLeg)
                {
                    parts.AddRange(usableBodyParts[BodyPartSlot.BodyPartType.Leg]);
                }
                else if (bodyPartSlot == BodyPartSlot.BodyPartType.LeftEar || bodyPartSlot == BodyPartSlot.BodyPartType.RightEar)
                {
                    parts.AddRange(usableBodyParts[BodyPartSlot.BodyPartType.Ear]);
                }
            }
		}

		return parts;
	}

	private static List<GameObject> RemoveAllOfType(List<GameObject> parts, BodyPart.ElementType type){
		List<GameObject> toRemove = new List<GameObject> ();

		foreach (GameObject part in parts) {
			if (part.GetComponent<BodyPart> ().GetElementType() == type) {
				toRemove.Add (part);
			}
		}

		foreach (GameObject part in toRemove) {
			parts.Remove (part);
		}

		return parts;
	}

	public void GenerateBody(int depth = 0, Orientation orientation = Orientation.Neutral ){

		this.StartCoroutine (this.GenerateBodyCoroutine (depth, orientation));
	}

	public static void RemoveConflicts(BodyPart part){
		List<GameObject> toRemove;

		foreach (BodyPartSlot.BodyPartType key in usableBodyParts.Keys) {
			toRemove = new List<GameObject> ();
			foreach (GameObject go in usableBodyParts[key]) {
				if (PlayerProfile.Conflicts (part, go.GetComponent<BodyPart> ())) {
					toRemove.Add (go);
				}
			}

			foreach (GameObject go in toRemove) {
				usableBodyParts [key].Remove (go);
			}
		}
	}

	public void CalculateScore (ref  int [] typeScores){
		typeScores [(int)m_type]++;
		foreach (BodyPart part in theseBodyParts) {
			part.CalculateScore (ref typeScores);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
