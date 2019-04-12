using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Author: Daryl Keogh
/// Description: Thisscript is used on a text object that is spawned when the playe ror an enemy takes damage.
/// it will move the object up, and fade out while doing so. It will also destroy itself after 1 second.
/// This is simply used as feedback for damage, so if the player does 5 damage to an enemy, and object will spawn with the
/// text been set to 5.0 and will mov eup on the screen and fade out.
/// </summary>
public class DamageIndicator : MonoBehaviour {


    float speed, ttl;
    Text text;

	// Use this for initialization
	void Start () {
        speed = .75f;
        ttl = 0;
        text = transform.GetChild(0).GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        ttl += Time.deltaTime;

        if(ttl >= 1.0f)
        {
            Destroy(this.gameObject);
        }

        //Fade out the text the longer it is alive
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - .95f * Time.deltaTime);

        //Slide the indicator up
        transform.position += new Vector3(1, 1, 0) * speed * Time.deltaTime;
	}
}
