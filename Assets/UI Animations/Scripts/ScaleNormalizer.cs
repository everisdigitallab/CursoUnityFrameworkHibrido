using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleNormalizer : MonoBehaviour {
	private Transform parentTrans;
	private Transform myTrans;

	// Use this for initialization
	void Start () {
		myTrans = transform;
		parentTrans = myTrans.parent;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float sx = parentTrans.localScale.x == 0 ? 0 : 1f / parentTrans.localScale.x;
		float sy = parentTrans.localScale.y == 0 ? 0 : 1f / parentTrans.localScale.y;
		float sz = parentTrans.localScale.z == 0 ? 0 : 1f / parentTrans.localScale.z;
		myTrans.localScale = new Vector3(sx, sy, sz);
	}
}
