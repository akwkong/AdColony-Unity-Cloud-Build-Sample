using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

	private GameController gameController;

	public GameObject explosionEffect;
	public float asteroidAngle;
	public float tumbleMin;
	public float tumbleMax;

	void Start() {
		GameObject gameControllerObj = GameObject.FindGameObjectWithTag(Constants.GameControllerTag);
		if (gameControllerObj != null) {
			gameController = gameControllerObj.GetComponent<GameController>();
		}
	}

	void Explode() {
		if (explosionEffect != null) {
			Instantiate(explosionEffect, transform.position, transform.rotation);
		}

		Asteroid asteroid = gameObject.GetComponent<Asteroid>();
		if (asteroid != null) {
			asteroid.Hide();
		}
	}

	public void Show() {
		gameObject.SetActive(true);

		float tumble = (Random.value * (tumbleMax - tumbleMin)) + tumbleMin;
		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void OnTapped() {
		GameObject rocketObject = GameObject.FindGameObjectWithTag("Rocket");
		if (rocketObject != null) {
			Rocket rocket = rocketObject.GetComponent<Rocket>();
			if (rocket != null) {
				rocket.SetTargetAngle(asteroidAngle);
			}
		}
	}

	public void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);

		Asteroid asteroid = gameObject.GetComponent<Asteroid>();
		if (asteroid != null) {
			asteroid.Explode();
			if (gameController != null) {
				gameController.PerformAdColonyAction(asteroid);
			}
		}
	}
}
