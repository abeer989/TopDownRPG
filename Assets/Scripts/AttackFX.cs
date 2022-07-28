using UnityEngine;

public class AttackFX : MonoBehaviour
{
    [SerializeField] bool playSFX;

    [Space]
    [SerializeField] float lifeTime;
    [SerializeField] int SFXIndex;

    private void OnEnable()
    {
        if (playSFX)
            AudioManager.instance.PlaySFX(SFXIndex);

        Destroy(gameObject, lifeTime);
    }
}
