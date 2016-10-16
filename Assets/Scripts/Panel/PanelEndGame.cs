using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelEndGame : MonoBehaviour {
	
	internal void Init(EndCondition endCondition) {
		GetComponentsInChildren<Text>()[0].text = endCondition.GetText();
	}

	internal void ShowHighScore(string gameId, int gameTime) {
		List<UserScore> highScores = HighScore.GetHighScore(gameId);
		string text = "Najlepsze wyniki: \n\n";

		for(int i=0; i < highScores.Count; i++) {
			text += (i+1) + ". "+ highScores[i].ToString() + "\n";
		}
		gameObject.FindByName<Text>("Text").text = text;
		Toggle(true);
	}

	public void Toggle(bool? enable=null) {
		gameObject.SetActive(enable!=null?enable.Value:!gameObject.activeSelf);
	}

	public void Toggle() {
		Toggle(null);
	}
}
