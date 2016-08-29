using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LvlUpBoard : MonoBehaviour {

    [SerializeField]
    AnimationCurve fadeOut;

    [SerializeField]
    float fadeInSpeed = 0.05f;

    [SerializeField]
    float fadeOutSpeed = 0.01f;

    [SerializeField]
    float textFactor = 1.2f;

    Color imgColor = Color.white;
    Color txtColor = Color.black;

    [SerializeField]
    Text textArea;

    [SerializeField]
    Image img;

    [SerializeField]
    float showDuration;

    [SerializeField]
    string[] messages;

    [SerializeField]
    float stepDelay = 0.01f;

    void OnEnable()
    {
        World.OnNewLevel += World_OnNewLevel;
    }

    void OnDestroy()
    {
        World.OnNewLevel -= World_OnNewLevel;
    }

    private void World_OnNewLevel(int lvl)
    {
        if (lvl < messages.Length)
        {
            textArea.text = messages[lvl];
            StartCoroutine(_fadeInOut());
        }
    }

    IEnumerator<WaitForSeconds> _fadeInOut()
    {
        imgColor.a = 0;
        txtColor.a = 0;
        img.color = imgColor;
        textArea.color = txtColor;

        img.enabled = true;
        textArea.enabled = true;

        for (float i = 0; i < 1f; i += fadeInSpeed)
        {

            imgColor.a = Mathf.Lerp(0, 1, fadeOut.Evaluate(i));
            txtColor.a = Mathf.Lerp(0, 1, textFactor * fadeOut.Evaluate(i));
            img.color = imgColor;
            textArea.color = txtColor;
            yield return new WaitForSeconds(stepDelay);
        }

        imgColor.a = 1;
        txtColor.a = 1;
        img.color = imgColor;
        textArea.color = txtColor;

        yield return new WaitForSeconds(showDuration);

        for (float i = 0; i < 1f; i += fadeOutSpeed)
        {

            imgColor.a = Mathf.Lerp(1, 0, fadeOut.Evaluate(i));
            txtColor.a = Mathf.Lerp(1, 0, textFactor * fadeOut.Evaluate(i));
            img.color = imgColor;
            textArea.color = txtColor;
            yield return new WaitForSeconds(stepDelay);
        }

        img.enabled = false;
        textArea.enabled = false;
    }
}
