using UnityEngine;
using System.Collections;

public class BoltMover : MonoBehaviour {

	public float speed;

	void Start() {
		Rigidbody rigidBody = GetComponent<Rigidbody>();

		Vector3 velocity = new Vector3(
			Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad), 
			0.0f, 
			Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad));
		velocity *= speed;
		rigidBody.velocity = velocity;
	}
}
