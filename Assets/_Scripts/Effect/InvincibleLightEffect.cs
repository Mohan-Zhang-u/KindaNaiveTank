using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleLightEffect : MonoBehaviour {
    public Light lt;


    Color[] LerpTab = new Color[3];
    public float duration = 5f;
    public float t;
    int looper = 0;
    // Use this for initialization
    void Start () {
        lt = GetComponent<Light>();

        LerpTab[0] = Color.red;
        LerpTab[1] = Color.blue;
        LerpTab[2] = Color.green;
    }
	
	// Update is called once per frame
	void Update () {
        t = Mathf.PingPong(Time.time, duration) / duration;

        lt.color = Color.Lerp(lt.color, LerpTab[looper], t);

        if (lt.color == LerpTab[looper])
        {
            looper++;
            looper = looper % LerpTab.Length;
        }
    }
}
