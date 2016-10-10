using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class PanelSchedule : MonoBehaviour, Schedule.ScheduleUpdateListener {

	private GameState GameState;

	internal void Init(GameState gameState) {
		//Schedule schedule, List<Situation> situations, List<Parameter> parameters

		GameState = gameState;
		List<Parameter> parameters = GameState.Parameters;
		Schedule schedule = GameState.Schedule;
		List<Situation> situations = GameState.Situations;
		
		//prepare parameters
		Transform parametersTransform = transform.GetChild(0);
		{
			int i = 0;
			foreach (Transform parameter in parametersTransform) {
				parameter.gameObject.SetActive(false);
				if (parameters.Count > i) {
					parameter.gameObject.SetActive(true);
					parameter.GetComponent<PanelParameter>().Init(parameters[i]);
				}
				i++;
			}
		}

		{
			//prepare 24 hours
			Transform hours = transform.GetChild(1).GetChild(0).GetChild(1);
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
		//prepare situation buttons
		Transform situationsTransform = transform.GetChild(1).GetChild(1);
		List<Situation> situationsAvailable = situations.Where(t => t.Selectable).ToList();
		int j = 0;
		foreach (Transform situation in situationsTransform) {
			situation.gameObject.SetActive(false);
			if (j < situationsAvailable.Count) {
				situation.gameObject.SetActive(true);
				situation.GetComponent<ButtonSelectableSituation>().Init(situationsAvailable[j], this);
			}
			j++;
		}
	}

	internal void ScheduleUpdated(int myHour, Situation ss) {
		GameState.Schedule.Update(myHour, ss);
	}

	internal void PropositionWasSelected(Situation situation) {
		IEnumerable<ButtonSchedule> scheduleButtons = GetAllScheduleButtons();
		foreach (ButtonSchedule bs in scheduleButtons) {
			bs.PropositionWasSelected(situation);
		}
	}

	internal void UnselectProposed() {
		IEnumerable<ButtonSchedule> scheduleButtons = GetAllScheduleButtons();
		foreach(ButtonSchedule bs in scheduleButtons) {
			bs.UnselectProposed();
		}
	}

	private List<ButtonSchedule> GetAllScheduleButtons() {
		List<ButtonSchedule> scheduleButtons = new List<ButtonSchedule>();
		Transform panelHours = gameObject.FindByName<Transform>("Hours");
		foreach (Transform hour in panelHours) {
			scheduleButtons.Add(hour.GetComponentInChildren<ButtonSchedule>());
		}
		return scheduleButtons;
	}

	public void ScheduleUpdated(List<ScheduledSituation> situations) {
		UpdateSchedule(situations);
	}

	private void UpdateSchedule(List<ScheduledSituation> situations) {
		IEnumerable<ButtonSchedule> scheduleButtons = GetAllScheduleButtons();
		int i = 0;
		foreach (ButtonSchedule bs in scheduleButtons) {
			bs.Init(situations[i].Situation, situations[i].Permament, this, i);
			i++;
		}
	}
}
