using UnityEngine;
using System;
using UnityEngine.UI;

public class PanelEndGame : MonoBehaviour {
	
	internal void Init(Game.EndCondition endCondition) {
		GetComponentsInChildren<Text>()[0].text = endCondition.GetText();
	}

	public void Toggle() {
		gameObject.SetActive(!gameObject.activeSelf);
	}

}
