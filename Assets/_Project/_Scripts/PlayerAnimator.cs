using UnityEngine;
using System;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;
    private PlayerState currentState = PlayerState.Idle;
    private Animator anim;
    private float unlockTime;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.speed = 1f;
    }
    
    private void OnEnable()
    {
        player.OnBallHit += TryPlayBumpAnimation;
        player.OnBallCatch += TryPlayCatchAnimation;
    }
    
    private void Update()
    {
        if (Time.time <= unlockTime) return;
        
        var newState = GetState();
        if (newState != currentState)
        {
            PlayAnimation(newState);
        }
    }
    
    private PlayerState GetState()
    {
        Vector2 moveInput = player.MoveInput;
        
        // Check left movement
        if (moveInput.x > moveInput.y && moveInput.x > 0.1f) // Added threshold
        {
            return currentState != PlayerState.Left && currentState != PlayerState.LeftStart
                ? LockState(PlayerState.LeftStart, 11)
                : PlayerState.Left;
        }
        // Check right movement
        else if (moveInput.y > moveInput.x && moveInput.y > 0.1f) // Added threshold
        {
            return currentState != PlayerState.Right && currentState != PlayerState.RightStart
                ? LockState(PlayerState.RightStart, 11)
                : PlayerState.Right;
        }
        // Idle state
        else
        {
            return PlayerState.Idle;
        }
    }
    
    private void TryPlayBumpAnimation()
    {
        if (currentState == PlayerState.Catch) return;
        PlayAnimation(LockState(PlayerState.Bump, 11));
    }
    
    private void TryPlayCatchAnimation()
    {
        PlayAnimation(LockState(PlayerState.Catch, 24));
    }
    
    private PlayerState LockState(PlayerState state, int frames)
    {
        float animLength = (frames / 24f) / anim.speed;
        unlockTime = Time.time + animLength;
        return state;
    }
    
    private void PlayAnimation(PlayerState state)
    {
        currentState = state;
        string animationName = Enum.GetName(typeof(PlayerState), state);
        anim.CrossFade(animationName, 0, 0);
    }
    
    private void OnDisable()
    {
        player.OnBallHit -= TryPlayBumpAnimation;
        player.OnBallCatch -= TryPlayCatchAnimation;
    }
    
    private enum PlayerState
    {
        Idle,
        LeftStart,
        Left,
        RightStart,
        Right,
        Bump,
        Catch
    }
}