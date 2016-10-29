using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSchedule : MonoBehaviour {

	class ButtonAction {

		public static ButtonAction Change = new ButtonAction((ButtonSchedule b) => {
				Game.Me.OpenWindow().OpenChooseSituation(Game.Me.GameState.Situations, b.MyHour, b.Ss);
			},
			Color.green,
			"Zmień"
		);
		public static ButtonAction IsPermament = new ButtonAction((ButtonSchedule b) => {
				Game.Me.OpenWindow().OpenText("Ta sytuacja jest permamentna, nie można jej usunąć.");
			},
			Color.gray,
			"Zmień"
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
	private bool IsPermament;
	private int MyHour;
	private ButtonAction ActionType;

	private void RefreshMe() {
		ActionType = IsPermament? ButtonAction.IsPermament : ButtonAction.Change;
		GetComponent<Image>().color = IsPermament ? Color.red : Color.white;
	}

	internal void Init(Situation s, bool isPermament, int myHour) {
		if (s == null) {
			throw new Exception("Situation can not be null");
		}
		MyHour = myHour;
		Ss = s;
		IsPermament = isPermament;
		RefreshMe();
	}

	public void ButtonActionClicked() {
		ActionType.OnClickAction(this);
	}
}
