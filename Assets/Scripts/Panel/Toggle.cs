using UnityEngine;

public class Toggle : MonoBehaviour {

	public void ToggleMe() {
		gameObject.SetActive(!gameObject.activeSelf);
	}
}
