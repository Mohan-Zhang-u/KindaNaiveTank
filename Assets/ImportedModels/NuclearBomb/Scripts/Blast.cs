using UnityEngine;
using System.Collections;

/// <summary>
/// Experimental: needs a lot of improvement!
/// </summary>

public class Blast : MonoBehaviour {
	public GameObject[] debris;
	public int debrisCount;
	public bool generateDebris;
	
	void OnTriggerEnter (Collider col) {
		if (hasMeshRenderer (col.gameObject) && !col.isTags (new string[2] { "Nuke", "Debris" }))
			StartCoroutine (Disintegrate (col.gameObject));
	}
	
	bool hasMeshRenderer (GameObject obj) {
		return obj.GetComponent <Renderer> () != null || obj.GetComponentInChildren <Renderer> () != null;
	}
	
	Color distanceBasedGrayShade (float distance) {
		float s = Mathf.Clamp (Mathf.Pow (distance / 200, 2), 0, 1);
		return new Color (s, s, s);
	}
	
	IEnumerator Disintegrate (GameObject go) {
		float time = 0.225f;
		Color initialColor = distanceBasedGrayShade (Vector3.Distance (go.transform.position, transform.position));
		//Make the object transparent, so we can fade it:
		if (go.GetComponent <Renderer> ()) {
			go.GetComponent <Renderer> ().material.SetFloat ("_Mode", 2);
		} else {
			go.GetComponentInChildren <Renderer> ().material.SetFloat ("_Mode", 2);
		}
		for (float t = 0; t < 1.0f; t += Time.deltaTime / time) {
			//Makes the object black
			if (go.GetComponent <Renderer> ()) {
				go.GetComponent <Renderer> ().material.color = Color.Lerp (
					Color.white,
					initialColor,
					t
				);
			} else {
				go.GetComponentInChildren <Renderer> ().material.color = Color.Lerp (
					Color.white,
					initialColor,
					t
				);
			}
			yield return 0;
		}
		Random.seed = (int)System.DateTime.Now.Ticks;
		if (Random.value > .05f || Vector3.Distance (go.transform.position, transform.position) < 100) {
			time = 0.025f;
			for (float t = 0; t < 1.0f; t += Time.deltaTime / time) {
				//Fades the object away
				if (go.GetComponent <Renderer> ()) {
					go.GetComponent <Renderer> ().material.color = Color.Lerp (
						Color.black,
						new Color32 (0, 0, 0, 0),
						t
					);
				} else {
					go.GetComponentInChildren <Renderer> ().material.color = Color.Lerp (
						Color.black,
						new Color32 (0, 0, 0, 0),
						t
					);
				}
				yield return 0;
			}
			
			if (debris.Length > 0 && generateDebris) {
				//Generate a bunch of meshes to use as debris
				for (int i = 0; i < (int)(debrisCount * go.transform.localScale.magnitude); i++) {
					GameObject g = Instantiate (debris[Random.Range (0, debris.Length - 1)], go.transform.position + Random.insideUnitSphere * go.transform.localScale.x, Random.rotation) as GameObject;
					g.SetActive (true);
					g.hideFlags = HideFlags.HideInHierarchy;
					g.AddComponent <Rigidbody> ().mass = .1f;
					g.GetComponent <Rigidbody> ().AddExplosionForce (80, transform.position, 500);
					g.AddComponent <AutoDestroyInXSeconds> ().t = Random.Range (3, 5);
				}
			} else if (generateDebris) Debug.LogWarning ("No debris meshes were added, skipping");
			
			Destroy (go);
		} else
			if (go.GetComponent <Renderer> ()) {
				go.GetComponent <Renderer> ().material.SetFloat ("_Mode", 0);
			} else {
				go.GetComponentInChildren <Renderer> ().material.SetFloat ("_Mode", 0);
			}
	}
}

public class AutoDestroyInXSeconds : MonoBehaviour {
	public float t;
	
	IEnumerator Start () {
		yield return new WaitForSeconds (t);
		Destroy (gameObject);
	}
}
