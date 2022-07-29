using TMPro;
using UnityEngine;

public class ShowDamageNumbers : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageText;

    [SerializeField] float lifeTime = 1;
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float transparencyChangeSpeed = 1;
    [SerializeField] float placementJitter = .5f;

    void Start() => Destroy(gameObject, lifeTime);

    void Update()
    {
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        
        if(damageText.color.a > 0)
            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, Mathf.MoveTowards(damageText.color.a, 0, transparencyChangeSpeed * Time.deltaTime));
    }

    public void SetDamageValue(int damage)
    {
        damageText.SetText(damage.ToString());
        transform.position += new Vector3(Random.Range(-placementJitter, placementJitter), Random.Range(-placementJitter, placementJitter));
    }
}
