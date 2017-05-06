using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClearInputField : MonoBehaviour, IPointerClickHandler {

	public void OnPointerClick(PointerEventData eventData) {
		this.gameObject.GetComponent<InputField> ().text = "";
	}
}
