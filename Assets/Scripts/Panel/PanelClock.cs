using UnityEngine;
using UnityEngine.UI;

public class PanelClock : MonoBehaviour {

	void Update () {
		Debug.Log("hour of day: " + Game.Me.GameState.HourOfDay + ", value: " + (1.0f - (Game.Me.GameState.GameTime % 24f ) / 24f));
		GetComponent<Image>().fillAmount = (1.0f - (Game.Me.GameState.GameTime % 24f) / 24f);
	}
}
