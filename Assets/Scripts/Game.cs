using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game Me;

	public PanelSchedule PanelSchedule;
	public PanelCenter PanelCenter;
	public PanelSelectModule PanelSelectModule;

	public GameState GameState;

	// Use this for initialization
	void Awake () {
		Screen.fullScreen = false;
		Me = this;
		ChangeToPanelSelectModule();
	}

	void Update () {
		if (GameState != null) {
			GameState.Update(Time.deltaTime);
		}
	}

	public void ChangeToPanelSchedule() {
		PanelCenter.gameObject.SetActive(false);
		PanelSchedule.gameObject.SetActive(true);
	}

	public void ChangeToPanelCenter() {
		PanelCenter.gameObject.SetActive(true);
		PanelCenter.UpdateSchedule();
		PanelSchedule.gameObject.SetActive(false);
		PanelSelectModule.gameObject.SetActive(false);
	}

	private void ChangeToPanelSelectModule() {
		PanelCenter.gameObject.SetActive(false);
		PanelSchedule.gameObject.SetActive(false);
		PanelSelectModule.gameObject.SetActive(true);
	}

	internal void LoadModule(string moduleId) {
		XmlDocument model = new XmlDocument();
		string modelDir = moduleId;

		model.LoadXml(Resources.Load<TextAsset>(modelDir).text);
		//getting rid of comments
		string pattern = "(<!--.*?--\\>)";
		model.InnerXml = Regex.Replace(model.InnerXml, pattern, string.Empty, RegexOptions.Singleline);

		List<Parameter> parameters = XmlLoader.LoadParameters(model);
		List<Situation> situations = XmlLoader.LoadSituations(model, parameters);
		Schedule scheduledSituations = XmlLoader.LoadSchedule(model, situations);

		GameState = new GameState(parameters, situations, scheduledSituations, new Model(XmlLoader.LoadTime(model, parameters)));

		PanelSchedule.Init(GameState);
		PanelCenter.Init(GameState);
		ChangeToPanelCenter();
	}

	internal void EndGame() {
		ChangeToPanelSelectModule();
	}

}
