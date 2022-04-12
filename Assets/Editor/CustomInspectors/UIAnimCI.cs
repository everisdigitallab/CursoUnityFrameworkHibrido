using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Events;


[CustomEditor(typeof (UIAnim), true)]
public class UIAnimCI : Editor {
	bool[] showThisAnim;

	public override void OnInspectorGUI(){
		//Pegar referência do script
		UIAnim UiA = (UIAnim)target;
		RectTransform trans = UiA.GetComponent<RectTransform> ();

		if (UiA.anims == null) {
			UiA.anims = new UIAnim.Animation[0];
		}
			
		int newSize = UiA.anims.Length;
		newSize = EditorGUILayout.DelayedIntField ("Size", newSize);
		if (newSize != UiA.anims.Length) {
			while (newSize < UiA.anims.Length) {
				RemoveAnimationArray (ref UiA.anims);
			}
			while (newSize > UiA.anims.Length) {
				AddAnimationArray (ref UiA.anims);
			}
			showThisAnim = new bool[UiA.anims.Length];
		}
		if (showThisAnim == null)
			showThisAnim = new bool[UiA.anims.Length];

		//para pegar unity event
		SerializedProperty animsProperty = serializedObject.FindProperty("anims");

		for (int i = 0; i < UiA.anims.Length; i++) {
			if (UiA.anims [i] == null)
				UiA.anims [i] = new UIAnim.Animation ();
			
			showThisAnim [i] = EditorGUILayout.Foldout (showThisAnim [i], "Anim " + i);

			if (showThisAnim [i]) {
				EditorGUI.indentLevel++;

				UiA.anims [i].state = (UIAnim.Animation.State)EditorGUILayout.EnumPopup ("State", UiA.anims [i].state);
				UiA.anims [i].open = EditorGUILayout.Toggle ("Open", UiA.anims [i].open);

				UiA.anims [i].openAnimType = (UIAnim.Animation.AnimType)EditorGUILayout.EnumFlagsField ("Open Anim Type", UiA.anims [i].openAnimType);
				UiA.anims [i].openCurve = EditorGUILayout.CurveField ("Open Curve", UiA.anims [i].openCurve);

				UiA.anims [i].closeAnimType = (UIAnim.Animation.AnimType)EditorGUILayout.EnumFlagsField ("Close Anim Type", UiA.anims [i].closeAnimType);
				UiA.anims [i].closeCurve = EditorGUILayout.CurveField ("Close Curve", UiA.anims [i].closeCurve);
		
				UiA.anims [i].currentAnimTime = EditorGUILayout.Slider ("Time", UiA.anims [i].currentAnimTime, 0, 1);

				EditorGUILayout.Space ();
				#region Translation
				if (UiA.anims [i].CheckAnimType (UIAnim.Animation.AnimType.Translation, true) || UiA.anims [i].CheckAnimType (UIAnim.Animation.AnimType.Translation, false)) {
					EditorGUILayout.LabelField ("Translation");
					EditorGUI.indentLevel++;

					UiA.anims [i].useTransforms = EditorGUILayout.Toggle("Use Transforms?", UiA.anims [i].useTransforms);
					if (UiA.anims [i].useTransforms){
						UiA.anims [i].closedTransform = (Transform)EditorGUILayout.ObjectField("Closed Transform", UiA.anims [i].closedTransform, typeof(Transform), true);
						UiA.anims [i].openTransform = (Transform)EditorGUILayout.ObjectField("Open Transform", UiA.anims [i].openTransform, typeof(Transform), true);
					} else {
						EditorGUILayout.BeginHorizontal ();

						UiA.anims [i].closedPos = EditorGUILayout.Vector2Field ("Closed Pos", UiA.anims [i].closedPos);
						Color bgColor = GUI.backgroundColor;
						GUI.backgroundColor = Color.green;
						if (GUILayout.Button ("", GUILayout.Width (20f))) {
							UiA.anims [i].openPos -= UiA.anims [i].closedPos - Vector2.zero;
							UiA.anims [i].closedPos = Vector2.zero;
						}
						GUI.backgroundColor = Color.red;
						if (GUILayout.Button ("", GUILayout.Width (20f))) {
							UiA.anims [i].closedPos = trans.localPosition;
						}
						GUI.backgroundColor = bgColor;
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						UiA.anims [i].openPos = EditorGUILayout.Vector2Field ("Open Pos", UiA.anims [i].openPos);
						GUI.backgroundColor = Color.green;
						if (GUILayout.Button ("", GUILayout.Width (20f))) {
							UiA.anims [i].closedPos -= UiA.anims [i].openPos - Vector2.zero;
							UiA.anims [i].openPos = Vector2.zero;
						}
						GUI.backgroundColor = Color.red;
						if (GUILayout.Button ("", GUILayout.Width (20f))) {
							UiA.anims [i].openPos = trans.localPosition;
						}
						GUI.backgroundColor = bgColor;
						EditorGUILayout.EndHorizontal ();
					}

					UiA.anims [i].openPosIsDefault = EditorGUILayout.Toggle ("Open Default", UiA.anims [i].openPosIsDefault);
					EditorGUI.indentLevel--;
					EditorGUILayout.Space ();
				}
				#endregion

				#region Scale
				if (UiA.anims [i].CheckAnimType (UIAnim.Animation.AnimType.Scale, true) || UiA.anims [i].CheckAnimType (UIAnim.Animation.AnimType.Scale, false)) {
					EditorGUILayout.LabelField ("Scale");
					EditorGUI.indentLevel++;

					EditorGUILayout.BeginHorizontal ();
					UiA.anims [i].closedScale = EditorGUILayout.Vector2Field ("Closed Scale", UiA.anims [i].closedScale);
					Color bgColor = GUI.backgroundColor;
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						UiA.anims [i].openScale -= UiA.anims [i].closedScale - Vector2.one;
						UiA.anims [i].closedScale = Vector2.one;
					}
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						UiA.anims [i].closedScale = trans.localScale;
					}
					GUI.backgroundColor = bgColor;
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					UiA.anims [i].openScale = EditorGUILayout.Vector2Field ("Open Scale", UiA.anims [i].openScale);
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						UiA.anims [i].closedScale -= UiA.anims [i].openScale - Vector2.one;
						UiA.anims [i].openScale = Vector2.one;
					}
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						UiA.anims [i].openScale = trans.localScale;
					}
					GUI.backgroundColor = bgColor;
					EditorGUILayout.EndHorizontal ();

					UiA.anims [i].openScaleIsDefault = EditorGUILayout.Toggle ("Open Default", UiA.anims [i].openScaleIsDefault);
					EditorGUI.indentLevel--;
					EditorGUILayout.Space ();
				}
				#endregion

				#region Rotation
				if (UiA.anims [i].CheckAnimType (UIAnim.Animation.AnimType.Rotation, true) || UiA.anims [i].CheckAnimType (UIAnim.Animation.AnimType.Rotation, false)) {
					EditorGUILayout.LabelField ("Rotation");
					EditorGUI.indentLevel++;

					EditorGUILayout.BeginHorizontal ();
					UiA.anims [i].closedRot = EditorGUILayout.DelayedFloatField ("Closed Rot", UiA.anims [i].closedRot);
					Color bgColor = GUI.backgroundColor;
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						UiA.anims [i].openRot -= UiA.anims [i].closedRot - 0;
						UiA.anims [i].closedRot = 0;
					}
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						UiA.anims [i].closedRot = trans.rotation.eulerAngles.z;
					}
					GUI.backgroundColor = bgColor;
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					UiA.anims [i].openRot = EditorGUILayout.DelayedFloatField ("Open Rot", UiA.anims [i].openRot);
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						UiA.anims [i].closedRot -= UiA.anims [i].openRot - 0;
						UiA.anims [i].openRot = 0;
					}
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						UiA.anims [i].openRot = trans.rotation.eulerAngles.z;
					}
					GUI.backgroundColor = bgColor;
					EditorGUILayout.EndHorizontal ();

					UiA.anims [i].openRotIsDefault = EditorGUILayout.Toggle ("Open Default", UiA.anims [i].openRotIsDefault);
					EditorGUI.indentLevel--;
					EditorGUILayout.Space ();
				}
				#endregion

				#region Opacity
				if (UiA.anims [i].CheckAnimType (UIAnim.Animation.AnimType.Opacity, true) || UiA.anims [i].CheckAnimType (UIAnim.Animation.AnimType.Opacity, false)) {
					EditorGUILayout.LabelField ("Opacity");
					EditorGUI.indentLevel++;

					EditorGUILayout.BeginHorizontal ();
					UiA.anims [i].closedOpcty = EditorGUILayout.DelayedFloatField ("Closed Opacity", UiA.anims [i].closedOpcty);
					Color bgColor = GUI.backgroundColor;
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						UiA.anims [i].openOpcty -= UiA.anims [i].closedOpcty - 1;
						UiA.anims [i].closedOpcty = 1;
					}
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						if (UiA.GetComponent<Image> () != null)
							UiA.anims [i].closedOpcty = UiA.GetComponent<Image> ().color.a;
						else if (UiA.GetComponent<RawImage> () != null)
							UiA.anims [i].closedOpcty = UiA.GetComponent<RawImage> ().color.a;
						else if (UiA.GetComponent<Text> () != null)
							UiA.anims [i].closedOpcty = UiA.GetComponent<Text> ().color.a;
						else if (UiA.GetComponent<TextMeshProUGUI> () != null)
							UiA.anims [i].closedOpcty = UiA.GetComponent<TextMeshProUGUI> ().color.a;
					}
					GUI.backgroundColor = bgColor;
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					UiA.anims [i].openOpcty = EditorGUILayout.DelayedFloatField ("Open Opacity", UiA.anims [i].openOpcty);
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						UiA.anims [i].closedOpcty -= UiA.anims [i].openOpcty - 1;
						UiA.anims [i].openOpcty = 1;
					}
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button ("", GUILayout.Width (20f))) {
						if (UiA.GetComponent<Image> () != null)
							UiA.anims [i].openOpcty = UiA.GetComponent<Image> ().color.a;
						else if (UiA.GetComponent<RawImage> () != null)
							UiA.anims [i].openOpcty = UiA.GetComponent<RawImage> ().color.a;
						else if (UiA.GetComponent<Text> () != null)
							UiA.anims [i].openOpcty = UiA.GetComponent<Text> ().color.a;
						else if (UiA.GetComponent<TextMeshProUGUI> () != null)
							UiA.anims [i].openOpcty = UiA.GetComponent<TextMeshProUGUI> ().color.a;
					}
					GUI.backgroundColor = bgColor;
					EditorGUILayout.EndHorizontal ();

					UiA.anims [i].openOpctyIsDefault = EditorGUILayout.Toggle ("Open Default", UiA.anims [i].openOpctyIsDefault);
					EditorGUI.indentLevel--;
					EditorGUILayout.Space ();
				}
				#endregion

				#region Var
				if (UiA.anims [i].CheckAnimType (UIAnim.Animation.AnimType.Var, true) || UiA.anims [i].CheckAnimType (UIAnim.Animation.AnimType.Var, false)) {
					EditorGUILayout.LabelField ("Var");
					EditorGUI.indentLevel++;

					EditorGUILayout.LabelField ("Int");
					EditorGUI.indentLevel++;
					UiA.anims [i].closedInt = EditorGUILayout.DelayedIntField ("Closed", UiA.anims [i].closedInt);
					UiA.anims [i].openInt = EditorGUILayout.DelayedIntField ("Open", UiA.anims [i].openInt);
					EditorGUI.indentLevel--;

					EditorGUILayout.LabelField ("Float");
					EditorGUI.indentLevel++;
					UiA.anims [i].closedFloat = EditorGUILayout.DelayedFloatField ("Closed", UiA.anims [i].closedFloat);
					UiA.anims [i].openFloat = EditorGUILayout.DelayedFloatField ("Open", UiA.anims [i].openFloat);
					EditorGUI.indentLevel--;

					EditorGUILayout.LabelField ("Bool");
					EditorGUI.indentLevel++;
					UiA.anims [i].closedBool = EditorGUILayout.Toggle ("Closed", UiA.anims [i].closedBool);
					UiA.anims [i].openBool = EditorGUILayout.Toggle ("Open", UiA.anims [i].openBool);
					EditorGUI.indentLevel--;

					UiA.anims [i].openVarIsDefault = EditorGUILayout.Toggle ("Open Default", UiA.anims [i].openVarIsDefault);
					EditorGUI.indentLevel--;
					EditorGUILayout.Space ();
				}
				#endregion

				EditorGUILayout.LabelField ("Controlled Animation");
				EditorGUI.indentLevel++;
				UiA.anims [i].controlVelToComplete = EditorGUILayout.Slider ("Vel to Complete", UiA.anims [i].controlVelToComplete, 0, 1);
				UiA.anims [i].controlPosToComplete = EditorGUILayout.Slider ("Pos to Complete", UiA.anims [i].controlPosToComplete, 0, 1);
				EditorGUI.indentLevel--;
				EditorGUILayout.Space ();

				SerializedProperty thisAnimProperty = animsProperty.GetArrayElementAtIndex (i);
				SerializedProperty onOpenStart = thisAnimProperty.FindPropertyRelative ("onOpenStart");
				SerializedProperty onOpenEnd = thisAnimProperty.FindPropertyRelative ("onOpenEnd");
				SerializedProperty onCloseStart = thisAnimProperty.FindPropertyRelative ("onCloseStart");
				SerializedProperty onCloseEnd = thisAnimProperty.FindPropertyRelative ("onCloseEnd");


				//SerializedObject o = new SerializedObject (UiA.anims [i]);
				//SerializedProperty onOpenStart = serializedObject.FindProperty("onOpenStart");
				//SerializedProperty onOpenEnd = o.FindProperty("onOpenEnd");
				//SerializedProperty onCloseStart = o.FindProperty("onCloseStart");
				//SerializedProperty onCloseEnd = o.FindProperty("onCloseEnd");

				EditorGUIUtility.LookLikeControls();
				EditorGUILayout.PropertyField(onOpenStart);
				EditorGUILayout.PropertyField(onOpenEnd);
				EditorGUILayout.PropertyField(onCloseStart);
				EditorGUILayout.PropertyField(onCloseEnd);
				if(GUI.changed)
				{
					serializedObject.ApplyModifiedProperties();
				}
				EditorGUI.indentLevel--;
			}
		}
			
		if (GUI.changed) {
			EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			EditorUtility.SetDirty (target);
		}
	}

	void AddAnimationArray(ref UIAnim.Animation[] animArray){
		UIAnim.Animation[] tempArray = new UIAnim.Animation[animArray.Length + 1];
		for (int i = 0; i < tempArray.Length; i++) {
			if (i < animArray.Length) {
				tempArray [i] = animArray [i];
			} else {
				tempArray [i] = new UIAnim.Animation ();
			}
		}
		animArray = tempArray;
	}

	void RemoveAnimationArray(ref UIAnim.Animation[] animArray){
		UIAnim.Animation[] tempArray = new UIAnim.Animation[animArray.Length - 1];
		for (int i = 0; i < tempArray.Length; i++) {
			tempArray [i] = animArray [i];
		}
		animArray = tempArray;
	}
}
