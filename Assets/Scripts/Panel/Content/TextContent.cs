
using UnityEngine;
using UnityEngine.UI;

public class TextContent : MonoBehaviour {
	internal void Open(string v) {
		gameObject.FindByName<Text>("Title").text = v;
	}
}
