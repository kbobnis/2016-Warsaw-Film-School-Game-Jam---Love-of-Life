using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game Me;

	public PanelSchedule PanelSchedule;
	public PanelCenter PanelCenter;

	public GameState GameState;

	// Use this for initialization
	void Awake () {
		Me = this;
		PanelCenter.gameObject.SetActive(true);
		
		PanelSchedule.gameObject.SetActive(false);

		XmlDocument model = new XmlDocument();
		string modelDir = "love_of_life";

		model.LoadXml(Resources.Load<TextAsset>(modelDir).text);
		//getting rid of comments
		string pattern = "(<!--.*?--\\>)";
		model.InnerXml = Regex.Replace(model.InnerXml, pattern, string.Empty, RegexOptions.Singleline);

		List<Parameter> parameters = XmlLoader.LoadParameters(model);
		List<Situation> situations = XmlLoader.LoadSituations(model, parameters);
		Schedule scheduledSituations = XmlLoader.LoadSchedule(model, situations);

		GameState = new GameState(parameters, scheduledSituations, new Model(XmlLoader.LoadTime(model, parameters)));

		PanelSchedule.Init(scheduledSituations, situations, parameters);
		PanelCenter.Init(PanelSchedule.Schedule, parameters);
		
		HourHasChanged(0);
		ChangeToPanelCenter();
	}

	internal void EndGame(Parameter parameter) {
		
	}

	// Update is called once per frame
	void Update () {
		int gameTimeNormalized = GameState.HourOfDay;
		float timeDelta = Time.deltaTime / 60f * GameState.ActualGameSpeed;
		GameState.UpdateParameters(timeDelta);
		
		GameState.GameTime += timeDelta;
		int gameTimeNormalizedAfter = GameState.HourOfDay;
		if(gameTimeNormalizedAfter != gameTimeNormalized) {
			HourHasChanged(gameTimeNormalizedAfter);
		}

	}

	private void HourHasChanged(int hour) {
		PanelCenter.HourHasChanged(hour, GameState);
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
