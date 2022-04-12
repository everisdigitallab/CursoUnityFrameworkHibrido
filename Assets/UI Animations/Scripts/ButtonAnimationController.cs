using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimationController : MonoBehaviour {
	private RectTransform trans;

	public enum Anim {scale, wave};
	public List<Anim> anims;	//tipo de animação

	private float animTime;	//tempo da animação
	private bool animationRunning;	//detecta se está tocando animação
	private float currentState;	//0 = normal, 1= anim

	public Vector2 normalScale = Vector2.one;	//escala do botão parado
	public Vector2 animScale = Vector2.one;	//escala do botão no pico da animação (estadoAtual = 1)
	public AnimationCurve animCurve = AnimationCurve.Constant (0, 0.2f, 0);	//curva da animação

	public GameObject waveMask;	//Objeto pai das waves
	public GameObject wavePrefab;	//prefab da wave
	public Sprite waveImage;	//imagem da wave
	public int wavesNumber;	//número de waves
	public float timeBetweenWaves;	//tempo entre waves
	public Vector2 finalWaveScale;	//scale final da wave
	public Color initialWaveColor;	//cor inicial da wave
	public Color finalWaveColor;	//cor final da wave
	public float initialWaveRot;	//rotação inicial da wave
	public float finalWaveRot;	//rotação final da wave
	public AnimationCurve waveAnimCurve = AnimationCurve.EaseInOut (0, 0, 0.2f, 1);	//curva de animação da wave

	// Use this for initialization
	void Start () {
		trans = GetComponent<RectTransform> ();
	}

	void Update(){
		if (animationRunning) {
			animTime += Time.deltaTime;
			currentState = animCurve.Evaluate (animTime);
			if (animTime > animCurve.keys [animCurve.length - 1].time) {
				animationRunning = false;
				animTime = 0;
			}
		}

		SetScale ();
	}

	//toca animação
	public void Animate () {
		if (anims.Contains (Anim.scale)) {
			animationRunning = true;
			animTime = 0;
		}
		if (anims.Contains (Anim.wave)) {
			StartCoroutine (CreateWaves (Input.mousePosition));
		}
	}

	IEnumerator CreateWaves(Vector3 origin){
		int i = 0;
		while (i < wavesNumber) {
			i++;
			//criar onda
			GameObject wave = Instantiate(wavePrefab, new Vector3(100,100,0), Quaternion.Euler(0,0,initialWaveRot), waveMask.transform);
			wave.transform.position = origin;
			wave.transform.localScale = Vector3.zero;
			WaveAnimation wavAnim = wave.GetComponent<WaveAnimation> ();
			wavAnim.finalScale = finalWaveScale;
			wavAnim.initialColor = initialWaveColor;
			wavAnim.finalColor = finalWaveColor;
			wavAnim.initialRot = initialWaveRot;
			wavAnim.finalRot = finalWaveRot;
			wavAnim.animCurve = waveAnimCurve;
			wavAnim.img.sprite = waveImage;
			yield return new WaitForSeconds(timeBetweenWaves);
		}
	}

	void SetScale(){
		float sx = currentState * (animScale.x - normalScale.x) + normalScale.x;
		float sy = currentState * (animScale.y - normalScale.y) + normalScale.y;
		trans.localScale = new Vector2 (sx, sy);
	}
}
