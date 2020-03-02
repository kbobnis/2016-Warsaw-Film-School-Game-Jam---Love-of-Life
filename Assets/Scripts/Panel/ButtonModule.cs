using UnityEngine;

public class ButtonModule : MonoBehaviour {

	public string ModuleId;

	public void Clicked() {
		Game.Me.LoadModule(ModuleId);
	}
}
