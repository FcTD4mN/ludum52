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

    private float lastDash;

    // References to GameObject or Scripts
    GameObject aimAssist;
    GameObject rotationPoint;
    // WeaponLauncher bow;
    public HasWeapon mWeaponComponent;

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
        mWeaponComponent = GetComponent<HasWeapon>();
        aimAssist = GameObject.Find("AimAssist");
        rotationPoint = GameObject.Find("RotationPoint");
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
            else if (touchingDirections.IsOnWall && GameManager.mInstance.IsUnlockAction(UnlockableAction.WallJump))
            {
                // @TODO : jump direction based on wall direction ?
                animator.SetTrigger("WallJump");
                rb.velocity = new Vector2(-(moveInput.x * mStats.GetFinalStat(eStatsNames.AirWallSpeed)),
                                            mStats.GetFinalStat(eStatsNames.JumpImpulse));
                CanDoubleJump = true;
            }
            else if (CanDoubleJump && GameManager.mInstance.IsUnlockAction(UnlockableAction.DoubleJump))
            {
                animator.SetTrigger("DoubleJump");
                rb.velocity = new Vector2(rb.velocity.x, mStats.GetFinalStat(eStatsNames.JumpImpulse));
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (touchingDirections.IsGrounded)
        {
            if (context.started)
            {
                // Left click
                // Swap direction if aiming to player's opposite side
                if ((IsFacingRight && !IsAimingRight) || (!IsFacingRight && IsAimingRight))
                {
                    SwapDirection();
                }
                mWeaponComponent.mWeapon.mAutoFire = true;
                animator.SetTrigger("HoldAttack");
            }
            else if (context.canceled)
            {
                // Release click
                mWeaponComponent.mWeapon.mAutoFire = false;
                animator.SetTrigger("Attack");
            }
        }
        else
        {
            if (context.started)
            {
                // Swap direction if aiming to player's opposite side
                if ((IsFacingRight && !IsAimingRight) || (!IsFacingRight && IsAimingRight))
                {
                    SwapDirection();
                }
                mWeaponComponent.mWeapon.mAutoFire = true;
                animator.SetTrigger("HoldAttack");
            }
            else if (context.canceled)
            {
                // Release click
                mWeaponComponent.mWeapon.mAutoFire = false;
                animator.SetTrigger("Attack");
            }
        }
    }

    public void OnSwitchArrow(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // No longer have we different types of arrow, it's all related to building you have activated
        }
    }

    public void OnThrowBomb(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Same here, unless we allow primary and secondary attacks
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        bool cooledDown = (Time.time - lastDash < mStats.GetFinalStat(eStatsNames.CoolDownDash));
        bool unlocked = GameManager.mInstance.IsUnlockAction(UnlockableAction.Dash);
        if (context.started && CanMove && unlocked && !cooledDown)
        {
            lastDash = Time.time;
            animator.SetTrigger("Dash");
            rb.velocity = new Vector2(moveInput.x * mStats.GetFinalStat(eStatsNames.DashSpeed) * 3f, rb.velocity.y);
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
        baseValues.mStatValues[eStatsNames.CoolDownDash.ToString()] = 2f;
        baseValues.mStatValues[eStatsNames.Health.ToString()] = 100f;
        baseValues.mStatValues[eStatsNames.MaxHealth.ToString()] = 100f;

        mStats.SetBaseStats(baseValues);
    }
}
