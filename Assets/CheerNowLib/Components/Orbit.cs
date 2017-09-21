/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour {

	public Transform target;
	public float orbitXSpeed;
	public float orbitYSpeed;
	public float minYAngle = -20f;
	public float maxYAngle = 50f;
	public bool revertY;
	public Vector3 offset;

	private Vector3 _beginMousePos;
	private float _xAngle;
	private float _yAngle;
	private float _distance;

	void Awake(){
		transform.LookAt(target);
	}

	void Start(){
		_distance = Vector3.Distance(transform.position, target.position);
		RememberAngle();
		OrbitCamera(Vector3.one);
	}

	void RememberAngle(){
		var angle = transform.eulerAngles;
		_xAngle = angle.y;
		_yAngle = angle.x;
	}

	
	// Update is called once per frame
	void Update () {
		if (target == null) {
			Debug.Log("Target is not set");
			return;
		}

		if (Input.GetMouseButtonDown(0)) {
			_beginMousePos = Input.mousePosition;
		}
		else if (Input.GetMouseButton(0)) {
			var delta = Input.mousePosition - _beginMousePos;
			OrbitCamera(delta);
		}

		if (Input.GetMouseButtonUp(0)) {
			RememberAngle();
		}
	}

	void OrbitCamera(Vector3 delta){
		var x = _xAngle + delta.x * orbitXSpeed;
		var y = _yAngle - delta.y * orbitYSpeed * (revertY ? -1 : 1);

		y = ClampAngle(y, minYAngle, maxYAngle);

		var rot = Quaternion.Euler(y, x, 0);
		var negDistance = new Vector3(0, 0, -_distance);
		transform.position = rot * negDistance + target.position + offset;
		transform.rotation = rot;
	}

	float ClampAngle(float angle, float min, float max){
		if (angle < -180f) {
			angle += 360f;
		} else if (angle > 180f) {
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
