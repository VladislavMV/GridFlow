using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 0.5f;

    private void Start()
    {
        Destroy(gameObject, destroyDelay);
    }
}