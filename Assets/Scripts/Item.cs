using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] ItemScriptable referenceScriptableObject;
    [SerializeField] GameObject interactCanvas;
    
    [Space]
    [SerializeField] string playerTag;

    SpriteRenderer spriteRenderer;
    Sprite itemSprite;

    string itemName;
    bool canPickup;

    private void OnEnable() => SetUpItem(reference: referenceScriptableObject);

    public enum ItemType
    {
        none,
        hp_potion,
        mp_potion,
        str_buff,
        def_buff,
        weapon,
        armor,
        collectable
    }

    private void Update()
    {
        if (canPickup && Input.GetKeyDown(key: KeyCode.E) && PlayerController.instance.CanMove)
        {
            GameManager.instance.AddItemToInventory(itemToAddDetails: referenceScriptableObject, itemQuantity: 1);
            Destroy(); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            canPickup = true;
            interactCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            canPickup = false;
            interactCanvas.SetActive(false);
        }
    }

    void Destroy()
    {
        gameObject.SetActive(false);
        Destroy(gameObject, .5f);
    }

    public void SetUpItem(ItemScriptable reference)
    {
        if (reference)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            itemSprite = reference.ItemSprite;
            itemName = reference.ItemName;

            spriteRenderer.sprite = itemSprite;
            gameObject.name = itemName; 
        }
    }
}
