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
	void Awake() {
		Screen.fullScreen = false;
		Me = this;
		gameObject.FindByName<Transform>("PanelMinigame").gameObject.SetActive(true);
		ChangePanel(typeof(PanelSelectModule));
	}
	void Update() {
		if (GameState != null) {
			LastUpdate = Time.time;
			GameState.Update(Time.deltaTime);
		}
	}

	public void QuitAdventure() {
		GameState.GameHasEnded = true;
		ChangePanel(typeof(PanelSelectModule));
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
		string gameHash = HashCalculator.Md5Sum(model.InnerXml);

		List<Parameter> parameters = XmlLoader.LoadParameters(model);
		List<Situation> situations = XmlLoader.LoadSituations(model, parameters);
		Schedule schedule = XmlLoader.LoadSchedule(model.GetElementsByTagName("schedule")[0], situations, true);
		XElement xElement = XElement.Parse(Resources.Load<TextAsset>(modelDir).text);
		int pointsEvery = int.Parse(xElement.Elements().FirstOrDefault(t => t.Name == "gainPoints").Attribute("afterEveryHour").Value);
		List<Plot.Element> plotElements = XmlLoader.LoadPlot(xElement.Elements().FirstOrDefault(t => t.Name == "plot"), parameters, situations);

		GameState = new GameState(parameters, situations, schedule, new Model(moduleId, XmlLoader.LoadTime(model, parameters)), new Plot(plotElements), gameHash, pointsEvery);
		GameState.GameTimeChangeListeners.Add(PanelCenter);
		GameState.ActualPointsChangeLisnters.Add(PanelCenter.gameObject.FindByName<PanelAvailablePoints>("PanelAvailablePoints"));

		PanelCenter.Init(GameState);
		ChangePanel(typeof(PanelCenter));
	}

	internal void EndGame(EndCondition endCondition) {
		if (!GameState.GameHasEnded) {
			GameState.GameHasEnded = true;
			ChangePanel(typeof(PanelSelectModule));
			int gameTime = (int)GameState.GameTime;
			string text = endCondition.GetText();
			GameHighScores scores = HighScore.Load(GameState.Model.Id, GameState.GameHash);
			if (endCondition.GetType() == typeof(EndCondition.Win)) {
				scores = HighScore.Save(GameState.Model.Id, GameState.GameHash, gameTime, 10);
				text += "\n\nJesteś na " + (scores.Scores.FindIndex(0, us => us.Score >= gameTime) + 1)
					+ " miejscu.\n\n";
			}
			int i = 0;
			if (scores.Scores.Count > 0) {
				text += "Wszystkie wyniki:\n" + scores.Scores
					.Select(t => (++i) + ". " + t.Score / 24 + " dni " + t.Score % 24 + " godzin ")
					.Aggregate((t, y) => t + "\n" + y);
			}
			OpenWindow().OpenText(text);
		}
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
