using UnityEngine;
using System.Collections;

public class EngineFlare : MonoBehaviour {

	public float engineScaleMin = 0.1f;
	public float engineScaleMax = 0.14f;

	void Update() {
		float engineScale = (Random.value * (engineScaleMax - engineScaleMin)) + engineScaleMin;
		transform.localScale = new Vector3(transform.localScale.x, engineScale, transform.localScale.z);
	}
}
