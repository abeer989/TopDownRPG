using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    [SerializeField] BoxCollider2D cameraBoundsBox;
    
    Transform player;

    float halfHeight;
    float halfWidth;

    // playing audio for the scene:
    [Space]
    [SerializeField] int musicIndex;

    public int MusicIndex
    {
        get { return musicIndex; }
    }

    private void Awake()
    {
        cameraBoundsBox.gameObject.transform.SetParent(transform.parent);
        player = FindObjectOfType<PlayerController>().transform;
        PlayerController.instance.SetBounds(_boundsBox: cameraBoundsBox);
    }

    private void Start()
    {
        AudioManager.instance.PlayMusic(musicIndex);

        // calculating halfWidth and halfHeight to keep the camera's whole FOV within bounds:
        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;
    }

    private void LateUpdate()
    {
        if (player)
            transform.position = new Vector3(x: Mathf.Clamp(player.position.x, cameraBoundsBox.bounds.min.x + halfWidth, cameraBoundsBox.bounds.max.x - halfWidth),
                                             y: Mathf.Clamp(player.position.y, cameraBoundsBox.bounds.min.y + halfHeight, cameraBoundsBox.bounds.max.y - halfHeight),
                                             z: transform.position.z);

        else
        {
            if(PlayerController.instance != null)  
                player = PlayerController.instance.transform;
        }
    }
}
