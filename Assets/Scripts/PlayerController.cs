using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{
    public float runSpeed = 4f;
    public float airWalkSpeed = 4f;
    public float airWallSpeed = 4f;
    public float dashSpeed = 6f;
    public float jumpImpulse = 7f;
    public float coolDownAttack = 0.5f;
    public float coolDownDash = 2f;
    private float lastAttack;
    private float lastDash;

    Vector2 moveInput;
    TouchingDirections touchingDirections;

    public float CurrentSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsRunning && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {
                        return runSpeed;
                    }
                    else
                    {
                        return airWalkSpeed;
                    }
                }
                else
                    return 0;
            }
            else
                return 0;

        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get
        {
            return _isFacingRight;
        }
        private set
        {
            if (_isFacingRight != value)
            {
                // Flip in opposite direction
                transform.localScale *= new Vector2(-1, 1);
                _isFacingRight = value;
            }
        }
    }

    private bool _isRunning = false;
    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        private set
        {
            _isRunning = value;
            animator.SetBool("isRunning", value);
        }
    }

    public bool IsDashing
    {
        get
        {
            return animator.GetBool("isDashing");
        }
    }

    public bool IsWallJumping
    {
        get
        {
            return animator.GetBool("isWallJumping");
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool("canMove");
        }
    }

    public bool CanDoubleJump
    {
        get
        {
            return animator.GetBool("canDoubleJump");
        }
        set
        {
            animator.SetBool("canDoubleJump", value);
        }
    }

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        lastAttack = -10f;
        lastDash = -10f;
    }

    private void FixedUpdate()
    {
        if (!IsWallJumping && !IsDashing)
            rb.velocity = new Vector2(moveInput.x * CurrentSpeed, rb.velocity.y);

        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsRunning = moveInput != Vector2.zero;

        SetFacingDirection(moveInput);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            // Face Right
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            // Face Left
            IsFacingRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // @TODO : check if he's alive also.
        if (context.started && CanMove)
        {
            if (touchingDirections.IsGrounded)
            {
                animator.SetTrigger("Jump");
                rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            }
            else if (touchingDirections.IsOnWall)
            {
                // @TODO : jump direction based on wall direction ?
                animator.SetTrigger("WallJump");
                rb.velocity = new Vector2(-(moveInput.x * airWallSpeed), jumpImpulse);
            }
            else if (CanDoubleJump)
            {
                animator.SetTrigger("DoubleJump");
                rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // @TODO: Throw Arrow + Hit vraiment un ennemi/collision
        if (context.started)
        {
            // Check cooldown
            if (Time.time - lastAttack < coolDownAttack)
            {
                return;
            }

            lastAttack = Time.time;
            animator.SetTrigger("Attack");
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove && !(Time.time - lastDash < coolDownDash))
        {
            lastDash = Time.time;
            animator.SetTrigger("Dash");
            rb.velocity = new Vector2(moveInput.x * dashSpeed, rb.velocity.y);
        }
    }
}
