﻿using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class PanelSchedule : MonoBehaviour {

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	internal void Init(Schedule schedule, List<Situation> situations) {
		//prepare 24 hours
		Transform hours = transform.GetChild(1).GetChild(0).GetChild(1);
		int i = 0;
		foreach (Transform hour in hours) {
			hour.GetComponentsInChildren<Text>()[0].text = "" + i.ToString("00");
			//add situations
			ButtonSchedule buttonSchedule = hour.GetComponentInChildren<ButtonSchedule>();
			ScheduledSituation s = schedule.getSituationForHour(i);
			buttonSchedule.Init(s!=null?s.Situation:null, s!=null?s.Permament:false, this);
			i++;
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
		foreach (Transform hour in transform.GetChild(1).GetChild(0).GetChild(1)) {
			scheduleButtons.Add(hour.GetComponentInChildren<ButtonSchedule>());
		}
		return scheduleButtons;
	}
}
