using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ButtonSelectableSituation : MonoBehaviour {

	private Situation Situation;
	private PanelSchedule PanelSchedule;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	internal void Init(Situation situation, PanelSchedule panelSchedule) {
		Situation = situation;
		PanelSchedule = panelSchedule;
		GetComponentInChildren<Text>().text = situation.Text;
	}

	public void Selected() {
		PanelSchedule.PropositionWasSelected(Situation);
	}
}
