using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private static bool skipStartScreen = false;

    [Header("References")]
    public RunnerPlayer player;
    public ZombieFollower zombie;

    [Header("UI")]
    public Text scoreText;
    public Text coinText;
    public GameObject gameOverPanel;
    public Text finalScoreText;

    [Header("Start Screen")]
    public GameObject startPanel;
    public Text startHighScoreText;

    private float distance;
    private int coins;
    private bool gameOver;
    private bool started;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (skipStartScreen)
        {
            skipStartScreen = false;
            started = true;
            if (startPanel != null)
            {
                startPanel.SetActive(false);
            }
        }
        else
        {
            started = false;
            Time.timeScale = 0f;
            if (startPanel != null)
            {
                startPanel.SetActive(true);
            }
            if (startHighScoreText != null)
            {
                int best = PlayerPrefs.GetInt("HighScore", 0);
                startHighScoreText.text = "High Score: " + best;
            }
        }

        UpdateUI();
    }

    bool BeginPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        if (k == null) return false;
        return k.enterKey.wasPressedThisFrame || k.spaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);
#endif
    }

    bool RestartPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        if (k == null) return false;
        return k.rKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }

    void Update()
    {
        if (!started)
        {
            if (BeginPressed())
            {
                StartGame();
            }
            return;
        }

        if (gameOver)
        {
            if (RestartPressed())
            {
                RestartGame();
            }
            return;
        }

        if (player != null && player.isAlive)
        {
            distance = player.transform.position.z;
            UpdateUI();
        }
    }

    public void StartGame()
    {
        started = true;
        Time.timeScale = 1f;
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        UpdateUI();
    }

    public void GameOver()
    {
        if (gameOver) return;
        gameOver = true;

        int score = CurrentScore();
        int best = PlayerPrefs.GetInt("HighScore", 0);
        bool newBest = score > best;

        if (newBest)
        {
            best = score;
            PlayerPrefs.SetInt("HighScore", best);
            PlayerPrefs.Save();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            string text = "Score: " + score;
            text += "\nCoins: " + coins;
            text += "\nBest: " + best;
            if (newBest) text += "  NEW!";
            finalScoreText.text = text;
        }
    }

    int CurrentScore()
    {
        return Mathf.FloorToInt(distance) + coins * 10;
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + CurrentScore();
        }
        if (coinText != null)
        {
            coinText.text = "Coins: " + coins;
        }
    }

    public void RestartGame()
    {
        skipStartScreen = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}