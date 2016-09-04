using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class CenterPanelParameters : MonoBehaviour {

	private List<Parameter> Parameters;

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
		UpdateParameters();
	}

	private void UpdateParameters() {
		
		int i = 0;
		foreach(Parameter p in Parameters) {
			GameObject parameterGO = transform.GetChild(i).gameObject;
			parameterGO.GetComponentInChildren<Text>().text = p.Text + " " + p.ActualValue + (p.IsRising()?"UP":"");
			i++;
		}
	}
}
