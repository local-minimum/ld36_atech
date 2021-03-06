﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


public enum TalkEvents { Silent, Talking, Thinking};
public delegate void Talk(TalkEvents type);

public class TextTalker : MonoBehaviour {

    public event Talk OnTalk;

    Animator anim;

    [SerializeField]
    Text TextArea;

    [SerializeField, Range(0, 1)]
    float defaultWordPause = 0.1f;
    float wordPause = 0.1f;

    [SerializeField, Range(0, 100)]
    int defaultSentencePauses = 5;
    int sentencePauses = 5;

    [SerializeField, Range(0, 1)]
    float defaultSentencePause = 0.4f;
    float sentencePause = 0.4f;

    [SerializeField]
    string sentencePauseAnimation = "...";

    [SerializeField]
    Button toggleButton;

    bool recievedInterrupt = false;
    bool talking = false;

    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    void OnEnable()
    {
        OnTalk += TextTalker_OnTalk;
    }

    void OnDisable()
    {
        OnTalk -= TextTalker_OnTalk;
    }

    private void TextTalker_OnTalk(TalkEvents type)
    {
        if (type == TalkEvents.Talking)
        {
            anim.SetTrigger("talk");
        } else
        {
            anim.SetTrigger("wait");
        }
    }

    public void Talk(string text)
    {
        StartCoroutine(_Talk(text));
    }

    public void Talk(DialoguePart part)
    {
        if (part.wordSpeed > 0)
        {
            wordPause = part.wordSpeed;
        }
        else
        {
            wordPause = defaultWordPause;
        }

        if (part.endOfSentenceSpeed > 0)
        {
            sentencePause = part.endOfSentenceSpeed;
        } else
        {
            sentencePause = defaultSentencePause;
        }

        if (part.endOfSentenceDelays > 0)
        {
            sentencePauses = part.endOfSentenceDelays;
        } else
        {
            sentencePauses = defaultSentencePauses;
        }

        Talk(part.text);
    }

    IEnumerator<WaitForSeconds> _Talk(string text)
    {
        talking = true;
        recievedInterrupt = false;

        if (OnTalk != null)
        {
            OnTalk(TalkEvents.Talking);
        }

        if (toggleButton)
        {
            toggleButton.interactable = false;
        }
        TextArea.text = "";
        int pos = 0;
        int len = text.Length;
        char[] wordlimits = new char[] { ',', '.', ' ', '?', '!', '-' };
        char[] sentencelimits = new char[] { '.', '?', '!' };
        bool atSentenceEnd = false;

        while (pos < len)
        {
            int concatStart = pos;
    

            while (pos < len)
            {

                if (sentencelimits.Contains(text[pos]))
                {
                    atSentenceEnd = true;
                    break;
                } else if (wordlimits.Contains(text[pos]))
                {
                    break;
                } else {
                    pos++;
                }
            }

            if (pos >= len)
            {
                break;
            }
            

            if (atSentenceEnd)
            {
                
                while (pos < len)
                {

                    if (sentencelimits.Contains(text[pos]))
                    {
                        atSentenceEnd = false;
                        break;
                    }
                    else {
                        pos++;
                    }
                }
                pos++;
                TextArea.text += text.Substring(concatStart, pos - concatStart);

                if (pos < len)
                {
                    if (OnTalk != null)
                    {
                        OnTalk(TalkEvents.Thinking);
                    }

                    recievedInterrupt = false;
                    string curText = TextArea.text;
                    for (int i = 0; i < sentencePauses; i++)
                    {
                        TextArea.text = curText + sentencePauseAnimation.Substring(0, i % (sentencePauseAnimation.Length + 1));
                        if (recievedInterrupt)
                        {
                            i = sentencePauses;
                            break;
                        }
                        else
                        {
                            yield return new WaitForSeconds(sentencePause);
                        }
                    }
                    TextArea.text = curText;
                    recievedInterrupt = false;

                    if (OnTalk != null)
                    {
                        OnTalk(TalkEvents.Talking);
                    }
                }
            }
            else {
                pos++;
                TextArea.text += text.Substring(concatStart, pos - concatStart);

                if (!recievedInterrupt)
                {
                    yield return new WaitForSeconds(wordPause);
                }
            }
            
        }

        TextArea.text = text;
        if (toggleButton)
        {
            toggleButton.interactable = true;
        }
        if (OnTalk != null)
        {
            OnTalk(TalkEvents.Silent);
        }
        talking = false;
    }

    void Update()
    {
        if (talking && !recievedInterrupt)
        {
   
            if (Input.anyKeyDown || (Input.touchCount > 0))
            {
                recievedInterrupt = true;

            }
        }
    }
}
