using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSchedule : MonoBehaviour {

	class ButtonAction {

		public static ButtonAction Delete = new ButtonAction((ButtonSchedule b) => {
				Game.Me.GameState.Schedule.AddSituation(b.MyHour, 1, null, false);
			},
			Color.green,
			"Usuń"
		);
		public static ButtonAction Add = new ButtonAction((ButtonSchedule b) => {
				Game.Me.OpenWindow().OpenChooseSituation(Game.Me.GameState.Situations, b.MyHour);
			},
			Color.green,
			"Dodaj"
		);
		public static ButtonAction IsPermament = new ButtonAction((ButtonSchedule b) => {
				Game.Me.OpenWindow().OpenText("Ta sytuacja jest permamentna, nie można jej usunąć.");
			},
			Color.gray,
			"Usuń"
		);
	
		public Action<ButtonSchedule> OnClickAction;
		internal Color ButtonColor;
		internal string ButtonText;

		public ButtonAction(Action<ButtonSchedule> p, Color buttonColor, string buttonText) {
			OnClickAction = p;
			ButtonColor = buttonColor;
			ButtonText = buttonText;
		}
	}

	private Situation Ss;
	private PanelSchedule PanelSchedule;
	private bool IsPermament;
	private int MyHour;
	private ButtonAction ActionType;

	private void RefreshMe() {
		ActionType = Ss != null ? (IsPermament ? ButtonAction.IsPermament : ButtonAction.Delete) : ButtonAction.Add;
		GetComponent<Image>().color = IsPermament ? Color.red : Color.white;
		gameObject.FindByName<Text>("TextHour").text = MyHour.ToString();
		gameObject.FindByName<Text>("TextName").text = Ss != null ? Ss.Text : "";
		//button action
		gameObject.FindByName<Text>("TextButtonAction").text = ActionType.ButtonText;
		gameObject.FindByName<Image>("ButtonAction").color = ActionType.ButtonColor;
	}

	internal void Init(Situation s, bool isPermament, PanelSchedule panelSchedule, int myHour) {
		MyHour = myHour;
		Ss = s;
		IsPermament = isPermament;
		PanelSchedule = panelSchedule;
		RefreshMe();
	}

	public void ButtonActionClicked() {
		ActionType.OnClickAction(this);
	}
}
