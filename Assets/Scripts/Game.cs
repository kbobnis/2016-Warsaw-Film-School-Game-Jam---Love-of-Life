using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class Game : MonoBehaviour {

	public PanelSchedule PanelSchedule;
	public PanelCenter PanelCenter;

	// Use this for initialization
	void Awake () {
		PanelCenter.gameObject.SetActive(false);
		PanelSchedule.gameObject.SetActive(true);

		XmlDocument model = new XmlDocument();
		string modelDir = "love_of_life";

		model.LoadXml(Resources.Load<TextAsset>(modelDir).text);
		//getting rid of comments
		string pattern = "(<!--.*?--\\>)";
		model.InnerXml = Regex.Replace(model.InnerXml, pattern, string.Empty, RegexOptions.Singleline);

		List<Parameter> parameters = XmlLoader.LoadParameters(model);
		List<Situation> situations = XmlLoader.LoadSituations(model, parameters);
		Schedule scheduledSituations = XmlLoader.LoadSchedule(model, situations);

		PanelSchedule.Init(scheduledSituations, situations, parameters);
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeToPanelSchedule() {
		PanelCenter.gameObject.SetActive(false);
		PanelSchedule.gameObject.SetActive(true);
	}

	public void ChangeToPanelCenter() {
		PanelCenter.gameObject.SetActive(true);
		PanelSchedule.gameObject.SetActive(false);
	}
}
