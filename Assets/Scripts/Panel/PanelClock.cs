using UnityEngine;
using UnityEngine.UI;

public class PanelClock : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		GetComponent<Image>().fillAmount = 1 - (Game.Me.GameState.GameTime % 24) / 24f;
	}
}
