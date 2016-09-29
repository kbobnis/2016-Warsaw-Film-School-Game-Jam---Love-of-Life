using UnityEngine;
using UnityEngine.EventSystems;

public class FastenButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public void OnPointerDown(PointerEventData eventData) {
		Game.Me.GameState.ActualGameSpeed = Game.Me.GameState.Model.TimeChanges.FasterSpeed;
	}

	public void OnPointerUp(PointerEventData eventData) {
		Game.Me.GameState.ActualGameSpeed = Game.Me.GameState.Model.TimeChanges.NormalSpeed;
	}
}
