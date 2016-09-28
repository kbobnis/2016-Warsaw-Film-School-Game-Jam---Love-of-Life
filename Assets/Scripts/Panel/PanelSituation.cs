using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelSituation : MonoBehaviour {

	internal void UpdateActiveSituation(Situation actualSituation) {
		GetComponentsInChildren<Text>()[0].text = "W tej chwili: \n\n" + actualSituation.Text;
		Game.Me.ActualSituation = new ScheduledSituation(Game.Me.GameTimeNormalized, 1, actualSituation, false);

		//show buttons on second panel
		Transform panelButtons = transform.GetChild(1);
		int i = 0;
		foreach(Transform button in panelButtons) {
			button.gameObject.SetActive(i < actualSituation.Buttons.Count);
			if (i < actualSituation.Buttons.Count) {
				LOL.Button b = actualSituation.Buttons[i];
				button.GetComponentInChildren<Text>().text = b.Text;
				button.GetComponent<Button>().onClick.RemoveAllListeners();
				button.GetComponent<Button>().onClick.AddListener(() => {
					foreach(Change c in b.Changes) {
						c.What.UpdateWithChange(c);
					}
				});

				button.GetComponent<Button>().enabled = b.Changes.All( change => change.What.CanUpdateWithoutOverflow(change) );
			}

			i++;
		}

	}

	internal void HourHasChanged(int newHour, Situation ss) {
		UpdateActiveSituation(ss);
	}
}
