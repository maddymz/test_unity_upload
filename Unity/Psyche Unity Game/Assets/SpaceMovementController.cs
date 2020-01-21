﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpaceMovementController : MonoBehaviour
{
	public GameObject Model;
	public float MaxSpeed;
	public float ThrottleCoefficient;

	Slider ThrottleSlider;
	float currentThrottle;
	Vector2 speed;
	Vector2 direction;
	Vector2 playerPos;
	Rigidbody2D rb;

	private void Awake()
	{
		MaxSpeed = 12;
		ThrottleCoefficient = 5;
		currentThrottle = 0;
		
		// Enable Model's Collider
		var playerCollider = Model.GetComponentInChildren<Collider>();
		playerCollider.enabled = true;
	}

	// Start is called before the first frame update
	void Start()
	{
		// Get the Rigid Body of the Player
		rb = this.GetComponent<Rigidbody2D>();

		// Get the current Speed of the Player as a Vector2
		speed = rb.velocity;

		// Get the Throttle Element of the UI
		getThrottleElement();
	}

	// Update is called once per frame
	void Update()
	{
		currentThrottle = ThrottleSlider.value;
		setDirection();
	}

	void FixedUpdate()
	{
		// Get the current Speed of the Player as a Vector2
		speed = rb.velocity;

		moveShip(direction, currentThrottle*ThrottleCoefficient);

		speedLimit();
	}

	void moveShip(Vector2 direction, float force)
	{
		rb.AddForce(direction * force);
	}

	public void setDirection()
	{
		//If we're NOT over UI we can consider it wanting to change the direction.
		if (Input.touchCount > 0)
		{
			if (!IsPointerOverUIObject())
			{
				Touch touch = Input.GetTouch(0);
				Vector3 mousePos = Input.mousePosition;
				mousePos = Camera.main.ScreenToWorldPoint(touch.position);
				direction[0] = mousePos.x - transform.position.x;
				direction[1] = mousePos.y - transform.position.y;
				direction = direction.normalized;
				Vector3 rotVec = new Vector3(direction[0], direction[1], 0);
				Model.transform.rotation = Quaternion.LookRotation(rotVec);
			}
		}
		// If we're seeing mouse input
		else if (Input.GetMouseButton(0))
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				Vector3 mousePos = Input.mousePosition;
				mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				direction[0] = mousePos.x - transform.position.x;
				direction[1] = mousePos.y - transform.position.y;
				direction = direction.normalized;
				Vector3 rotVec = new Vector3(direction[0], direction[1], 0);
				Model.transform.rotation = Quaternion.LookRotation(rotVec);
			}
		}
	}

	private void speedLimit()
	{
		if (speed.magnitude > MaxSpeed) {
			// +1 adjusting for any possible force also applied during current force update
			rb.AddForce(-1 * speed.normalized * (speed.magnitude - MaxSpeed + ThrottleCoefficient));
		}
	}

	private void getThrottleElement()
	{
		// Check the status of the throttle
		GameObject temp = GameObject.Find("Slider_Throttle");
		if (temp != null)
		{
			// Get the Slider Component
			ThrottleSlider = temp.GetComponent<Slider>();

			// If a Slider Component was not found on the GameObject.
			if (ThrottleSlider == null)
				Debug.LogError("[" + temp.name + "] - Does not contain a Slider Component!");
		}
	}

	private bool IsPointerOverUIObject()
	{
		PointerEventData eventDataPos = new PointerEventData(EventSystem.current);
		eventDataPos.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataPos, results);
		return results.Count > 0;
	}
}




