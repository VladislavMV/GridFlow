using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;

    [Header("Audio")]
    public AudioMixer mainMixer;
    public Slider volumeSlider;

    [Header("Health UI")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Smooth Health Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    private float targetHealthValue;

    [Header("Weapon UI")]
    public Image weaponIcon;
    public TextMeshProUGUI ammoText;

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;

    private float elapsedTime;
    private bool isTimerRunning = true;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        float currentVol;
        mainMixer.GetFloat("MasterVolume", out currentVol);
        if (volumeSlider != null) volumeSlider.value = currentVol;

        Time.timeScale = 1f;

        if (healthSlider != null)
        {
            targetHealthValue = healthSlider.value;
        }

        if (GameDataManager.Instance != null)
        {
            elapsedTime = GameDataManager.Instance.savedTimer;
            UpdateTimerDisplay();
        }

        if (GameDataManager.Instance != null)
        {
            UpdateScoreDisplay(GameDataManager.Instance.currentScore);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }

        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealthValue, Time.deltaTime * smoothSpeed);
        }
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("MasterVolume", volume);
    }

    public void QuitGame()
    {
        Debug.Log("Вихід з гри...");
        Application.Quit();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer() => isTimerRunning = false;

    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Фінальний рахунок: " + GameDataManager.Instance.currentScore.ToString();
        }
    }

    public void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void RestartGame()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.ResetData();
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            targetHealthValue = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
        }
    }

    public void UpdateWeapon(Sprite icon, int currentAmmo, bool isMelee)
    {
        if (weaponIcon != null) weaponIcon.sprite = icon;

        if (isMelee) ammoText.text = " ";
        else ammoText.text = currentAmmo.ToString();
    }

    public void UpdateAmmoOnly(int currentAmmo)
    {
        ammoText.text = currentAmmo.ToString();
    }

    public void OnMusicSliderChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXSliderChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}