using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game Me;
	public Model Model;
	public Schedule ScheduledSituations;

	public PanelSchedule PanelSchedule;
	public PanelCenter PanelCenter;

	public float GameTime;
	public ScheduledSituation ActualSituation;
	public List<Parameter> Parameters;
	public int GameTimeNormalized {
		get {
			return (int)GameTime % 24;
		}
	}
	public float ActualGameSpeed = 1f;

	// Use this for initialization
	void Awake () {
		Me = this;
		PanelCenter.gameObject.SetActive(false);
		PanelSchedule.gameObject.SetActive(true);

		XmlDocument model = new XmlDocument();
		string modelDir = "love_of_life";

		model.LoadXml(Resources.Load<TextAsset>(modelDir).text);
		//getting rid of comments
		string pattern = "(<!--.*?--\\>)";
		model.InnerXml = Regex.Replace(model.InnerXml, pattern, string.Empty, RegexOptions.Singleline);

		Parameters = XmlLoader.LoadParameters(model);
		List<Situation> situations = XmlLoader.LoadSituations(model, Parameters);
		ScheduledSituations = XmlLoader.LoadSchedule(model, situations);

		TimeChanges timeChanges = XmlLoader.LoadTime(model, Parameters);
		ActualGameSpeed = timeChanges.NormalSpeed;
		ActualSituation = ScheduledSituations.getSituationForHour(0, true);

		Model = new Model(timeChanges);

		PanelSchedule.Init(ScheduledSituations, situations, Parameters);
		PanelCenter.Init(PanelSchedule.Schedule, Parameters);
		HourHasChanged(0);
	}

	internal void EndGame(Parameter parameter) {
		
	}

	// Update is called once per frame
	void Update () {
		int gameTimeNormalized = GameTimeNormalized;
		float timeDelta = Time.deltaTime / 60f * ActualGameSpeed;
		UpdateParameters(timeDelta);
		
		GameTime += timeDelta;
		int gameTimeNormalizedAfter = GameTimeNormalized;
		if(gameTimeNormalizedAfter != gameTimeNormalized) {
			HourHasChanged(gameTimeNormalizedAfter);
		}

	}

	private void UpdateParameters(float timeDelta) {
		//clear all parameter negative values
		foreach(Parameter p in Parameters) {
			p.IsUsedAndIsZero = false;
			p.IsDraggedDownBy.Clear();
			p.IsDraggingDown = false;
		}

		foreach(Change change in ActualSituation.Situation.Changes) {
			change.What.UpdateWithChange(change, timeDelta);
		}

		//Model.TimeChanges
		foreach(Change change in Model.TimeChanges.Changes) {
			change.What.UpdateWithChange(change, timeDelta);
		}

	}

	private void HourHasChanged(int hour) {
		PanelCenter.HourHasChanged(hour);
	}

	public void Faster() {
		
	}

	public void ChangeToPanelSchedule() {
		PanelCenter.gameObject.SetActive(false);
		PanelSchedule.gameObject.SetActive(true);
	}

	public void ChangeToPanelCenter() {
		PanelCenter.gameObject.SetActive(true);
		PanelCenter.UpdateSchedule();
		PanelSchedule.gameObject.SetActive(false);
	}
}
