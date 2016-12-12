using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public Vector3 force;
	public int score = 0;
	public GameObject GatePrefab;
	public GameObject CubePrefab;

	bool isDead = false;
	Vector3 camPos;
	GUIStyle scoreStyle = new GUIStyle();
	GUIStyle hintStyle = new GUIStyle();

	const string YOUR_SCORE_LABEL = "Score: ";
	bool showHint = true;

	// Use this for initialization
	void Start () {
		scoreStyle.normal.textColor = Color.white;
		hintStyle.normal.textColor = Color.white;
		hintStyle.alignment = TextAnchor.UpperCenter;

		float halfScreenWidthInUnits = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0)).x - 2f;
		Vector3 deltaPos = Vector3.up * 10f;
		Vector3 prevPos = Vector3.zero;	
		Vector3 curPos = Vector3.zero;	

		for (int i = 1; i < 100; i++) {
			curPos = prevPos + deltaPos;
			curPos = new Vector3(Random.Range(-halfScreenWidthInUnits, halfScreenWidthInUnits), curPos.y, 0);
			Instantiate (GatePrefab, curPos, Quaternion.identity);

			Instantiate (CubePrefab, new Vector3(curPos.x + Random.Range(-2f, 2f), curPos.y + Random.Range(2f, 4f)), Quaternion.identity);
			Instantiate (CubePrefab, new Vector3(curPos.x + Random.Range(-2f, 2f), curPos.y - Random.Range(2f, 4f)), Quaternion.identity);
			prevPos = new Vector3 (0, curPos.y);
		}
	}
	
	// Update is called once per frame
	void Update () {
		showHint = false;
		if (isDead) {
			showHint = true;
			if (Input.GetMouseButtonDown (0)) {
				Application.LoadLevel (Application.loadedLevel);
			}
			return;
		}

		Vector3 curPos = Camera.main.WorldToScreenPoint (transform.position);

		if (curPos.x > Screen.width - 10 || curPos.x < 10) {
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (
				Mathf.Abs(GetComponent<Rigidbody2D> ().velocity.x) * ((curPos.x < 10) ? 1: -1),
				GetComponent<Rigidbody2D> ().velocity.y
			);
		}

		if (Input.GetMouseButtonDown (0)) {
			if (GetComponent<Rigidbody2D> ().isKinematic) {
				GetComponent<Rigidbody2D> ().isKinematic = false;
			}

			GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
			GetComponent<Rigidbody2D> ().angularVelocity = 0f;

			if (Input.mousePosition.x > Screen.width / 2) {
				GetComponent<Rigidbody2D> ().AddForce (force, ForceMode2D.Impulse);
			} else {
				GetComponent<Rigidbody2D> ().AddForce (new Vector3(-force.x, force.y), ForceMode2D.Impulse);
			}
		}

		if (curPos.y < 10) {
			Die ();
		}

		MoveCamera ();
	}

	void Die () {
		isDead = true;
		GetComponent<Collider2D> ().enabled = false;
		Debug.Log ("You died!");
	}

	void OnCollisionEnter2D (Collision2D col) {
		Die ();
	}

	void OnTriggerEnter2D(Collider2D coll) {
		coll.enabled = false;
		++score;
		Debug.Log (this.score);
	}

	void OnGUI() { 
		GUI.Label(new Rect(10, 10, 100, 20), YOUR_SCORE_LABEL + score.ToString(), scoreStyle);
		if (showHint) {
			GUI.Label(new Rect((Screen.width / 4), Screen.height - 75f, 100, 20), "YOU LOST!", hintStyle);
			GUI.Label(new Rect((Screen.width / 4), Screen.height - 50f, 100, 20), "TAP TO START", hintStyle);
		}
	}

	void MoveCamera() {
		camPos = Camera.main.transform.position;

		if (transform.position.y > camPos.y) {
			Camera.main.transform.position = new Vector3 (camPos.x, transform.position.y, camPos.z);
		}
	}
}
