using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image logoImage;

    [Header("LogoImage Setting")]
    [Range(0f, 5f)] [SerializeField] private float logoWaitTime = 1f;
    [Range(0f, 5f)] [SerializeField] private float logoFadeinDuration = 1f;
    [Range(0f, 5f)] [SerializeField] private float logoShowDuration = 2f;
    [Range(0f, 5f)] [SerializeField] private float logoFadeoutDuration = 1f;

    [Space(10)]

    [Header("LogoImage Setting")]
    [Range(0f, 5f)] [SerializeField] private float backgroundWaitTime = 1f;
    [Range(0f, 5f)] [SerializeField] private float backgroundFadeinDuration = 1f;
    [Range(0f, 5f)] [SerializeField] private float backgroundShowDuration = 2f;
    [Range(0f, 5f)] [SerializeField] private float backgroundFadeoutDuration = 1f;

    public IEnumerator ShowLogo() {
        gameObject.SetActive(true);

        // Initial Setting
        background.gameObject.SetActive(true);
        Color bc = background.color;
        background.color = new Color(bc.r, bc.g, bc.b, 1f);

        logoImage.gameObject.SetActive(true);
        Color lc = logoImage.color;
        logoImage.color = new Color(lc.r, lc.g, lc.b, 0f);
        
        // Show logo
        yield return new WaitForSeconds(logoWaitTime);

        yield return Fade(logoImage, 0f, 1f, logoFadeinDuration);

        yield return new WaitForSeconds(logoShowDuration);

        yield return Fade(logoImage, 1f, 0f, logoFadeoutDuration);

        logoImage.gameObject.SetActive(false);

        yield return SceneManager.LoadSceneAsync("MainScene");

        yield return new WaitForSeconds(backgroundWaitTime);

        yield return Fade(background, 1f, 0f, backgroundFadeoutDuration);

        background.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(Image target, float startAlpha, float endAlpha, float time) {
        float elapsed = 0f;
        Color c = target.color;

        while(elapsed <= time) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / time);
            target.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        target.color = new Color(c.r, c.g, c.b, endAlpha);
    }
}
