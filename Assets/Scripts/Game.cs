using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game Me;

	public PanelSchedule PanelSchedule;
	public PanelCenter PanelCenter;
	public PanelSelectModule PanelSelectModule;
	public PanelEndGame PanelEndGame;

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
		Schedule schedule = XmlLoader.LoadSchedule(model.GetElementsByTagName("schedule")[0], situations);
		XElement xElement = XElement.Parse(Resources.Load<TextAsset>(modelDir).text);
		List<Plot.Element> plotElements = XmlLoader.LoadPlot(xElement.Elements().First(t => t.Name == "plot"), parameters, situations);

		GameState = new GameState(parameters, situations, schedule, new Model(XmlLoader.LoadTime(model, parameters)), new Plot(plotElements));
		GameState.Schedule.AddScheduleUpdateSituation(PanelSchedule);
		GameState.Schedule.AddScheduleUpdateSituation(PanelCenter);

		PanelSchedule.Init(GameState);
		PanelCenter.Init(GameState);
		ChangeToPanelCenter();
	}

	internal void EndGame(EndCondition endCondition) {
		GameState.GameHasEnded = true;
		ChangePanel(typeof(PanelEndGame));
		PanelSelectModule.gameObject.SetActive(true);
		PanelEndGame.Init(endCondition);
	}

	private void ChangePanel(Type type) {
		PanelCenter.gameObject.SetActive(type == typeof(PanelCenter));
		PanelSchedule.gameObject.SetActive(type == typeof(PanelSchedule));
		PanelSelectModule.gameObject.SetActive(type == typeof(PanelSelectModule));
		PanelEndGame.gameObject.SetActive(type == typeof(PanelEndGame));
	}

	internal abstract class EndCondition {
		internal abstract string GetText();

		internal class Lose : EndCondition {
			private Parameter Parameter;
			private GameState GameState;

			public Lose(Parameter parameter, GameState gameState) {
				Parameter = parameter;
				GameState = gameState;
			}

			internal override string GetText() {
				return "Przegrałeś w dniu " + GameState.DayNumber + ", bo wartośc parametru " + Parameter.Text + " spadła poniżej zera.";
			}
		}

		internal class Win : EndCondition {
			private Plot.Element PlotElement;

			public Win(Plot.Element plotElement) {
				PlotElement = plotElement;
			}

			internal override string GetText() {
				return "Wygrałeś, bo skończyłeś zadanie: " + PlotElement.Text;
			}
		}
	}
}
