using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {

	enum RocketState {Waiting, RotateToTarget, FirePause, RotateToBase};

	float targetRotation = 0f;
	float currentRotation = 0f;

	RocketState state = RocketState.Waiting;
	float firePauseTimer = 0f;

	public float baseRotation = 35f;
	public float rotateSpeed = 200f;
	public GameObject bolt;
	public Transform boltLaunchPoint;

	void Start() {
		currentRotation = targetRotation = baseRotation;
	}

	void Update() {
		switch (state) {
		case RocketState.RotateToTarget:
		case RocketState.RotateToBase:
			bool clockwise = true;
			float rotDist = targetRotation - currentRotation;
			if (rotDist < 0) {
				rotDist += 360f;
			}

			if (rotDist > 180f) {
				rotDist -= 180f;
				clockwise = false;
			}

			float rotDelta = Time.deltaTime * rotateSpeed;
			if (rotDist <= rotDelta) {
				currentRotation = targetRotation;
				if (state == RocketState.RotateToTarget) {
					state = RocketState.FirePause;
					firePauseTimer = 1f;

					Fire ();
				} else {
					state = RocketState.Waiting;
				}
			} else {
				if (clockwise) {
					currentRotation += rotDelta;
					if (currentRotation >= 360f) {
						currentRotation -= 360f;
					}
				} else {
					currentRotation -= rotDelta;
					if (currentRotation < 0f) {
						currentRotation += 360f;
					}
				}
			}

			transform.eulerAngles = new Vector3(0f, currentRotation, 0f);
			break;
		
		case RocketState.FirePause:
			firePauseTimer -= Time.deltaTime;
			if (firePauseTimer <= 0f) {
				state = RocketState.RotateToBase;
				targetRotation = baseRotation;
			}
			break;

		default:
		//case RocketState.Waiting:
			break;
		}
	}

	void Fire() {
		if (bolt != null && boltLaunchPoint != null) {
			Instantiate(bolt, boltLaunchPoint.position, boltLaunchPoint.rotation);
		}
	}

	public void SetTargetAngle(float targetAngle) {
		targetRotation = targetAngle;
		state = RocketState.RotateToTarget;
	}
}
