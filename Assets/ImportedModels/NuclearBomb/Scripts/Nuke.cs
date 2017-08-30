using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nuke : MonoBehaviour {
    public GameObject Head;
    public ParticleSystem Body;
    public float Target;
    public GameObject blast;
    public LensFlare _light;
    public GameObject bomb;
    public GameObject[] particles;
    public GameObject crater;
    public Light m_light;
    public bool detonateByE;
    
    [Range (1, 10)]
    public float time;
    private float m_height;
    private bool detonated;
    private Renderer m_blast;
    private float m_speed = 1;

	// Use this for initialization
	void Start () {
        //Set speed base on body's particle speed and the height based on Target's Y postion
        this.m_height = Target;
        blast.transform.localScale = Vector3.zero;
        crater.GetComponent <Renderer> ().material.renderQueue = 2000;
        blast.GetComponent <Renderer> ().material.renderQueue = 2000;
        m_blast = blast.GetComponent <Renderer> ();
    }
	
	public void Detonate () {
		StartCoroutine (flare ());
		foreach (GameObject g in particles) g.SetActive (true);
		detonated = true;
		Destroy (bomb);
		StartCoroutine (showCrater ());
		StartCoroutine (fadeLight ());
	}
	
	IEnumerator showCrater () {
		yield return new WaitForSeconds (2);
		crater.SetActive (true);
	}
	
	IEnumerator fadeLight () {
		float init = m_light.intensity;
		float time = 5;
		for (float t = 0; t < 1; t += Time.deltaTime / time) {
			m_light.intensity = Mathf.Lerp (
				init,
				0,
				t
			);
			yield return 0;
		}
		Destroy (GetComponent<Light>());
	}
	
	IEnumerator flare () {
		for (float t = 0; t < 1; t += Time.deltaTime * 5) {
			_light.brightness = Mathf.Lerp (
				_light.brightness,
				5,
				t
			);
			yield return 0;
		}
		for (float t = 0; t < 1; t += Time.deltaTime / 250) {
			_light.brightness = Mathf.Lerp (
				_light.brightness,
				0,
				t
			);
			_light.color = Color.Lerp (
				_light.color,
				Color.clear,
				t
			);
			yield return 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (detonated) {
			Head.transform.localPosition = Vector3.Lerp (
				Head.transform.localPosition,
				Vector3.zero.SetZ (m_height),
				Time.deltaTime / time
			);
			if (blast != null)
				if(blast.transform.localScale.x > 5f) Destroy (blast);
				else {
					blast.transform.localScale += (Vector3.one / 100) * m_speed;
					SetAlpha (Mathf.Lerp (m_blast.material.color.a, 0, Time.deltaTime / 3));
					m_speed = Mathf.Clamp (m_speed - .001f, 0, 1);
				}
		} else if (Input.GetKeyDown (KeyCode.E) && detonateByE) Detonate ();
	}
	
	void SetAlpha (float a) {
		Color init = m_blast.material.color;
		init.a = a;
		m_blast.material.color = init;
	}
}

public static class extensions {
	public static Vector3 SetX (this Vector3 vec, float x) { vec.x = x; return vec; }
	public static Vector3 SetY (this Vector3 vec, float y) { vec.y = y; return vec; }
	public static Vector3 SetZ (this Vector3 vec, float z) { vec.z = z; return vec; }
	public static void copyFrom (this Transform t, Transform other) {
		t.position = other.position;
		t.rotation = other.rotation;
		t.localScale = other.localScale;
	}
	public static bool isTags (this Collider col, string[] tags) {
		foreach (string tag in tags) if (tag == col.tag) return true;
		return false;
	}
}