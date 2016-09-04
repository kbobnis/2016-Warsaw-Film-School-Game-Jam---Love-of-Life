using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PanelParameter : MonoBehaviour {

	private Parameter Parameter;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponentInChildren<Text>().text = Parameter.Text;
	}

	internal void Init(Parameter parameter) {
		Parameter = parameter;
		
	}
}
