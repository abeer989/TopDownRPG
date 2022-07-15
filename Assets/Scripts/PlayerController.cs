using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Components")]
    [SerializeField] Rigidbody2D RB;
    [SerializeField] Animator animator;

    [Header("Floats")]
    [SerializeField] float moveSpeed;

    string horizontalAxis = "Horizontal";
    string verticalAxis = "Vertical";

    string lastExitUsed/* = "."*/;

    public string LastExitUsed
    {
        get { return lastExitUsed; }
        set { lastExitUsed = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        else
            Destroy(gameObject);
    }

    void Update()
    {
        float horizontalMovement = Input.GetAxisRaw(horizontalAxis);
        float verticalMovement = Input.GetAxisRaw(verticalAxis);

        RB.velocity = new Vector3(horizontalMovement, verticalMovement, 0) * moveSpeed;

        animator.SetFloat("moveX", RB.velocity.x);
        animator.SetFloat("moveY", RB.velocity.y);

        if (horizontalMovement == 1 || horizontalMovement == -1 || verticalMovement == 1 || verticalMovement == -1)
        {
            animator.SetFloat("lastMoveX", horizontalMovement);
            animator.SetFloat("lastMoveY", verticalMovement);
        }
    }
}
