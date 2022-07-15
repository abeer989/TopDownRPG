using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField] Image fadeImage;

    [SerializeField] float fadeSpeed;

    bool shouldFadeToBlack;
    bool shouldFadeFromBlack;

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

    private void Update()
    {
        if (shouldFadeToBlack)
        {
            fadeImage.color = new Color(r: fadeImage.color.r, g: fadeImage.color.g, b: fadeImage.color.b,
                                        a: Mathf.MoveTowards(fadeImage.color.a, 1, fadeSpeed * Time.deltaTime));

            if (fadeImage.color.a == 1)
                shouldFadeToBlack = false;
        }

        else if (shouldFadeFromBlack)
        {
            fadeImage.color = new Color(r: fadeImage.color.r, g: fadeImage.color.g, b: fadeImage.color.b,
                                        a: Mathf.MoveTowards(fadeImage.color.a, 0, fadeSpeed * Time.deltaTime));

            if (fadeImage.color.a == 0)
                shouldFadeFromBlack = false;
        }
    }

    public void FadeToBlack()
    {
        shouldFadeToBlack = true;
        shouldFadeFromBlack = false;
    }    
    
    public void FadeFromBlack()
    {
        shouldFadeFromBlack = true;
        shouldFadeToBlack = false;
    }
}
