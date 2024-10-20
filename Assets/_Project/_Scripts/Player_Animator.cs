using KVDW;
using UnityEngine;

public class Player_Animator : MonoBehaviour
{
    [SerializeField] private Player _player;
    private Animator _anim;
    private float _lockedTill;
    private bool _transitioning;
    private SimpleStateMachine<AnimationState> _animationState;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _anim.speed = 1f;

        InitAnimationStates();  // Initialize the state machine
    }

    private void Update()
    {
        if (Time.time < _lockedTill) return; // If the animation is locked, don't change state

        // Transition to LeftStart first, then Left after LeftStart finishes
        if (LeftCondition())
        {
            if (_animationState.State != AnimationState.Left && _animationState.State != AnimationState.LeftStart)
            {
                _animationState.SetState(AnimationState.LeftStart);
                return;
            }
        }

        // Transition to RightStart first, then Right after RightStart finishes
        if (RightCondition())
        {
            if (_animationState.State != AnimationState.Right && _animationState.State != AnimationState.RightStart)
            {
                _animationState.SetState(AnimationState.RightStart);
                return;
            }
        }

        // Check if LeftStart or RightStart animations are playing and transition to Left or Right
        var stateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (_animationState.State == AnimationState.LeftStart && stateInfo.normalizedTime >= 1f)
        {
            _animationState.SetState(AnimationState.Left);
            return;
        }

        if (_animationState.State == AnimationState.RightStart && stateInfo.normalizedTime >= 1f)
        {
            _animationState.SetState(AnimationState.Right);
            return;
        }

        // Default to Idle if no movement input
        _animationState.SetState(AnimationState.Idle);
    }

    void InitAnimationStates()
    {
        _animationState = new SimpleStateMachine<AnimationState>()
    {
        new SimpleState<AnimationState>()
        {
            name = AnimationState.Idle,
            onEnter = (AnimationState previous) =>
            {
                _anim.CrossFade(Idle, 0, 0);
            }
        },
        new SimpleState<AnimationState>()
        {
            name = AnimationState.Left,
            onEnter = (AnimationState previous) =>
            {
                if (previous == AnimationState.Idle || previous == AnimationState.RightStart)
                {
                    TransitionLock(11);
                }
                _anim.CrossFade(Left, 0, 0);
            }
        },
        new SimpleState<AnimationState>()
        {
            name = AnimationState.LeftStart,
            onEnter = (AnimationState previous) =>
            {
                _anim.CrossFade(LeftStart, 0, 0);
                TransitionLock(11);  // Lock during the LeftStart animation
            }
        },
        new SimpleState<AnimationState>()
        {
            name = AnimationState.Right,
            onEnter = (AnimationState previous) =>
            {
                if (previous == AnimationState.Idle || previous == AnimationState.LeftStart)
                {
                    TransitionLock(11);
                }
                _anim.CrossFade(Right, 0, 0);
            }
        },
        new SimpleState<AnimationState>()
        {
            name = AnimationState.RightStart,
            onEnter = (AnimationState previous) =>
            {
                _anim.CrossFade(RightStart, 0, 0);
                TransitionLock(11);  // Lock during the RightStart animation
            }
        },
        new SimpleState<AnimationState>()
        {
            name = AnimationState.Bump,
            onEnter = (AnimationState previous) =>
            {
                _anim.CrossFade(Bump, 0, 0);
                TransitionLock(11);  // Lock during the Bump animation
            }
        },
        new SimpleState<AnimationState>()
        {
            name = AnimationState.Catch,
            onEnter = (AnimationState previous) =>
            {
                _anim.CrossFade(Catch, 0, 0);
                TransitionLock(24);  // Lock during the Catch animation
            }
        }
        };
    }


    private void TransitionLock(int frames)
    {
        float animLength = (frames / 24f) / _anim.speed;
        _lockedTill = Time.time + animLength;
    }

    bool LeftCondition()
    {
        return _player.MoveInput[0] > 0 && _player.MoveInput[0] > _player.MoveInput[1];
    }

    bool RightCondition()
    {
        return _player.MoveInput[1] > 0 && _player.MoveInput[1] > _player.MoveInput[0];
    }

    private int Idle = Animator.StringToHash("Idle");
    private int Left = Animator.StringToHash("Left");
    private int LeftStart = Animator.StringToHash("LeftStart");
    private int Right = Animator.StringToHash("Right");
    private int RightStart = Animator.StringToHash("RightStart");
    private int Bump = Animator.StringToHash("Bump");
    private int Catch = Animator.StringToHash("Catch");
}

public enum AnimationState
{
    Idle,
    Left,
    LeftStart,
    Right,
    RightStart,
    Bump,
    Catch
}


//using KVDW;
//using UnityEngine;

//public class Player_Animator : MonoBehaviour
//{
//    [SerializeField] private Player _player;
//    private Animator _anim;
//    private float _lockedTill;
//    private bool _transitioning;
//    private SimpleStateMachine<AnimationState> _animationState;

//    private void Awake()
//    {
//        _anim = GetComponent<Animator>();
//        _anim.speed = 1f;

//        //var stateInfo = _anim.GetCurrentAnimatorStateInfo(0);

//        //if(stateInfo.normalizedTime > 1)
//        //{
//        //    _anim.Play(Idle, 0, 0);
//        //}
//    }

//    private void Update()
//    {
//        if (LeftCondition())
//        {


//            _animationState.SetState(AnimationState.Left);
//        }

//        if(RightCondition()) _animationState.SetState(AnimationState.Right);

//       if(_animationState.State == AnimationState.LeftStart || _animationState.State == AnimationState.RightStart)
//       {
//            var stateInfo = _anim.GetCurrentAnimatorStateInfo(0);

//            if (stateInfo.normalizedTime <= 1)
//            {
//                //_anim.Play(Idle, 0, 0);
//                return;
//            }
//        }

//        _animationState.SetState(AnimationState.Idle);
//    }

//    //private void OnEnable()
//    //{
//    //    _player.OnBallHit += OnPlayerOnBallHit;
//    //    _player.OnBallCatch += OnPlayerOnBallCatch;
//    //}

//    void InitAnimationStates()
//    {
//        _animationState = new SimpleStateMachine<AnimationState>()
//        {
//            new SimpleState<AnimationState>()
//            {
//                name = AnimationState.Idle,
//                onEnter = (AnimationState previous) =>
//                {
//                    _anim.CrossFade(Idle, 0, 0);
//                }
//            },
//            new SimpleState<AnimationState>()
//            {
//                name = AnimationState.Left,
//                onEnter = (AnimationState previous) =>
//                {
//                    if(previous == AnimationState.Idle || previous == AnimationState.RightStart) TransitionLock(11);
//                    //_anim.CrossFade(Left, 0, 0);
//                }
//            },
//            new SimpleState<AnimationState>()
//            {
//                name = AnimationState.LeftStart,
//                onEnter = (AnimationState previous) =>
//                {
//                    _anim.CrossFade(LeftStart, 0, 0);
//                }
//            },
//            new SimpleState<AnimationState>()
//            {
//                name = AnimationState.Right,
//                onEnter = (AnimationState previous) =>
//                {
//                    _anim.CrossFade(Right, 0, 0);
//                }
//            },
//            new SimpleState<AnimationState>()
//            {
//                name = AnimationState.RightStart,
//                onEnter = (AnimationState previous) =>
//                {
//                    _anim.CrossFade(RightStart, 0, 0);
//                }
//            },
//            new SimpleState<AnimationState>()
//            {
//                name = AnimationState.Bump,
//                onEnter = (AnimationState previous) =>
//                {
//                    _anim.CrossFade(Bump, 0, 0);
//                }
//            },
//            new SimpleState<AnimationState>()
//            {
//                name = AnimationState.Catch,
//                onEnter = (AnimationState previous) =>
//                {
//                    _anim.CrossFade(Catch, 0, 0);
//                }
//            }
//        };
//    }

//    //AnimationState LockedState(AnimationState state, int frames)
//    //{
//    //    float animLength = (frames / 24f) / _anim.speed;
//    //    _lockedTill = Time.time + animLength;
//    //    return state;
//    //}

//    void TransitionLock(int frames)
//    {
//        float animLength = (frames / 24f) / _anim.speed;
//        _lockedTill = Time.time + animLength;
//    }

//    int LockDuration(int state, int frames)
//    {
//        float animLength = (frames / 24f) / _anim.speed;
//        _lockedTill = Time.time + animLength;
//        return state;
//    }

//    bool LeftCondition()
//    {
//        return _player.MoveInput[0] > 0 && _player.MoveInput[0] > _player.MoveInput[1];
//    }
//    bool RightCondition()
//    {
//        return _player.MoveInput[1] > 0 && _player.MoveInput[1] > _player.MoveInput[0];
//    }

//    private int Idle = Animator.StringToHash("Idle");
//    private int Left = Animator.StringToHash("Left");
//    private int LeftStart = Animator.StringToHash("LeftStart");
//    private int Right = Animator.StringToHash("Right");
//    private int RightStart = Animator.StringToHash("RightStart");
//    private int Bump = Animator.StringToHash("Bump");
//    private int Catch = Animator.StringToHash("Catch");
//}


////#region Cached Properties


////    #endregion
////}

//public enum AnimationState
//{
//    Idle,
//    Left,
//    LeftStart,
//    Right,
//    RightStart,
//    Bump,
//    Catch
//}

