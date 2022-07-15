using UnityEngine;

public class DialogActivator : MonoBehaviour
{
    [SerializeField] string playerTag;

    string[] NPCLines;
    bool canActivate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            canActivate = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            canActivate = false;
    }
}
