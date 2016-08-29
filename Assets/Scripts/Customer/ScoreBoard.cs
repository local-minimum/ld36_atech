using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreBoard : MonoBehaviour {

    [SerializeField]
    Text textArea;

    [SerializeField]
    Image img;

    [SerializeField]
    float stepDelay = 0.01f;

    [SerializeField]
    int stepSize = 1;

    [SerializeField]
    Customer _customer;

    [SerializeField]
    AnimationCurve fadeOut;

    [SerializeField] float fadeSpeed = 0.01f;

    [SerializeField]
    float textFactor = 1.2f;

    Color imgColor = Color.white;
    Color txtColor = Color.black;

    bool scoring = false;

    void OnEnable()
    {
        _customer.OnCustomerModeChange += _customer_OnCustomerModeChange;
        World.OnNewScore += World_OnNewScore;
        
    }

    void OnDisable()
    {
        _customer.OnCustomerModeChange -= _customer_OnCustomerModeChange;
    }

    private void _customer_OnCustomerModeChange(CustomerMode mode)
    {
        StartCoroutine(_fadeOut());
    }

    void OnDestroy()
    {
        World.OnNewScore -= World_OnNewScore;
    }

    private void World_OnNewScore(int oldScore, int score)
    {
        StartCoroutine(_score(oldScore, score));
    }

    IEnumerator<WaitForSeconds> _score(int from, int to)
    {
        scoring = true;
        textArea.text = from.ToString();
        img.enabled = true;
        textArea.enabled = true;
        imgColor.a = 1;
        txtColor.a = 1;
        img.color = imgColor;
        textArea.color = txtColor;
        for (int score= from; score< to; score+=stepSize)
        {
            textArea.text = score.ToString();
            yield return new WaitForSeconds(stepDelay);
        }
        textArea.text = to.ToString();
        scoring = false;
    }

    IEnumerator<WaitForSeconds> _fadeOut()
    {
        for (float i=0; i<1f; i+=fadeSpeed)
        {
            if (scoring)
            {
                break;
            }
            imgColor.a = Mathf.Lerp(1, 0, fadeOut.Evaluate(i));
            txtColor.a = Mathf.Lerp(1, 0, textFactor * fadeOut.Evaluate(i));
            img.color = imgColor;
            textArea.color = txtColor;
            yield return new WaitForSeconds(stepDelay);
        }
    }
}
