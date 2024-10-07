﻿using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(InputManager))]
public class PlayerMovement : MonoBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	[SerializeField]
	public float MoveSpeed = 4.0f;
	[Tooltip("Sprint speed of the character in m/s")]
	public float SprintSpeed = 6.0f;
	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.1f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.5f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayers;

	[Header("Footstep and Landing Audio")]
	[Tooltip("Footstep sounds to play while walking or running")]
	[SerializeField] private AudioClip[] m_footstepClips;

	[Tooltip("Landing sounds to play when we land")]
	[SerializeField] private AudioClip[] m_landingClips;
	private float _footstepTimer = 0.0f;
	private float _baseFootstepInterval = 0.5f;
	private float _footstepInterval = 0.5f;

	// player
	private float _speed;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;
	private bool _wasGrounded = true;

	private CharacterController _controller;
	private InputManager _input;

	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
		_input = GetComponent<InputManager>();
	}

	private void Start()
	{
		// reset our timeouts on start
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;
	}

	private void Update()
	{
		JumpAndGravity();
		GroundedCheck();
		Move();
		HandleLandingSound();
	}

	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
	}

	private void Move()
	{
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = _input.IsSprinting() ? SprintSpeed : MoveSpeed;

		_footstepInterval = _input.IsSprinting() ? _baseFootstepInterval * (MoveSpeed / SprintSpeed) : _baseFootstepInterval;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (_input.GetMove() == Vector2.zero) targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = _input.GetAnalogMovement() ? _input.GetMove().magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		// normalise input direction
		Vector3 inputDirection = new Vector3(_input.GetMove().x, 0.0f, _input.GetMove().y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (_input.GetMove() != Vector2.zero)
		{
			// move
			inputDirection = transform.right * _input.GetMove().x + transform.forward * _input.GetMove().y;
		}

		// move the player
		Vector3 finalMovement = (inputDirection.normalized * _speed + new Vector3(0, _verticalVelocity, 0)) * Time.deltaTime;
		_controller.Move(finalMovement);

		HandleFootstepSounds(currentHorizontalSpeed, targetSpeed);
	}

	private void JumpAndGravity()
	{
		if (Grounded)
		{
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;

			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			// Jump
			if (_input.IsJumpping() && _jumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				_jumpTimeoutDelta = JumpTimeout;
			}

			// jump timeout
			if (_jumpTimeoutDelta >= 0.0f)
			{
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			// reset the jump timeout timer
			_jumpTimeoutDelta = JumpTimeout;

			// fall timeout
			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}

			// if we are not grounded, do not jump
			_input.SetJump(false);

			// _landSoundPlayed = false; // Remove this line
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}


	private void HandleFootstepSounds(float currentSpeed, float targetSpeed)
	{
		if (Grounded && currentSpeed > 0.1f)
		{
			_footstepTimer += Time.deltaTime;

			if (_footstepTimer >= _footstepInterval)
			{
				AudioManager.Instance.PlayOneShot(m_footstepClips);
				_footstepTimer = 0.0f;
			}
		}
		else
		{
			_footstepTimer = 0.0f;
		}
	}


	private void HandleLandingSound()
	{
		if (!_wasGrounded && Grounded)
		{
			// Play landing sound
			AudioManager.Instance.PlayOneShot(m_landingClips);
		}
		_wasGrounded = Grounded;
	}
}