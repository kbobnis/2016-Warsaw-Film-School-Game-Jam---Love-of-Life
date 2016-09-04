using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelSituation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	internal void UpdateActiveSituation(Situation actualSituation) {
		GetComponentsInChildren<Text>()[0].text = actualSituation.Text;
	}

	internal void HourHasChanged(int newHour, Situation ss) {
		UpdateActiveSituation(ss);

	}
}
