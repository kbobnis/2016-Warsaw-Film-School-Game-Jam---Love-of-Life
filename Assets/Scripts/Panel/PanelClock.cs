using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelClock : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Image>().fillAmount = 1 - (Game.Me.GameTime % 24) / 24f;
	}
}
