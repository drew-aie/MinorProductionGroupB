using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CopSeekBehavior : MonoBehaviour
{
    [SerializeField, Tooltip("What the cop will be chasing. (The Player)")]
    private GameObject _target;

    private NavMeshAgent _cop;
    private float _angularSpeed;
    private float _currentAngularVelocity;

    private Vector3 _velocity = Vector3.zero;
    private Vector3 _lastFacing;

    // Start is called before the first frame update
    void Awake()
    {
        _cop = GetComponent<NavMeshAgent>();
        _cop.updateRotation = false;
        _angularSpeed = _cop.angularSpeed;
    }

    // FixedUpdate is called once per fixed framerate frame
    void Update()
    {
        if (!_cop.enabled)
            return;

        _cop.destination = _target.transform.position;

        gameObject.transform.LookAt(_cop.transform.position + _cop.velocity);

        //Smoothing agent movement to prevent jittering
        _cop.transform.position = Vector3.SmoothDamp(_cop.transform.position, _cop.nextPosition, ref _velocity, 0.05f);
    }

    private void FixedUpdate()
    {
        //Checking if agent is rotating
        if (RotationCheck())
            //Scaling agent's angular speed down by it's speed value if so
            _cop.angularSpeed = MapValue(0, _cop.speed, _cop.angularSpeed, 0, 1);
        else
            //Resetting angular speed if not
            _cop.angularSpeed = _angularSpeed;
    }

    //Checks if the agent is currently rotating
    private bool RotationCheck()
    {
        Vector3 currentFacing = _cop.transform.forward;

        //Storing how many degrees the agent is rotating per second
        _currentAngularVelocity = Vector3.Angle(currentFacing, _lastFacing) / Time.deltaTime;

        _lastFacing = currentFacing;

        //Checking if degrees rotated per second is greater than 1
        bool agentRotating = _currentAngularVelocity > 1f;

        //Returning true or false if so
        if (agentRotating)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Returns a number between two values/ranges.
    /// </summary>
    /// <param name="inputMin">The lowest number of the range input.</param>
    /// <param name="inputMax">The largest number of the range input.</param>
    /// <param name="outputMin">The lowest number of the range output</param>
    /// <param name="outputMax">The largest number of the range output</param>
    /// <param name="value">Input value.</param>
    /// <returns>The number between the entered ranges.</returns>
    private float MapValue(float inputMin, float inputMax, float outputMin, float outputMax, float value)
    {
        return outputMin + ((outputMax - outputMin) / (inputMax - inputMin)) * (value - inputMin);
    }
}
