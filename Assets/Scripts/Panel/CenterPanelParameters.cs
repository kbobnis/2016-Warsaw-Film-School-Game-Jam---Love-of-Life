using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CenterPanelParameters : MonoBehaviour {

	private List<Parameter> Parameters;
	private float LastUpdate;

	internal void Init(List<Parameter> parameters) {
		Parameters = parameters;
		foreach (Transform child in transform) {
			child.gameObject.SetActive(false);
		}
		int i = 0;
		foreach (Parameter p in Parameters) {
			transform.GetChild(i).gameObject.SetActive(true);
			i++;
		}
		UpdateParameters();
	}

	void Update() {
		if (LastUpdate + 0.1f < Time.time) {
			LastUpdate = Time.time;
			UpdateParameters();
		}
	}

	private void UpdateParameters() {
		
		int i = 0;
		foreach (Parameter p in Parameters) {
			GameObject parameterGO = transform.GetChild(i).gameObject;

			Transform valuePanel = parameterGO.transform.GetChild(1);
			valuePanel.GetChild(0).gameObject.SetActive(p.HasMaxValue());
			if (p.HasMaxValue()) {
				valuePanel.GetChild(0).GetComponent<Image>().fillAmount = p.ActualValue / (p.MaxValue.Calculate(true) * p.ActualMaxValueMultiplier); //the fill amount will stay the same here
			}
			//change color if this is holding param back
			//change color if this is dragging others down
			parameterGO.GetComponent<Image>().color = p.IsUsedAndIsZero || p.IsDraggingDown? Color.red : Color.white;

			//show arrow down if is lowered by a parameter
			parameterGO.transform.GetChild(0).GetChild(1).GetComponent<Image>().enabled = p.IsDraggedDownBy.Count > 0;

			valuePanel.GetChild(1).GetComponent<Text>().text = (p.ActualValue ).ToString("0.00") + " / " + (p.MaxValue.Calculate(true) * p.ActualMaxValueMultiplier).ToString("0.00");  //the fill amount will stay the same here
			parameterGO.GetComponentInChildren<Text>().text = p.Text;
			i++;
		}
	}
}
