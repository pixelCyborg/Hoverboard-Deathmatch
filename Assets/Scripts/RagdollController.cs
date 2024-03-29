﻿using UnityEngine;
using System.Collections;

public class RagdollController : MonoBehaviour {
    Rigidbody[] body;
    float[] weights;

	// Use this for initialization
	void Start () {
        body = GetComponentsInChildren<Rigidbody>();
        weights = new float[body.Length];
        for(int i = 0; i < body.Length; i++)
        {
            body[i].useGravity = false;
            body[i].isKinematic = true;
            weights[i] = body[i].mass;
            body[i].mass = 0;
        }
    }
	
	// Update is called once per frame
	public void Ragdoll () {
	    for(int i = 0; i < body.Length; i++)
        {
            body[i].tag = "Untagged";
            body[i].useGravity = true;
            body[i].isKinematic = false;
            body[i].mass = weights[i];
        }
        GetComponent<Animator>().enabled = false;
    }

    public void UnRagdoll()
    {
        for (int i = 0; i < body.Length; i++)
        {
            body[i].tag = "Player";
            body[i].useGravity = false;
            body[i].isKinematic = true;
            body[i].mass = 0;
        }
        GetComponent<Animator>().enabled = true;
    }
}
