using UnityEngine;
using System.Collections;

public class LabelLock : MonoBehaviour {
	// This script is attached to the labels that are child objects of the asteroids. 
	// This forces the labels to not rotate with the spinning parent asteroids.

	Vector3 position;
	Quaternion rotation;

	void Awake() {
		position = transform.position;
		rotation = transform.rotation;
	}

	void LateUpdate() {
		transform.position = position;
		transform.rotation = rotation;
	}
}
