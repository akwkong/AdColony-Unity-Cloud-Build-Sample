using UnityEngine;
using System.Collections;

public class RocketSpin : MonoBehaviour {

	public float spinSpeed = -100f;

	void Update() {
		transform.Rotate(Vector3.up, Time.deltaTime * spinSpeed);
	}
}
