using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoyStickScript : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

//	public RectTransform VJSRect;
	private Image BGImg;
	private Image StickImg;
	private Vector3 TempV;
	private Vector3 InputV; // between (-1,0,-1) to (1,0,1) from bottom left to top right, its magnitude shall be 1.
	public Vector3 JoyStickInputVectors{
		get{
			return InputV;
		}
	}

	private void Start(){
//		VJSRect = GetComponent<RectTransform> ();
		BGImg = GetComponent<Image> ();
		StickImg = transform.GetChild (0).GetComponent<Image> ();
		InputV = Vector3.zero;
	}

	public virtual void OnDrag(PointerEventData ped){
		Vector2 posi;
		// We get the local position of the joystick
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(BGImg.rectTransform,ped.position, ped.pressEventCamera, out posi)){

			posi.x = posi.x / BGImg.rectTransform.sizeDelta.x;
			posi.y = posi.y / BGImg.rectTransform.sizeDelta.y;

			TempV = new Vector3 (2 * (posi.x - 0.5f), 0, 2 * (posi.y - 0.5f));

			InputV = (TempV.magnitude > 1.0f) ? TempV.normalized : TempV;
			//move joystick img 据说有可能不能直接用pixel的位置？？
			StickImg.rectTransform.anchoredPosition = new Vector3(InputV.x * (StickImg.rectTransform.sizeDelta.x/2), InputV.z * (StickImg.rectTransform.sizeDelta.y/2));

		}
	}

	public virtual void OnPointerDown(PointerEventData ped){
//		Debug.Log ("oneDown");
		OnDrag (ped);
	}

	public virtual void OnPointerUp(PointerEventData ped){
		InputV = Vector3.zero;
		StickImg.rectTransform.anchoredPosition = Vector3.zero;
	}

	// Two choices: first, if input is a touch on the virtual joystick, it will return the value (-1,1) horizontal and vetical.
	// else if input is by input "Horizontal" or "Vertical" (e.g. mac keyboard), it will return that value.
	public float Horizontal(){
		if (InputV.x != 0)
			return InputV.x;
		else
			return Input.GetAxis ("Horizontal");
	}

	public float Vertical(){
		if (InputV.z != 0)
			return InputV.z;
		else
			return Input.GetAxis ("Vertical");
	}
		
}
