using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game Me;

	public PanelCenter PanelCenter;
	public PanelSelectModule PanelSelectModule;
	public PanelWindow PanelWindow;

	public GameState GameState;
	public float LastUpdate;

	// Use this for initialization
	void Awake () {
		Screen.fullScreen = false;
		Me = this;
		gameObject.FindByName<Transform>("PanelMinigame").gameObject.SetActive(true);
		ChangePanel(typeof(PanelSelectModule));
	}

	void Update () {
		if (GameState != null ) {
			LastUpdate = Time.time;
			GameState.Update(Time.deltaTime);
		}
	}

	internal PanelWindow OpenWindow() {
		GameObject pw = Instantiate(PanelWindow.gameObject, transform, true) as GameObject;
		pw.SetActive(true);
		return pw.GetComponent<PanelWindow>();
	}

	internal void CloseWindow(GameObject window) {
		Destroy(window);
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
		Schedule schedule = XmlLoader.LoadSchedule(model.GetElementsByTagName("schedule")[0], situations, true);
		XElement xElement = XElement.Parse(Resources.Load<TextAsset>(modelDir).text);
		List<Plot.Element> plotElements = XmlLoader.LoadPlot(xElement.Elements().FirstOrDefault(t => t.Name == "plot"), parameters, situations);

		GameState = new GameState(parameters, situations, schedule, new Model(moduleId, XmlLoader.LoadTime(model, parameters)), new Plot(plotElements));
		GameState.GameTimeChangeListeners.Add(PanelCenter);

		PanelCenter.Init(GameState);
		ChangePanel(typeof(PanelCenter));
	}

	internal void EndGame(EndCondition endCondition) {
		GameState.GameHasEnded = true;
		ChangePanel(typeof(PanelSelectModule));
		OpenWindow().OpenText("Koniec gry: \n" + endCondition.GetText());
		HighScore.SaveGameScore(GameState.Model.Id, (int)GameState.GameTime);
	}

	//this is used by button in GUI
	public void ChangeToPanelCenter() {
		ChangePanel(typeof(PanelCenter));
	}

	private void ChangePanel(Type type) {
		PanelCenter.gameObject.SetActive(type == typeof(PanelCenter));
		PanelSelectModule.gameObject.SetActive(type == typeof(PanelSelectModule));
	}
}
