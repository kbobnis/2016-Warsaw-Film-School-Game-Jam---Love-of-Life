using UnityEngine;
using UnityEngine.UI;

public class PanelClock : MonoBehaviour {

	void Update () {
		GetComponent<Image>().fillAmount = (1.0f - (Game.Me.GameState.GameTime % 24f) / 24f);
	}
}
