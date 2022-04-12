using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class UIAnim : MonoBehaviour {

	[System.Serializable]
	public class Animation
	{
		public enum State
		{
			Open, Close, Opening, Closing, Controlled, invOpening, invClosing
		}
		public State state;

		public bool open;	//determina se está aberto ou fechado
		bool tempOpen;	//checa a mudança da variável open

		public enum AnimType
		{
			Translation = (1 << 0),
			Scale = (1 << 1),
			Rotation = (1 << 2),
			Opacity = (1 << 3),
			Var = (1 << 4)
		}
		public AnimType openAnimType;	//tipo de animação de abertura
		public AnimType closeAnimType;	//tipo de animação de fechamento
		public AnimationCurve openCurve = AnimationCurve.EaseInOut(0,0,0.2f,1);	//curva de animação de abertura
		public AnimationCurve closeCurve = AnimationCurve.EaseInOut(0,1,0.2f,0);	//curva de animação de fechamento
		public float currentAnimTime;	//posição da animação na linha do tempo (0:início, 1:fim)

		#region Translation Anim vars
		public bool useTransforms;
		public Transform closedTransform;
		public Transform openTransform;
		public Vector2 closedPos;
		public Vector2 openPos;
		public bool openPosIsDefault;
		#endregion

		#region Scale Anim vars
		public Vector2 closedScale;
		public Vector2 openScale;
		public bool openScaleIsDefault;
		#endregion

		#region Rotation Anim vars
		public float closedRot;
		public float openRot;
		public bool openRotIsDefault;
		#endregion

		#region Opacity Anim vars
		public float closedOpcty;
		public float openOpcty;
		public bool openOpctyIsDefault;
		#endregion

		#region Var Anim vars
		public int closedInt;
		public int openInt;
		public float closedFloat;
		public float openFloat;
		public bool closedBool;
		public bool openBool;
		public bool openVarIsDefault;
		#endregion

		#region Controlled
		public bool controlled;	//determina se a animação está sendo controlada por um agente externo
		public float controlledPos;	//posição da animação controlada na timeline
		float tempControlledPos;	//posição da animação controlada na timeline no frame anterior
		public float controlVelToComplete;	//velocidade em que a animação deve estar para ser completa ao deixar de ser controlada
		public float controlPosToComplete;	//posição em que a animação deve estar para ser completa ao deixar de ser controlada (caso a velocidade não for suficiente)
		#endregion

		public UnityEvent onOpenStart;
		public UnityEvent onOpenEnd;
		public UnityEvent onCloseStart;
		public UnityEvent onCloseEnd;

		public Animation(){
			openAnimType = (AnimType)0;
			closeAnimType = (AnimType)0;
			openCurve = AnimationCurve.EaseInOut(0,0,0.2f,1);
			closeCurve = AnimationCurve.EaseInOut(0,1,0.2f,0);
			currentAnimTime = 1;

			openScale = Vector2.one;
			openScaleIsDefault = true;
			openOpcty = 1;
			openOpctyIsDefault = true;
		}

		public void Animate(){
			if (controlled) {
				state = State.Controlled;
			}
			if (state == State.Open) {
				if (!open) {
					state = State.Closing;
					onCloseStart.Invoke ();
					currentAnimTime = 0;
				}
			} else if (state == State.Close) {
				if (open) {
					state = State.Opening;
					onOpenStart.Invoke ();
					currentAnimTime = 0;
				}
			} else if (state == State.Opening || state == State.Closing) {
				currentAnimTime = Mathf.Min (1, currentAnimTime + ((state == State.Opening) ? (Time.deltaTime / openCurve.keys [openCurve.length - 1].time) : (Time.deltaTime / closeCurve.keys [closeCurve.length - 1].time)));
				if (currentAnimTime == 1) {
					if ((state == State.Opening)) {
						state = State.Open;
						onOpenEnd.Invoke ();
					} else {
						state = State.Close;
						onCloseEnd.Invoke ();
					}
				}
			} else if (state == State.invOpening || state == State.invClosing) {
				currentAnimTime = Mathf.Max (0, currentAnimTime - ((state == State.invOpening) ? (Time.deltaTime / openCurve.keys [openCurve.length - 1].time) : (Time.deltaTime / closeCurve.keys [closeCurve.length - 1].time)));
				if (currentAnimTime == 0) {
					if ((state == State.invOpening)) {
						state = State.Close;
						currentAnimTime = 1;
					} else {
						state = State.Open;
						currentAnimTime = 1;
					}
				}
			} else {
				currentAnimTime = controlledPos;
				if (!controlled) {
					if (controlledPos >= controlPosToComplete || controlledPos - tempControlledPos >= controlVelToComplete) {
						open = !open;
						if (open) {
							state = State.Opening;
							onOpenStart.Invoke ();
						} else {
							state = State.Closing;
							onCloseStart.Invoke ();
						}
					} else {
						if (open) {
							state = State.invClosing;
						} else {
							state = State.invOpening;
						}
					}

					controlledPos = 0;
					tempControlledPos = 0;
				}
				tempControlledPos = Mathf.Lerp(tempControlledPos,controlledPos,0.3f);
			}
		}

		#region GetCurrentValues
		public Vector3 GetCurrentTranslation(){
			Vector3 curT = new Vector3 ();
			
			if (state == State.Open || state == State.Opening || state == State.invOpening) {
				if (CheckAnimType (AnimType.Translation, true)) {
					if (!useTransforms)
						curT = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time) * (openPos - closedPos) + closedPos;
					else
						curT = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time) * (openTransform.localPosition - closedTransform.localPosition) + closedTransform.localPosition;
				} else {
					if (!useTransforms)
						curT = openPosIsDefault ? openPos : closedPos;
					else
						curT = openPosIsDefault ? openTransform.localPosition : closedTransform.localPosition;
				}
			} else if (state == State.Close || state == State.Closing || state == State.invClosing) {
				if (CheckAnimType (AnimType.Translation, false)) {
					if (!useTransforms)
						curT = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time) * (openPos - closedPos) + closedPos;
					else
						curT = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time) * (openTransform.localPosition - closedTransform.localPosition ) + closedTransform.localPosition;
                } else {
					if (!useTransforms)
						curT = openPosIsDefault ? openPos : closedPos;
					else
						curT = openPosIsDefault ? openTransform.localPosition : closedTransform.localPosition;
				}
			} else {
				if (open) {
					if (CheckAnimType (AnimType.Translation, false)) {
						if (!useTransforms)
							curT = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time) * (openPos - closedPos) + closedPos;
						else
							curT = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time) * (openTransform.localPosition - closedTransform.localPosition) + closedTransform.localPosition;
                    } else {
						if (!useTransforms)
							curT = openPosIsDefault ? openPos : closedPos;
						else
							curT = openPosIsDefault ? openTransform.localPosition : closedTransform.localPosition;
					}
				} else {
					if (CheckAnimType (AnimType.Translation, true)) {
						if (!useTransforms)
							curT = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time) * (openPos - closedPos) + closedPos;
						else
							curT = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time) * (openTransform.localPosition - closedTransform.localPosition) + closedTransform.localPosition;
					} else {
						if (!useTransforms)
							curT = openPosIsDefault ? openPos : closedPos;
						else
							curT = openPosIsDefault ? openTransform.localPosition : closedTransform.localPosition;
					}
				}
			}

			return curT;
		}

		public Vector2 GetCurrentScale(){
			Vector2 curS = new Vector2 ();

			if (state == State.Open || state == State.Opening || state == State.invOpening) {
				if (CheckAnimType (AnimType.Scale, true))
					curS = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time) * (openScale - closedScale) + closedScale;
				else
					curS = openScaleIsDefault ? openScale : closedScale;
			} else if (state == State.Close || state == State.Closing || state == State.invClosing) {
				if (CheckAnimType (AnimType.Scale, false))
					curS = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time) * (openScale - closedScale) + closedScale;
				else
					curS = openScaleIsDefault ? openScale : closedScale;
			} else {
				if (open) {
					if (CheckAnimType (AnimType.Scale, false))
						curS = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time) * (openScale - closedScale) + closedScale;
					else
						curS = openScaleIsDefault ? openScale : closedScale;
				} else {
					if (CheckAnimType (AnimType.Scale, true))
						curS = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time) * (openScale - closedScale) + closedScale;
					else
						curS = openScaleIsDefault ? openScale : closedScale;
				}
			}

			return curS;
		}

		public float GetCurrentRotation(){
			float curR = 0;

			if (state == State.Open || state == State.Opening || state == State.invOpening) {
				if (CheckAnimType (AnimType.Rotation, true))
					curR = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time) * (openRot - closedRot) + closedRot;
				else
					curR = openScaleIsDefault ? openRot : closedRot;
			} else if (state == State.Close || state == State.Closing || state == State.invClosing){
				if (CheckAnimType (AnimType.Rotation, false))
					curR = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time) * (openRot - closedRot) + closedRot;
				else
					curR = openRotIsDefault ? openRot : closedRot;
			} else {
				if (open) {
					if (CheckAnimType (AnimType.Rotation, false))
						curR = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time) * (openRot - closedRot) + closedRot;
					else
						curR = openRotIsDefault ? openRot : closedRot;
				} else {
					if (CheckAnimType (AnimType.Rotation, true))
						curR = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time) * (openRot - closedRot) + closedRot;
					else
						curR = openScaleIsDefault ? openRot : closedRot;
				}
			}

			return curR;
		}

		public float GetCurrentOpacity(){
			float curO = 0;

			if (state == State.Open || state == State.Opening || state == State.invOpening) {
				if (CheckAnimType (AnimType.Opacity, true))
					curO = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time) * (openOpcty - closedOpcty) + closedOpcty;
				else
					curO = openScaleIsDefault ? openOpcty : closedOpcty;
			} else if (state == State.Close || state == State.Closing || state == State.invClosing){
				if (CheckAnimType (AnimType.Opacity, false))
					curO = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time) * (openOpcty - closedOpcty) + closedOpcty;
				else
					curO = openOpctyIsDefault ? openOpcty : closedOpcty;
			} else {
				if (open) {
					if (CheckAnimType (AnimType.Opacity, false))
						curO = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time) * (openOpcty - closedOpcty) + closedOpcty;
					else
						curO = openOpctyIsDefault ? openOpcty : closedOpcty;
				} else {
					if (CheckAnimType (AnimType.Opacity, true))
						curO = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time) * (openOpcty - closedOpcty) + closedOpcty;
					else
						curO = openScaleIsDefault ? openOpcty : closedOpcty;
				}
			}

			return curO;
		}

		public int GetCurrentInt(){
			int intValue;
			float value;

			if (state == State.Open || state == State.Opening || state == State.invOpening) {
				if (CheckAnimType (AnimType.Var, true)) {
					value = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time);
					intValue = Mathf.FloorToInt (value * (openInt - closedInt) + closedInt);
				} else {
					intValue = openVarIsDefault ? openInt : closedInt;
				}
			} else if (state == State.Close || state == State.Closing || state == State.invClosing){
				if (CheckAnimType (AnimType.Var, false)) {
					value = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time);
					intValue = Mathf.FloorToInt (value * (openInt - closedInt) + closedInt);
				}
				else {
					intValue = openVarIsDefault ? openInt : closedInt;
				}
			} else {
				if (open) {
					if (CheckAnimType (AnimType.Var, false)) {
						value = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time);
						intValue = Mathf.FloorToInt (value * (openInt - closedInt) + closedInt);
					}
					else {
						intValue = openVarIsDefault ? openInt : closedInt;
					}
				} else {
					if (CheckAnimType (AnimType.Var, true)) {
						value = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time);
						intValue = Mathf.FloorToInt (value * (openInt - closedInt) + closedInt);
					} else {
						intValue = openVarIsDefault ? openInt : closedInt;
					}
				}
			}

			return intValue;
		}

		public float GetCurrentFloat(){
			float floatValue;
			float value;

			if (state == State.Open || state == State.Opening || state == State.invOpening) {
				if (CheckAnimType (AnimType.Var, true)) {
					value = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time);
					floatValue = value * (openFloat - closedFloat) + closedFloat;
				} else {
					floatValue = openVarIsDefault ? openFloat : closedFloat;
				}
			} else if (state == State.Close || state == State.Closing || state == State.invClosing){
				if (CheckAnimType (AnimType.Var, true)) {
					value = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time);
					floatValue = value * (openFloat - closedFloat) + closedFloat;
				}
				else {
					floatValue = openVarIsDefault ? openFloat : closedFloat;
				}
			} else {
				if (open) {
					if (CheckAnimType (AnimType.Var, true)) {
						value = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time);
						floatValue = value * (openFloat - closedFloat) + closedFloat;
					}
					else {
						floatValue = openVarIsDefault ? openFloat : closedFloat;
					}
				} else {
					if (CheckAnimType (AnimType.Var, true)) {
						value = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time);
						floatValue = value * (openFloat - closedFloat) + closedFloat;
					} else {
						floatValue = openVarIsDefault ? openFloat : closedFloat;
					}
				}
			}

			return floatValue;
		}

		public bool GetCurrentBool(){
			bool boolValue;
			float value;

			if (state == State.Open || state == State.Opening || state == State.invOpening) {
				if (CheckAnimType (AnimType.Var, true)) {
					value = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time);
					boolValue = (value > 1) ? openBool : closedBool;
				} else {
					boolValue = openVarIsDefault ? openBool : closedBool;
				}
			} else if (state == State.Close || state == State.Closing || state == State.invClosing){
				if (CheckAnimType (AnimType.Var, true)) {
					value = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time);
					boolValue = (value > 1) ? openBool : closedBool;
				}
				else {
					boolValue = openVarIsDefault ? openBool : closedBool;
				}
			} else {
				if (open) {
					if (CheckAnimType (AnimType.Var, true)) {
						value = closeCurve.Evaluate (currentAnimTime * closeCurve.keys [closeCurve.length - 1].time);
						boolValue = (value > 1) ? openBool : closedBool;
					}
					else {
						boolValue = openVarIsDefault ? openBool : closedBool;
					}
				} else {
					if (CheckAnimType (AnimType.Var, true)) {
						value = openCurve.Evaluate (currentAnimTime * openCurve.keys [openCurve.length - 1].time);
						boolValue = (value > 1) ? openBool : closedBool;
					} else {
						boolValue = openVarIsDefault ? openBool : closedBool;
					}
				}
			}

			return boolValue;
		}
		#endregion

		/// <summary>
		/// Determina se a animação de abertura ou fechamento são do tipo _animType
		/// </summary>
		/// <param name="_animType">AnimType a ser verificado</param>
		/// <param name="open"><c>true</c> para checar a openAnim e <c>false</c> para checar a closeAnim.</param>
		public bool CheckAnimType (AnimType _animType, bool _open){
			if (_open)
				return ((int)openAnimType & (int)_animType) != 0;
			else
				return ((int)closeAnimType & (int)_animType) != 0;
		}

		public void ClearUnusedAnimations(){
			if (!CheckAnimType (AnimType.Translation, true) && !CheckAnimType (AnimType.Translation, false)) {
				useTransforms = false;
				openPos = Vector2.zero;
				closedPos = Vector2.zero;
				openPosIsDefault = false;
			}
			if (!CheckAnimType (AnimType.Scale, true) && !CheckAnimType (AnimType.Scale, false)) {
				openScale = Vector2.one;
				closedScale = Vector2.one;
				openScaleIsDefault = false;
			}
			if (!CheckAnimType (AnimType.Rotation, true) && !CheckAnimType (AnimType.Rotation, false)) {
				openRot = 0;
				closedRot = 0;
				openRotIsDefault = false;
			}
			if (!CheckAnimType (AnimType.Opacity, true) && !CheckAnimType (AnimType.Opacity, false)) {
				openOpcty = 1;
				closedOpcty = 1;
				openOpctyIsDefault = false;
			}
		}
	}

	RectTransform rectTrans;
	Image[] images;
	RawImage[] rawImages;
	Text[] texts;
	TextMeshProUGUI[] TMPros;

	Vector3 normalPos;
	Vector2 normalScale;
	float normalRot;
	float[] normalOpctyImages;
	float[] normalOpctyRawImages;
	float[] normalOpctyTexts;
	float[] normalOpctyTMPros;

	public Animation[] anims;

	public bool CheckAnimTypeInArray(Animation.AnimType atype){
		for (int i = 0; i < anims.Length; i++) {
			if (anims [i].CheckAnimType (atype, true) || anims [i].CheckAnimType (atype, false)) {
				return true;
			}
		}
		return false;
	}

	// Use this for initialization
	void Awake () {
		rectTrans = GetComponent<RectTransform> ();
		images = GetComponentsInChildren<Image> ();
		rawImages = GetComponentsInChildren<RawImage> ();
		texts = GetComponentsInChildren<Text> ();
		TMPros = GetComponentsInChildren<TextMeshProUGUI> ();

		normalPos = rectTrans.localPosition;
		normalScale = rectTrans.localScale;
		normalRot = rectTrans.rotation.eulerAngles.z;

		normalOpctyImages = new float[images.Length];
		for (int i = 0; i < images.Length; i++) {
			normalOpctyImages [i] = images [i].color.a;
		}
		normalOpctyRawImages = new float[rawImages.Length];
		for (int i = 0; i < rawImages.Length; i++) {
			normalOpctyRawImages [i] = rawImages [i].color.a;
		}
		normalOpctyTexts = new float[texts.Length];
		for (int i = 0; i < texts.Length; i++) {
			normalOpctyTexts [i] = texts [i].color.a;
		}
		normalOpctyTMPros = new float[TMPros.Length];
		for (int i = 0; i < TMPros.Length; i++) {
			normalOpctyTMPros [i] = TMPros [i].color.a;
		}

		foreach (Animation an in anims) {
			an.ClearUnusedAnimations ();
		}
	}

	void Start(){
		if (anims.Length > 0)
			SetPosition ();
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Animation an in anims) {
			an.Animate ();
		}

		if (anims.Length > 0)
			SetPosition ();
	}

	void SetPosition(){
		Vector3 finalPos = Vector3.zero;
		Vector2 finalScale = Vector2.one;
		float finalRotation = 0;
		float finalOpcty = 1;

		foreach (Animation an in anims) {
			finalPos += an.GetCurrentTranslation() - ((an.useTransforms && (an.CheckAnimType(Animation.AnimType.Translation,true) || an.CheckAnimType(Animation.AnimType.Translation,false))) ? normalPos : Vector3.zero);
			finalScale = new Vector2(finalScale.x*an.GetCurrentScale ().x, finalScale.y*an.GetCurrentScale ().y);
			finalRotation += an.GetCurrentRotation ();
			finalOpcty *= an.GetCurrentOpacity ();
		}

		if (CheckAnimTypeInArray(Animation.AnimType.Translation))
			rectTrans.localPosition = finalPos + normalPos;
		if (CheckAnimTypeInArray(Animation.AnimType.Scale))
			rectTrans.localScale = new Vector3(finalScale.x * normalScale.x, finalScale.y * normalScale.y, finalScale.x * normalScale.x);
		if (CheckAnimTypeInArray(Animation.AnimType.Rotation))
			rectTrans.localRotation = Quaternion.Euler (0, 0, finalRotation + normalRot);
		if (CheckAnimTypeInArray (Animation.AnimType.Opacity)) {
			for (int i = 0; i < images.Length; i++) {
				if (images [i] != null && (images [i].gameObject == gameObject || images [i].GetComponent<UIAnim> () == null || !images [i].GetComponent<UIAnim> ().CheckAnimTypeInArray (Animation.AnimType.Opacity))) {
					Color c = images [i].color;
					c.a = finalOpcty * normalOpctyImages [i];
					images [i].color = c;
				}
			}
			for (int i = 0; i < rawImages.Length; i++) {
				if (rawImages [i] != null && (rawImages [i].gameObject == gameObject || rawImages [i].GetComponent<UIAnim> () == null || !rawImages [i].GetComponent<UIAnim> ().CheckAnimTypeInArray (Animation.AnimType.Opacity))) {
					Color c = rawImages [i].color;
					c.a = finalOpcty * normalOpctyRawImages [i];
					rawImages [i].color = c;
				}
			}
			for (int i = 0; i < texts.Length; i++) {
				if (texts [i] != null && (texts [i].gameObject == gameObject || texts [i].GetComponent<UIAnim> () == null || !texts [i].GetComponent<UIAnim> ().CheckAnimTypeInArray (Animation.AnimType.Opacity))) {
					Color c = texts [i].color;
					c.a = finalOpcty * normalOpctyTexts [i];
					texts [i].color = c;
				}
			}
			for (int i = 0; i < TMPros.Length; i++) {
				if (TMPros [i] != null && (TMPros [i].gameObject == gameObject || TMPros [i].GetComponent<UIAnim> () == null || !TMPros [i].GetComponent<UIAnim> ().CheckAnimTypeInArray (Animation.AnimType.Opacity))) {
					Color c = TMPros [i].color;
					c.a = finalOpcty * normalOpctyTMPros [i];
					TMPros [i].color = c;
				}
			}
		}
	}

	public void OpenAnim(int animIndex){
		anims [animIndex].open = true;
	}

	public void CloseAnim(int animIndex){
		anims [animIndex].open = false;
	}
}
