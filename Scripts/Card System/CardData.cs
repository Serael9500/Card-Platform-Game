using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Data")]
public class CardData : ScriptableObject {

    public int id;
    //public int cost;
    [Range(0f, 100f)] public float spawnPercent;
    public List<CardAction> actions;
    
	[Header("Card form vars")]
    public Sprite card_background;
    public string card_title;
    public Sprite card_image;
    public string card_description {
		get {
			string text = "";
			foreach (CardAction action in actions)
				if (action != null)
					text += action.ToString();
			return text;
		}
	}

	[Header("Icon form")]
	public Sprite icon_background;
	public Sprite icon_image;


}