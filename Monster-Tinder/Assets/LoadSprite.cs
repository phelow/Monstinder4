using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadSprite : MonoBehaviour {
    [SerializeField]
    private BodyPart m_bp;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer;
    
    private static Dictionary<BodyPartSlot.BodyPartType,Dictionary<BodyPart.ElementType, Object []>> ms_sprites;


    // Use this for initialization
    void Awake ()
    {
        this.m_spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.m_bp = this.transform.parent.GetComponent<BodyPart>();
        this.m_spriteRenderer.sprite = null;
        if (ms_sprites == null)
        {
            ms_sprites = new Dictionary<BodyPartSlot.BodyPartType, Dictionary<BodyPart.ElementType, Object[]>>();
            foreach (BodyPartSlot.BodyPartType bodyPartType in System.Enum.GetValues(typeof(BodyPartSlot.BodyPartType)))
            {
                ms_sprites[bodyPartType] = new Dictionary<BodyPart.ElementType, Object []>();
                foreach (BodyPart.ElementType elementType in System.Enum.GetValues(typeof(BodyPart.ElementType)))
                {
                    ms_sprites[bodyPartType].Add(elementType, Resources.LoadAll<Sprite>("Sprite\\" + bodyPartType.ToString() + "\\" + elementType.ToString()));
                }
            }
        }

        Object[] sprites = ms_sprites[this.m_bp.GetBodyPartType()][this.m_bp.GetElementType()];
        try {
            if (this.m_bp.m_setsprite != null)
            {
                this.m_spriteRenderer.sprite = this.m_bp.m_setsprite;
            }
            else {
                this.m_spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)] as Sprite;
            }
            this.m_bp.m_setsprite = this.m_spriteRenderer.sprite;

        }
        catch
        {
            Debug.LogError(this.m_bp.GetBodyPartType().ToString() + " " + this.m_bp.GetElementType().ToString());
        }
        m_bp.SetCollider(m_spriteRenderer.gameObject.AddComponent<BoxCollider2D>());
    }
	
	// Update is called once per frame
    [ExecuteInEditMode]
	void Update () {
	}
}
