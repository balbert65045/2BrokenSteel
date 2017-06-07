using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathBlocks : MonoBehaviour {

    public float Lifetime = 1f;
    private float StartTime;
	// Use this for initialization
	void Start () {
        StartTime = Time.timeSinceLevelLoad;
    }
	
	// Update is called once per frame
	void Update () {
		if (StartTime + Lifetime < Time.timeSinceLevelLoad)
        {
            Destroy(gameObject);
        }
	}
}
