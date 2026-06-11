using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        if (mainMixer != null && volumeSlider != null)
        {
            float currentVol;
            if (mainMixer.GetFloat("MasterVolume", out currentVol))
            {
                volumeSlider.value = currentVol;
            }
        }
    }

    public void PlayGame()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.ResetData();
        }

        SceneManager.LoadScene(1);
    }

    public void OpenOptions()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        if (mainMixer != null)
        {
            mainMixer.SetFloat("MasterVolume", volume);
        }
    }
}