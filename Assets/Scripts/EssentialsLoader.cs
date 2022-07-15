using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject UICanvasPrefab;

    private void Awake()
    {
        // loading in a player if one doesn't already exist in the scene:
        if (PlayerController.instance == null)
            PlayerController.instance = Instantiate(playerPrefab).GetComponent<PlayerController>();
            //Instantiate(playerPrefab);

        if (UIController.instance == null)
            UIController.instance = Instantiate(UICanvasPrefab).GetComponent<UIController>();
    }
}