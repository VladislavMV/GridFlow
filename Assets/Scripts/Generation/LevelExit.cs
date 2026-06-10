using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private GameObject interactHint;
    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactHint != null) interactHint.SetActive(true);
            Debug.Log("Сходи: Натисніть E, щоб перейти далі");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactHint != null) interactHint.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            NextLevel();
        }
    }

    private void NextLevel()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health h = player.GetComponent<Health>();
            PlayerShooting ps = player.GetComponentInChildren<PlayerShooting>();

            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.SavePlayerData(h, ps);
            }
        }

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneChanger.Instance.ChangeScene(nextSceneIndex);
    }
}