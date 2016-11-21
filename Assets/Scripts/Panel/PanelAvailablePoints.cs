using UnityEngine;
using UnityEngine.UI;

public class PanelAvailablePoints : MonoBehaviour, ActualPointsChangeListener {

	public void PointsChanged(int actualPoints) {
		gameObject.FindByName<Text>("QuestText").text = "Masz " + actualPoints + " punktów. Kliknij, aby użyć.";
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
