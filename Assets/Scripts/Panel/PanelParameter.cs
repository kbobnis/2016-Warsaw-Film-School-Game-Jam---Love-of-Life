using UnityEngine;
using UnityEngine.UI;

public class PanelParameter : MonoBehaviour {

	private Parameter Parameter;

	void Update () {
		GetComponentInChildren<Text>().text = Parameter.Text;
	}

	internal void Init(Parameter parameter) {
		Parameter = parameter;
		
	}
}
