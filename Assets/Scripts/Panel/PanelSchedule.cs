using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class PanelSchedule : MonoBehaviour, Schedule.ScheduleUpdateListener {

	private GameState GameState;

	internal void Init(GameState gameState) {
		//Schedule schedule, List<Situation> situations, List<Parameter> parameters

		GameState = gameState;
		List<Parameter> parameters = GameState.Parameters;
		Schedule schedule = GameState.Schedule;
		List<Situation> situations = GameState.Situations;

		//prepare 24 hours
		Transform hours = transform.gameObject.FindByName<Transform>("Hours");
		int i = 0;
		foreach (Transform hour in hours) {
			hour.GetComponentsInChildren<Text>()[0].text = "" + i.ToString("00");
			//add situations
			ButtonSchedule buttonSchedule = hour.GetComponentInChildren<ButtonSchedule>();
			ScheduledSituation s = schedule.getSituationForHour(i);
			buttonSchedule.Init(s != null ? s.Situation : null, s != null ? s.Permament : false, this, i);
			i++;
		}

	}

	public void ScheduleUpdated(List<ScheduledSituation> situations) {
		List<ButtonSchedule> scheduleButtons = new List<ButtonSchedule>();
		Transform panelHours = gameObject.FindByName<Transform>("Hours");
		foreach (Transform hour in panelHours) {
			scheduleButtons.Add(hour.GetComponentInChildren<ButtonSchedule>());
		}

		int i = 0;
		foreach (ButtonSchedule bs in scheduleButtons) {
			Situation s = null;
			bool isPermament = false;
			ScheduledSituation forThisHour = situations.FirstOrDefault(t => t.From == i);
			if (forThisHour != null) {
				s = forThisHour.Situation;
				isPermament = forThisHour.Permament;
			}
			bs.Init(s, isPermament, this, i);
			i++;
		}
	}
}
