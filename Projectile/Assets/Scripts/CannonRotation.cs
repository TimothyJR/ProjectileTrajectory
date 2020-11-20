using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonRotation : MonoBehaviour
{
	// Script used for show off
	// Moves the cannon object
	[SerializeField] private float rotationX = 0.0f;
	[SerializeField] private float rotationZ = 0.0f;
	[SerializeField] private GameObject swivelObject = null;
	[SerializeField] private GameObject upDownObject = null;

	// Fire cannonballs
	[SerializeField] private ProjectilePath pathProjection = null;
	[SerializeField] private GameObject cannonBall = null;
	[SerializeField] private bool fire = false;

	// Changes gravity
	[SerializeField] private Vector3 gravity = Vector3.zero;
	[SerializeField] private bool applyGravity = false;

	private void FixedUpdate()
	{
		swivelObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
		upDownObject.transform.localRotation = Quaternion.Euler(-rotationX, 0.0f, 0.0f);

		if (fire)
		{
			GameObject tempCannonBall = GameObject.Instantiate(cannonBall);
			tempCannonBall.transform.position = pathProjection.transform.position;
			tempCannonBall.transform.forward = pathProjection.transform.forward;
			tempCannonBall.GetComponent<Rigidbody>().velocity = pathProjection.StartVelocity;
			fire = false;
		}

		if (applyGravity)
		{
			Physics.gravity = gravity;
			applyGravity = false;
		}
	}
}
