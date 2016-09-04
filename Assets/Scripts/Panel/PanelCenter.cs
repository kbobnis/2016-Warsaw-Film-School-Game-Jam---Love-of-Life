using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCenter : MonoBehaviour {

	public Transform SchedulePeak;
	public PanelSituation PanelSituation;
	public CenterPanelParameters PanelParameters;

	public Situation ActualSituation;
	public Schedule Schedule;

	internal void UpdateSchedule() {

		int i = 0;
		foreach(Transform scheduleHour in SchedulePeak) {
			ScheduledSituation s = Schedule.getSituationForHour(i);
			scheduleHour.GetComponentInChildren<Text>().text = s!=null?s.Situation.Text:"";
			i++;
		}
		
	}

	internal void HourHasChanged(int newHour) {
		ScheduledSituation ss = Schedule.getSituationForHour(newHour);
		Situation s = null;
		if (ss == null) {
			s = Schedule.DefaultSituation;
		} else {
			s = ss.Situation;
		}
		PanelSituation.HourHasChanged(newHour, s);
	}

	internal void Init(Schedule schedule, List<Parameter> parameters) {
		Schedule = schedule;
		PanelParameters.Init(parameters);
	}
}
