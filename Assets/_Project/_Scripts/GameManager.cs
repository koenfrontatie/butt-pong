using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State { get; private set; }

    private ViewController _viewController;
    private Ball _ball;

    public int[] Score = new int[2];

    public int ScoreToWin = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        _viewController = FindObjectOfType<ViewController>();
        _ball = FindObjectOfType<Ball>();
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                _viewController.OnGameStateChanged(newState);
                break;
            case GameState.Playing:
                _viewController.OnGameStateChanged(newState);
          
                Score[0] = 0;
                Score[1] = 0;
                _ball.OnGameplayStart();
                _viewController.UpdateScore();

                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                _viewController.OnGameStateChanged(newState);
                break;
        }

        State = newState;
    }

    public void OnPointScored(Vector2 inc)
    {
        Score[0] += (int)inc.x;
        Score[1] += (int)inc.y;

        _viewController.UpdateScore();

        if (Score[0] >= ScoreToWin)
        {
            _viewController.SetWinner(0);

            UpdateGameState(GameState.GameOver);

        }

        if (Score[1] >= ScoreToWin)
        {
            _viewController.SetWinner(1);

            UpdateGameState(GameState.GameOver);
        }
    }
}

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}
