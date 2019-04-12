using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Daryl Keogh
/// Description: This script is used to destroy an objects parent object when the method Destroy is called. It can be useful
/// in the animator
/// </summary>
public class DeathTrigger : MonoBehaviour {

    [SerializeField]
    GameObject parent;

	public void Destroy()
    {
        Destroy(parent);
    }
}
