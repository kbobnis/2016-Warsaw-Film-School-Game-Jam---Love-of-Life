using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectableSituation : MonoBehaviour {

	private Situation Situation;
	private PanelSchedule PanelSchedule;

	internal void Init(Situation situation, PanelSchedule panelSchedule) {
		Situation = situation;
		PanelSchedule = panelSchedule;
		GetComponentInChildren<Text>().text = situation.Text;
	}
}
