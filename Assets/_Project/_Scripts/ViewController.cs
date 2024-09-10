using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour
{
    private Button _startButton, _restartButton;
    private Canvas _menuCanvas, _gameplayCanvas, _gameOverCanvas;
    private TextMeshProUGUI _p1ScoreText, _p2ScoreText, _gameOverText;


    void Awake()
    {
        _startButton = GameObject.Find("StartButton").GetComponent<Button>();
        _restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
        
        _p1ScoreText = GameObject.Find("P1 Score").GetComponent<TextMeshProUGUI>();
        _p2ScoreText = GameObject.Find("P2 Score").GetComponent<TextMeshProUGUI>();
        _gameOverText = GameObject.Find("GameOverText").GetComponent<TextMeshProUGUI>();
        
        _menuCanvas = GameObject.Find("MenuCanvas").GetComponent<Canvas>();
        _gameplayCanvas = GameObject.Find("GameplayCanvas").GetComponent<Canvas>();
        _gameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();

    }

    void OnStartButtonClicked()
    {
        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    void OnRestartButtonClicked()
    {
        GameManager.Instance.UpdateGameState(GameState.MainMenu);
    }

    public void OnGameStateChanged(GameState newState)
    {
        _menuCanvas.gameObject.SetActive(false);
        _gameplayCanvas.gameObject.SetActive(false);
        _gameOverCanvas.gameObject.SetActive(false);

        switch (newState)
        {
            case GameState.MainMenu:
                _menuCanvas.gameObject.SetActive(true);
                break;
            case GameState.Playing:
                _gameplayCanvas.gameObject.SetActive(true);
                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                _gameOverCanvas.gameObject.SetActive(true);
                break;
        }
    }

    public void UpdateScore()
    {
        _p1ScoreText.text = GameManager.Instance.Score[0].ToString();
        _p2ScoreText.text = GameManager.Instance.Score[1].ToString();
    }

    public void SetWinner(int index)
    {
        //_gameOverText.gameObject.SetActive(true);
        if(index == 0)
        {
            _gameOverText.text = "PLAYER 1 WINS!";
        }
        else
        {
            _gameOverText.text = "PLAYER 2 WINS!";
        }
    }
}
