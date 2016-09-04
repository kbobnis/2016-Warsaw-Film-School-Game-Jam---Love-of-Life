using UnityEngine;
using UnityEngine.EventSystems;

public class FastenButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public void OnPointerDown(PointerEventData eventData) {
		Game.Me.ActualGameSpeed = Game.Me.Model.TimeChanges.FasterSpeed;
	}

	public void OnPointerUp(PointerEventData eventData) {
		Game.Me.ActualGameSpeed = Game.Me.Model.TimeChanges.NormalSpeed;
	}
}
