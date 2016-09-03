using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class PanelSchedule : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	internal void Init(Schedule schedule, List<Situation> situations) {
		//prepare 24 hours
		Transform hours = transform.GetChild(0).GetChild(1);
		int i = 0;
		foreach(Transform hour in hours) {
			hour.GetComponentsInChildren<Text>()[0].text = "" + i.ToString("00");
			//add situations
			ButtonSchedule buttonSchedule = hour.GetComponentInChildren<ButtonSchedule>();
			ScheduledSituation s = schedule.getSituationForHour(i);
			buttonSchedule.gameObject.SetActive(s != null);
			if (s != null) {
				buttonSchedule.Init(s);
			}
			i++;
		}
		//prepare situation buttons
		Transform situationsTransform = transform.GetChild(1);
		List<Situation> situationsAvailable = situations.Where(t => t.Selectable).ToList();
		int j = 0;
		foreach (Transform situation in situationsTransform) {
			situation.gameObject.SetActive(false);
			if (j < situationsAvailable.Count) {
				situation.gameObject.SetActive(true);
				situation.GetComponent<ButtonSelectableSituation>().Init(situationsAvailable[j]);
			}
			j++;
		}

	}
}
