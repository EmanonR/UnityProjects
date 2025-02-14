using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    [SerializeField] float PickUpRange = 1.5f;
    public Item item;

    SpriteRenderer sprite; 
    float lerpTime = .5f;
    float lerpProgress = 0;
    bool interacted = false, done = false;
    Vector2 startPos;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        if (item != null)
        {
            sprite.sprite = item.icon;
        }
    }

    private void Update()
    {
        if (GameManager.instance.player == null) return;

        if (done)
        {
            Interact();
            return;
        }

        if (!interacted)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, GameManager.instance.player.position + new Vector3(0, 1));

            if (distanceToPlayer < PickUpRange)
            {
                interacted = true;
                startPos = transform.position;
            }
        }
        else
        {
            PickUp();
        }
    }

    void PickUp()
    {
        lerpProgress += Time.deltaTime / lerpTime;
        transform.position = Vector2.Lerp(startPos, GameManager.instance.player.position + new Vector3(0,1), lerpProgress);

        if (lerpProgress >= 1)
        {
            done = true;
        }
    }

    void Interact()
    {
        //Move to inventory
        GameManager.instance.inventoryManager.AddItem(item);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (item != null && !Application.isPlaying)
        {
            sprite = GetComponent<SpriteRenderer>();

            sprite.sprite = item.icon;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, PickUpRange);
    }
}
