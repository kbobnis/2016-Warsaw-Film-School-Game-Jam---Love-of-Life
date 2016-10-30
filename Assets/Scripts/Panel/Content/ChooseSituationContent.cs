using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChooseSituationContent : MonoBehaviour {

	public GameObject SituationPrefab;

	internal void Open(List<Situation> situations, int hour, GameObject panelWindow, Situation actualSituation) {
		SituationPrefab.SetActive(false);

		Situation.Type sNow = Game.Me.GameState.Schedule.GetActualDayNightType(hour);
		gameObject.FindByName<Text>("Title").text = "Wybierz sytuację na godzinę: " + hour + " (w " + (sNow == Situation.Type.Day?"dzień":"nocy") + ")";

		Transform scroll = gameObject.FindByName<Transform>("ScrollSituations");
		foreach (Transform child in scroll) {
			if (child.gameObject != SituationPrefab) {
				Destroy(child.gameObject);
			}
		}

		foreach(Situation s in situations) { 
			if (s.Selectable) {
				GameObject go = Instantiate(SituationPrefab);
				go.SetActive(true);
				go.transform.SetParent(scroll);
				go.transform.localScale = new Vector3(1, 1, 1);
				go.FindByName<Image>("ButtonTextSituation").color = s.DayNightType.Color;

				go.FindByName<Text>("TextSituation").text = s.Text;
				go.FindByName<Text>("TextSituation").color = s.DayNightType == Situation.Type.Day?Color.black:Color.white;
				if (s == actualSituation) {
					go.FindByName<Image>("ButtonTextSituation").color = Color.gray;
					go.FindByName<Text>("TextSituation").text += " (obecna sytuacja) ";
				}
				go.FindByName<Button>("ButtonTextSituation").onClick.AddListener(AddSituation(hour, s));
				go.FindByName<Button>("ButtonTextSituation").onClick.AddListener(() => {
					Game.Me.CloseWindow(panelWindow);
				});
				go.FindByName<Text>("TextSituationInfo").text = "Szczegóły sytuacji " + s.Text;
				go.FindByName<Button>("ButtonSituationInfo").onClick.AddListener(OpenInfo(s));
			}
		}
	}

	private UnityAction AddSituation(int hour, Situation s) {
		return () => { Game.Me.GameState.Schedule.AddSituation(hour, 1, s, false, true); };
	}

	private UnityAction OpenInfo(Situation s) {
		return () => { Game.Me.OpenWindow().OpenText(" Sytuacja " + s.Text + ". Info: " + s.ToString()); };
	}
}