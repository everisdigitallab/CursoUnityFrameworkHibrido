using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveAnimation : MonoBehaviour {
	private RectTransform trans;
	public Image img;

	public float currentState;	//estado atual da wave

	public Vector2 finalScale;	//scale final da wave
	public Color initialColor;	//cor inicial da wave
	public Color finalColor;	//cor final da wave
	public float initialRot;	//rotação inicial da wave
	public float finalRot;	//rotação final da wave

	public AnimationCurve animCurve;	//curva de animação
	[HideInInspector]
	public float animTime;	//tempo da animação

	// Use this for initialization
	void Start () {
		trans = GetComponent<RectTransform> ();
		img = GetComponent<Image> (); 
	}
	
	// Update is called once per frame
	void Update () {
		SetPosition ();

		animTime += Time.deltaTime;
		currentState = animCurve.Evaluate (animTime);
		if (animTime > animCurve.keys [animCurve.length - 1].time) {
			Destroy (gameObject);
		}
	}

	void SetPosition(){
		float sx = currentState * finalScale.x;
		float sy = currentState * finalScale.y;
		trans.localScale = new Vector2 (sx, sy);

		img.color = Color.Lerp (initialColor, finalColor, currentState);

		float r = currentState * (finalRot - initialRot) + initialRot;
		trans.localRotation = Quaternion.Euler (0, 0, r);
	}
}
