using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelSituation : MonoBehaviour {

	private Situation Situation;
	private List<Parameter> Parameters;

	internal void UpdateActiveSituation(Situation actualSituation, List<Parameter> parameters) {
		GetComponentsInChildren<Text>()[0].text = "W tej chwili: \n\n" + actualSituation.Text;
		Game.Me.ActualSituation = new ScheduledSituation(Game.Me.GameTimeNormalized, 1, actualSituation, false);

		//show buttons on second panel
		Transform panelButtons = transform.GetChild(1);
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

					foreach(Parameter p in parameters) {
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
			UpdateActiveSituation(Situation, Parameters);
		}
	}

	internal void HourHasChanged(int newHour, Situation ss, List<Parameter> parameters) {
		Situation = ss;
		Parameters = parameters;
		UpdateActiveSituation(ss, parameters);
	}
}
