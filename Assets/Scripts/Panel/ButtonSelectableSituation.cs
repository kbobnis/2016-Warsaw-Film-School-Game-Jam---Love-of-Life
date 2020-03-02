using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectableSituation : MonoBehaviour {

	private Situation Situation;

	internal void Init(Situation situation) {
		Situation = situation;
		GetComponentInChildren<Text>().text = situation.Text;
	}
}
