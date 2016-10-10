using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelSituation : MonoBehaviour {

	private Situation Situation;
	private GameState GameState;

	internal void UpdateActiveSituation(Situation actualSituation, GameState gameState) {
		GetComponentsInChildren<Text>()[0].text = "Dzień " + gameState.DayNumber + ", godzina " + gameState.HourOfDay;
		GetComponentsInChildren<Text>()[1].text = "W tej chwili: \n\n" + actualSituation.Text;
		gameObject.FindByName<Text>("QuestText").text = gameState.ActualPlotElement.GetInfo();

		//show buttons on second panel
		Transform panelButtons = gameObject.FindByName<Transform>("PanelButtons");
		int i = 0;
		foreach (Transform button in panelButtons) {
			button.gameObject.SetActive(i < actualSituation.Buttons.Count);
			if (i < actualSituation.Buttons.Count) {
				LOL.Button b = actualSituation.Buttons[i];
				button.GetComponentInChildren<Text>().text = b.Text + "\n\nKoszt: " + b.Changes.Where(change => change.ValueCalculation != null && change.ValueCalculation.Calculate() < 0).Select(change => change.What.Text + " " + change.ValueCalculation.Calculate().ToString("0.0")).Aggregate((t, y) => t + ", " + y);
				button.GetComponent<Button>().onClick.RemoveAllListeners();
				button.GetComponent<Button>().onClick.AddListener(() => {
					foreach (Change c in b.Changes) {
						c.UpdateParams();
					}

					foreach(Parameter p in gameState.Parameters) {
						p.UpdateValuesFromPreviousLoop();
					}
				});
				bool canIt = b.Changes.All(change => change.CanUpdateWithoutOverflow(change));
				button.GetComponent<Image>().color = canIt ? Color.white : Color.red;
				button.GetComponent<Button>().enabled = canIt;
			}

			i++;
		}
	}

	void Update() {
		if (Situation != null) {
			UpdateActiveSituation(Situation, GameState);
		}
	}

	internal void HourHasChanged(int newHour, Situation ss, GameState gameState) {
		Situation = ss;
		GameState = gameState;
		UpdateActiveSituation(ss, gameState);
	}
}
