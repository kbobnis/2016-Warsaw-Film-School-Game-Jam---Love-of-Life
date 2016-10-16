using System.Collections.Generic;
using UnityEngine;

public class PanelWindow : MonoBehaviour{


	internal void OpenChooseSituation(List<Situation> situations, int hour) {
		gameObject.FindByName<RectTransform>("ChooseSituationContent").gameObject.SetActive(true);
		gameObject.FindByName<ChooseSituationContent>("ChooseSituationContent").Open(situations, hour, gameObject);
	}

	public void Cancel() {
		Game.Me.CloseWindow(gameObject);
	}

	internal void OpenText(string v) {
		gameObject.FindByName<RectTransform>("TextContent").gameObject.SetActive(true);
		gameObject.FindByName<TextContent>("TextContent").Open(v);
	}
}

