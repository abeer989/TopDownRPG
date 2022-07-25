using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables/Ref.
    public static PlayerController instance;

    [Header("Components")]
    [SerializeField] Rigidbody2D RB;
    [SerializeField] Animator animator;

    [Header("Floats")]
    [SerializeField] float moveSpeed;

    BoxCollider2D playerBoundsBox;

    string horizontalAxis = "Horizontal";
    string verticalAxis = "Vertical";
    string lastExitUsed;
    bool canMove = true;

    // public properties:
    public string LastExitUsed
    {
        get { return lastExitUsed; }
        set { lastExitUsed = value; }
    }

    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }

    public Animator Animator
    {
        get { return animator; }
        set { animator = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    } 
    #endregion

    void Update()
    {
        #region Vertical/Horizontal Movement:
        float horizontalMovement = Input.GetAxisRaw(horizontalAxis);
        float verticalMovement = Input.GetAxisRaw(verticalAxis);

        if (canMove)
            RB.velocity = new Vector3(horizontalMovement, verticalMovement, 0) * moveSpeed;

        else
            RB.velocity = Vector2.zero;

        animator.SetFloat("moveX", RB.velocity.x);
        animator.SetFloat("moveY", RB.velocity.y);

        if (canMove)
        {
            if (horizontalMovement == 1 || horizontalMovement == -1 || verticalMovement == 1 || verticalMovement == -1)
            {
                animator.SetFloat("lastMoveX", horizontalMovement);
                animator.SetFloat("lastMoveY", verticalMovement);
            } 
        }

        // keeping the player within the same bounds as the camera:
        transform.position = new Vector3(x: Mathf.Clamp(transform.position.x, playerBoundsBox.bounds.min.x + .5f, playerBoundsBox.bounds.max.x - .5f),
                                         y: Mathf.Clamp(transform.position.y, playerBoundsBox.bounds.min.y, playerBoundsBox.bounds.max.y - 2),
                                         z: transform.position.z);
        #endregion
    }

    /// <summary>
    /// set bounds for player according to the camera bounds:
    /// </summary>
    /// <param name="_boundsBox"></param>
    public void SetBounds(BoxCollider2D _boundsBox) => playerBoundsBox = _boundsBox;
}
