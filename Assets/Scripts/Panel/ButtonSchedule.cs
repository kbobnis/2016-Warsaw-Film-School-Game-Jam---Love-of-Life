using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ButtonSchedule : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	internal void Init(ScheduledSituation s) {
		GetComponentInChildren<Text>().text = s.Situation.Text;
		GetComponent<Image>().color = s.Permament ? Color.red : Color.white;
	}
}
