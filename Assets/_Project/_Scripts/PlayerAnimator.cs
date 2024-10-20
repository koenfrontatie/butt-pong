using UnityEngine;
using System;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player _player;
    private PlayerState _currentState = PlayerState.Idle;
    private Animator _anim;
    private float _unlockTime;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _anim.speed = 1f;
    }

    private void OnEnable()
    {
        _player.OnBallHit += TryPlayBumpAnimation;
        _player.OnBallCatch += TryPlayCatchAnimation;
    }

    private void Update()
    {
        if (Time.time <= _unlockTime) return;
        var newState = GetState();
        if (newState != _currentState)
        {
            PlayAnimation(newState);
        }
    }

    private PlayerState GetState()
    {
        if (_player.MoveInput[0] > 0 && _player.MoveInput[0] > _player.MoveInput[1]) // condition for left
        {
            return _currentState != PlayerState.Left && _currentState != PlayerState.LeftStart
                ? LockState(PlayerState.LeftStart, 11) 
                : PlayerState.Left;
        }
        else if (_player.MoveInput[1] > 0 && _player.MoveInput[1] > _player.MoveInput[0]) // condition for right
        {
            return _currentState != PlayerState.Right && _currentState != PlayerState.RightStart // checks if transition is needed
                ? LockState(PlayerState.RightStart, 11)
                : PlayerState.Right;
        }
        else
        {
            return PlayerState.Idle;
        }
    }

    private void TryPlayBumpAnimation()
    {
        if (_currentState == PlayerState.Catch) return;
        PlayAnimation(LockState(PlayerState.Bump, 11));
    }

    private void TryPlayCatchAnimation()
    {
        PlayAnimation(LockState(PlayerState.Catch, 24));
    }

    private PlayerState LockState(PlayerState state, int frames)
    {
        float animLength = (frames / 24f) / _anim.speed;
        _unlockTime = Time.time + animLength;
        return state;
    }

    private void PlayAnimation(PlayerState state)
    {
        _currentState = state;
        string animationName = Enum.GetName(typeof(PlayerState), state);
        _anim.CrossFade(animationName, 0, 0);
    }

    private void OnDisable()
    {
        _player.OnBallHit -= TryPlayBumpAnimation;
        _player.OnBallCatch -= TryPlayCatchAnimation;
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