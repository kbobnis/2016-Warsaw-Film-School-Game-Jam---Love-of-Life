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
			private Parameter Parameter;
			private GameState GameState;

			public Win(Parameter parameter, GameState gameState) {
				Parameter = parameter;
				GameState = gameState;
			}

			internal override string GetText() {
				return "Wygrałeś w dniu " + GameState.DayNumber + ", bo wartość parametru " + Parameter.Text + " osiągnęła maksimum.";
			}
		}
	}
}
