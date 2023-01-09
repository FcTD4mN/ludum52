using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using static cStatsDescriptor;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{
    // Char Stats
    public HasStats mStats;

    private float lastAttack;
    private float lastDash;

    // References to GameObject or Scripts
    GameObject aimAssist;
    GameObject rotationPoint;
    WeaponLauncher bow;

    [HideInInspector]
    public Interactable currentInteractable;

    // Movements and actions
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
                        return mStats.GetFinalStat(eStatsNames.RunSpeed);
                    }
                    else
                    {
                        return mStats.GetFinalStat(eStatsNames.AirWalkSpeed);
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

    public bool IsAimingRight
    {
        get
        {
            Vector2 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return (camPos.x >= gameObject.transform.position.x);
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

    public bool IsAlive
    {
        get
        {
            return animator.GetBool("isAlive");
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

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        bow = GetComponent<WeaponLauncher>();
        aimAssist = GameObject.Find("AimAssist");
        rotationPoint = GameObject.Find("RotationPoint");
        lastAttack = -10f;
        lastDash = -10f;

        // Stats
        mStats = GetComponent<HasStats>();
        BuildStats();
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

        if (IsAlive)
        {
            IsRunning = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
            IsRunning = false;

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

    private void SwapDirection()
    {
        IsFacingRight = !IsFacingRight;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // @TODO : check if he's alive also.
        if (context.started && CanMove)
        {
            if (touchingDirections.IsGrounded)
            {
                animator.SetTrigger("Jump");
                rb.velocity = new Vector2(rb.velocity.x, mStats.GetFinalStat(eStatsNames.JumpImpulse));
            }
            else if (touchingDirections.IsOnWall)
            {
                // @TODO : jump direction based on wall direction ?
                animator.SetTrigger("WallJump");
                rb.velocity = new Vector2(-(moveInput.x * mStats.GetFinalStat(eStatsNames.AirWallSpeed)),
                                            mStats.GetFinalStat(eStatsNames.JumpImpulse));
            }
            else if (CanDoubleJump)
            {
                animator.SetTrigger("DoubleJump");
                rb.velocity = new Vector2(rb.velocity.x, mStats.GetFinalStat(eStatsNames.JumpImpulse));
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // SwapDirection();
            if ((int)GameManager.mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Arrows) > 0)
            {
                // Check cooldown
                if (Time.time - lastAttack < mStats.GetFinalStat(eStatsNames.CoolDownAttack))
                {
                    return;
                }

                // Swap direction if aiming to player's opposite side
                if ((IsFacingRight && !IsAimingRight) || (!IsFacingRight && IsAimingRight))
                {
                    SwapDirection();
                }

                SpriteRenderer sr = aimAssist.GetComponent<SpriteRenderer>();
                bow.targetPos = sr.transform.position;

                lastAttack = Time.time;
                animator.SetTrigger("Attack");
            }
        }
    }

    public void OnThrowBomb(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if ((int)GameManager.mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Bombs) > 0)
            {
                bow.LaunchBomb(IsFacingRight ? true : false);
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove && !(Time.time - lastDash < mStats.GetFinalStat(eStatsNames.CoolDownDash)))
        {
            lastDash = Time.time;
            animator.SetTrigger("Dash");
            rb.velocity = new Vector2(moveInput.x * mStats.GetFinalStat(eStatsNames.DashSpeed), rb.velocity.y);
        }
    }

    public void OnTest(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Test si besoin 'T' keyboard
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.started && currentInteractable != null)
        {
            currentInteractable.Interact();
            currentInteractable.isActive = false;
        }
    }


    private void BuildStats()
    {
        cStatsDescriptor baseValues = new cStatsDescriptor();
        baseValues.mStatValues[eStatsNames.RunSpeed.ToString()] = 4f;
        baseValues.mStatValues[eStatsNames.AirWalkSpeed.ToString()] = 4f;
        baseValues.mStatValues[eStatsNames.AirWallSpeed.ToString()] = 4f;
        baseValues.mStatValues[eStatsNames.DashSpeed.ToString()] = 6f;
        baseValues.mStatValues[eStatsNames.JumpImpulse.ToString()] = 7f;
        baseValues.mStatValues[eStatsNames.CoolDownAttack.ToString()] = 0.5f;
        baseValues.mStatValues[eStatsNames.CoolDownDash.ToString()] = 2f;
        baseValues.mStatValues[eStatsNames.Health.ToString()] = 100f;
        baseValues.mStatValues[eStatsNames.MaxHealth.ToString()] = 100f;

        mStats.SetBaseStats(baseValues);
    }
}
