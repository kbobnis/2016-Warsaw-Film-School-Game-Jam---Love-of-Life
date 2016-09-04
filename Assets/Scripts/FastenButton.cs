using UnityEngine;
using UnityEngine.EventSystems;

public class FastenButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerDown(PointerEventData eventData) {
		Debug.Log(this.gameObject.name + " Was Clicked.");
		Game.Me.ActualGameSpeed = Game.Me.Model.TimeChanges.FasterSpeed;
	}

	public void OnPointerUp(PointerEventData eventData) {
		Debug.Log(this.gameObject.name + " Was unclicked Clicked.");
		Game.Me.ActualGameSpeed = Game.Me.Model.TimeChanges.NormalSpeed;
	}
}
