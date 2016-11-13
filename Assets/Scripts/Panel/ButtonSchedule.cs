using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSchedule : MonoBehaviour {

	class ButtonAction {

		public static ButtonAction Change = new ButtonAction((ButtonSchedule b) => {
				Game.Me.OpenWindow().OpenChooseSituation(Game.Me.GameState.Situations, b.MyHour, b.Ss);
			}
		);
		public static ButtonAction IsPermament = new ButtonAction((ButtonSchedule b) => {
				Game.Me.OpenWindow().OpenText("Ta sytuacja jest permamentna, nie można jej usunąć.");
			}
		);
	
		public Action<ButtonSchedule> OnClickAction;

		public ButtonAction(Action<ButtonSchedule> p) {
			OnClickAction = p;
		}
	}

	public Situation Ss;
	public bool IsPermament;
	public int MyHour;
	private ButtonAction ActionType;

	internal void Init(Situation s, bool isPermament, int myHour) {
		MyHour = myHour;
		Ss = s;
		IsPermament = isPermament;
		ActionType = isPermament ? ButtonAction.IsPermament : ButtonAction.Change;
		GetComponentInChildren<Text>().text = s!=null?s.Text:"";
	}

	public void ButtonActionClicked() {
		ActionType.OnClickAction(this);
	}
}
