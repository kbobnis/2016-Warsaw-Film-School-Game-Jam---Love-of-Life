using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PanelCenter : MonoBehaviour {

	public Transform SchedulePeak;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	internal void UpdateSchedule(Schedule schedule) {

		int i = 0;
		foreach(Transform scheduleHour in SchedulePeak) {
			ScheduledSituation s = schedule.getSituationForHour(i);
			scheduleHour.GetComponentInChildren<Text>().text = s!=null?s.Situation.Text:"";
			i++;
		}
		
	}
}
