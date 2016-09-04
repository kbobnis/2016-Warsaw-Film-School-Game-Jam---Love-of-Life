using UnityEngine;
using UnityEngine.UI;

public class ButtonSchedule : MonoBehaviour {

	private Situation Ss;
	private Situation ProposedSituation;
	private PanelSchedule PanelSchedule;
	private bool IsPermament;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void RefreshMe() {
		GetComponentInChildren<Text>().text = "";
		GetComponent<Button>().enabled = Ss != null || ProposedSituation != null;
		if (Ss != null) {
			GetComponentInChildren<Text>().text = Ss.Text;
		} else if (ProposedSituation != null) {
			GetComponentInChildren<Text>().text = "Wstaw tutaj " + ProposedSituation.Text;
		}
	}

	internal void Init(Situation s, bool isPermament, PanelSchedule panelSchedule) {
		ProposedSituation = null;
		Ss = s;
		IsPermament = isPermament;
		PanelSchedule = panelSchedule;
		RefreshMe();
		GetComponent<Image>().color = IsPermament ? Color.red : Color.white;
	}

	internal void PropositionWasSelected(Situation proposed) {
		ProposedSituation = proposed;
		RefreshMe();
	}

	public void Clicked() {
		if (IsPermament) {
			return;
		}
		if (Ss != null) {
			Ss = null;
		}
		if (ProposedSituation != null && Ss == null) {
			Ss = ProposedSituation;
		}
		PanelSchedule.UnselectProposed();
	}

	internal void UnselectProposed() {
		ProposedSituation = null;
		RefreshMe();
	}
}
