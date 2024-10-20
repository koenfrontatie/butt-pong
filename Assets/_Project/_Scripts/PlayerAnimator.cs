using UnityEngine;
using System;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player _player;
    private Animator _anim;
    private float _lockedTill;

    private enum PlayerState
    {
        Idle,
        Left,
        LeftStart,
        Right,
        RightStart,
        Bump,
        Catch
    }

    private PlayerState _currentState = PlayerState.Idle;
    private PlayerState _lastMovementState = PlayerState.Idle;
    private bool _transitioning = true;

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
        if (Time.time >= _lockedTill)
        {
            var newState = GetState();
            if (newState != _currentState)
            {
                PlayAnimation(newState);
            }
        }
    }

    private PlayerState GetState()
    {
        PlayerState newState = _currentState;

        if (_player.MoveInput[0] > 0 && _player.MoveInput[0] > _player.MoveInput[1])
        {
            newState = HandleHorizontalMovement(PlayerState.Left, PlayerState.LeftStart);
        }
        else if (_player.MoveInput[1] > 0 && _player.MoveInput[1] > _player.MoveInput[0])
        {
            newState = HandleHorizontalMovement(PlayerState.Right, PlayerState.RightStart);
        }
        else if (_player.MoveInput[0] == 0 && _player.MoveInput[1] == 0)
        {
            newState = PlayerState.Idle;
        }

        CheckTransitioning(newState);
        return newState;
    }

    private PlayerState HandleHorizontalMovement(PlayerState moveState, PlayerState startState)
    {
        if (_transitioning)
        {
            _transitioning = false;
            return startState;
        }
        return moveState;
    }

    private PlayerState LockState(PlayerState state, int frames)
    {
        float animLength = (frames / 24f) / _anim.speed;
        _lockedTill = Time.time + animLength;
        return state;
    }

    private void CheckTransitioning(PlayerState newState)
    {
        _transitioning =
            _currentState == PlayerState.Idle && (newState == PlayerState.Left || newState == PlayerState.Right) ||
            (_currentState == PlayerState.Left && newState == PlayerState.Right) ||
            (_currentState == PlayerState.Right && newState == PlayerState.Left) ||
            _currentState == PlayerState.Catch;

        if (newState == PlayerState.Left || newState == PlayerState.Right)
        {
            _lastMovementState = newState;
        }
    }

    private void TryPlayBumpAnimation()
    {
        if (Time.time >= _lockedTill && _currentState != PlayerState.Catch)
        {
            PlayAnimation(LockState(PlayerState.Bump, 11));
        }
    }

    private void TryPlayCatchAnimation()
    {
        if (Time.time >= _lockedTill)
        {
            PlayAnimation(LockState(PlayerState.Catch, 24));
        }
    }

    private void PlayAnimation(PlayerState state)
    {
        _currentState = state;
        string animationName = Enum.GetName(typeof(PlayerState), state);
        _anim.CrossFade(animationName, 0, 0);
        Debug.Log(animationName);
    }

    private void OnDisable()
    {
        _player.OnBallHit -= TryPlayBumpAnimation;
        _player.OnBallCatch -= TryPlayCatchAnimation;
    }
}