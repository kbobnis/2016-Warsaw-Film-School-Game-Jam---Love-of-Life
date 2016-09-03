using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ButtonSelectableSituation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	internal void Init(Situation situation) {
		GetComponentInChildren<Text>().text = situation.Text;
	}
}
