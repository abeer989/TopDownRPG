using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    [SerializeField] float lifeTime;

    private void OnEnable() => Destroy(gameObject, lifeTime);
}
