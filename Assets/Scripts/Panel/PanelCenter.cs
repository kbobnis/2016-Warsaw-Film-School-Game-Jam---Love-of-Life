using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCenter : MonoBehaviour {

	public Transform SchedulePeak;
	public PanelSituation PanelSituation;
	public CenterPanelParameters PanelParameters;

	private int? LastHourOfDay;
	public Situation ActualSituation;
	private GameState GameState;

	void Update() {
		if (GameState != null) {
			int hour = Game.Me.GameState.HourOfDay;
			if (LastHourOfDay != hour) {
				HourHasChanged(hour);
				LastHourOfDay = hour;
			}
		}
	}

	internal void UpdateSchedule() {
		int i = 0;
		foreach (Transform scheduleHour in SchedulePeak) {
			ScheduledSituation s = GameState.Schedule.getSituationForHour(i);
			scheduleHour.GetComponentInChildren<Text>().text = s != null ? s.Situation.Text : "";
			i++;
		}
	}

	internal void HourHasChanged(int newHour) {
		ScheduledSituation ss = GameState.Schedule.getSituationForHour(newHour);
		Situation s = null;
		if (ss == null) {
			s = GameState.Schedule.DefaultSituation;
		} else {
			s = ss.Situation;
		}
		GameState.ActualSituation = new ScheduledSituation(GameState.HourOfDay, 1, s, false);
		PanelSituation.HourHasChanged(newHour, s, GameState);
	}

	internal void Init(GameState gameState) {
		GameState = gameState;
		PanelParameters.Init(gameState.Parameters);
	}
}
