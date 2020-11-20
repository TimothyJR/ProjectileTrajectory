using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePath : MonoBehaviour
{
	/// <summary>
	/// The velocity the projectile is expected to be sent at
	/// The value set in the inspector is not used if useForwardDirection == true
	/// </summary>
	[SerializeField] 
	private Vector3 startVelocity = Vector3.up;
	public Vector3 StartVelocity { get { return startVelocity; } }

	/// <summary>
	/// Whether the trajectory should be calculated using the start velocity from the inspector
	/// or the forward vector of the transform
	/// </summary>
	[SerializeField]
	private bool useForwardDirection = false;

	/// <summary>
	/// The force to apply in calculations if using the forward vector
	/// </summary>
	[SerializeField, Tooltip("Only used when using the forward direction")]
	private float force = 1.0f;

	/// <summary>
	/// How many points we want to predict along the trajectory
	/// More points = more accuracy, less performance
	/// Should be set in conjunction with the maxTime to get reasonable results without costing too much performance
	/// </summary>
	[SerializeField] 
	private uint traceResolution = 5;

	/// <summary>
	/// How long the projectile will travel
	/// </summary>
	[SerializeField]
	private float maxTime = 1.0f;

	/// <summary>
	/// The line renderer used to display the trajectory
	/// </summary>
	[SerializeField]
	private LineRenderer line;

	/// <summary>
	/// Object to indicate where the hit occurs
	/// </summary>
	[SerializeField]
	private GameObject hitMarker = null;

	/// <summary>
	/// The step amount based on the resolution and the max time
	/// </summary>
	private float raycastStep = 0.0f;

	/// <summary>
	/// Spawned version of the hit marker
	/// </summary>
	private GameObject spawnedHitMarker = null;

	private void Awake()
	{
		spawnedHitMarker = GameObject.Instantiate(hitMarker);
		// Set this object as the parent, so when this object is disabled, the marker is disabled too
		spawnedHitMarker.transform.parent = gameObject.transform;
	}

	// Update is called once per frame
	void Update()
	{
		float time = 0.0f;
		Vector3 lastPosition = transform.position;
		Vector3 nextPosition;
		RaycastHit hit;
		bool hitHappened = false;
		int index = 1;
		raycastStep = maxTime / traceResolution;
		line.positionCount = (int)traceResolution + 1;
		line.SetPosition(0, transform.position);

		// If using forward direction, set the starting velocity.
		if(useForwardDirection)
		{
			startVelocity = transform.forward * force;
		}

		// Calculate the path bit by bit until a collision or max time is reached
		while(!hitHappened && index < traceResolution + 1)
		{
			time = Mathf.Min(time + raycastStep, maxTime);
			nextPosition = GetPositionAtTime(transform.position, startVelocity, time, Physics.gravity);

			// Linecast from the current position to the next position to see if we hit
			if(Physics.Linecast(lastPosition, nextPosition, out hit))
			{
				// If a hit happens, we are done
				hitHappened = true;
				lastPosition = hit.point;
				spawnedHitMarker.transform.rotation = Quaternion.LookRotation(hit.normal);
			}
			else
			{
				// Get ready for next loop
				lastPosition = nextPosition;
			}

			// Update the line renderer
			line.SetPosition(index, lastPosition);
			index++;
		}

		// Make sure to get rid of any extra positions if we ended before max time
		line.positionCount = index;

		// Move our marker object to the proper position
		spawnedHitMarker.transform.position = line.GetPosition(index - 1);
	}

	/// <summary>
	/// Uses the physics equation xo + vo * t + (1/2) * a * t ^ 2 to calculate the position
	/// </summary>
	/// <param name="startPosition"></param>
	/// <param name="startVelocity"></param>
	/// <param name="time"></param>
	/// <param name="acceleration"></param>
	/// <returns></returns>
	Vector3 GetPositionAtTime(Vector3 startPosition, Vector3 startVelocity, float time, Vector3 acceleration)
	{
		return startPosition + startVelocity * time + acceleration * 0.5f * time * time;
	}
}
