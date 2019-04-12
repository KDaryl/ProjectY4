using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: This class is used to move the spell that is cast by the players AI companion.
/// it will spawn the spell item itself and move towards its target and also spawn an impact particle iff hit.
/// </summary>
public class SpellMover : MonoBehaviour {

    //Particle that is spawned on hit
    [SerializeField]
    GameObject hitParticle;

    Rigidbody rb;
    float timer;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        timer = 2.5f; //Lives for 2 and a half seconds
	}

    //Simply looks at the target of the AI
    public void SpellLookAt(GameObject target)
    {
        transform.LookAt(target.transform);
    }

	
	void FixedUpdate () {
        timer -= Time.fixedDeltaTime;

        if (timer <= 0)
            Destroy(this);

        rb.AddRelativeForce(Vector3.forward * 50 * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    //Destroy the spell and spawns an impact particle object
    public void DestroySpell(Vector3 pos)
    {
        Instantiate(hitParticle, pos, hitParticle.transform.rotation);
        Destroy(gameObject);
    }
}
