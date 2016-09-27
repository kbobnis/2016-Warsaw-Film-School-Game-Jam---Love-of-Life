using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelSituation : MonoBehaviour {

	internal void UpdateActiveSituation(Situation actualSituation) {
		GetComponentsInChildren<Text>()[0].text = "W tej chwili: \n\n" + actualSituation.Text;
		Game.Me.ActualSituation = new ScheduledSituation(Game.Me.GameTimeNormalized, 1, actualSituation, false);
	}

	internal void HourHasChanged(int newHour, Situation ss) {
		UpdateActiveSituation(ss);

	}
}
