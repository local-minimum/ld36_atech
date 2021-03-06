﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum CustomerMode {Order, Pay};
public delegate void CustomerModeEvent(CustomerMode mode);

[System.Serializable]
public class DialoguePart
{
    public int level;
    public string identifier;
    public string name;
    public string orderTitle;
    public string text;
    public string element;
    public string[] positiveCriteria;
    public string[] negativeCriteria;
    public float wordSpeed = 0.2f;
    public float endOfSentenceSpeed = 0.4f;
    public int endOfSentenceDelays = 5;
    public string positiveFeedback;
    public string neutralFeedback;
    public string negativeFeedback;
}

public class Customer : MonoBehaviour {

    Canvas canvas;

    public event CustomerModeEvent OnCustomerModeChange;

    public Image avatar;
    public Text nameArea;
    public Text textArea;
    public TextTalker textTalk;
    public Image faceImage;
    public Text workshopNameArea;

    static CustomerMode _customerMode = CustomerMode.Order;
    public CustomerMode customerMode
    {
        get
        {
            return _customerMode;
        }

        set
        {
            _customerMode = value;
            if (OnCustomerModeChange != null)
            {
                OnCustomerModeChange(_customerMode);
            }
        }
    }

    public static void ResetGame()
    {
        _customerMode = CustomerMode.Order;
        currentIndex = -1;
        foreach(int lvl in usedDialogues.Keys)
        {
            for (int i=0, l=usedDialogues[lvl].Count; i< l; i++)
            {
                usedDialogues[lvl][i] = false;
            }
        }
    }

    static int currentIndex = -1;
    public List<Sprite> sprites = new List<Sprite>();
    public List<Sprite> faces = new List<Sprite>();
    public List<string> identifiers = new List<string>();
    public List<AudioClip> musics = new List<AudioClip>();

    public List<string> json_files = new List<string>();

    static Dictionary<int, List<DialoguePart>> dialogues = new Dictionary<int, List<DialoguePart>>();
    static Dictionary<int, List<bool>> usedDialogues = new Dictionary<int, List<bool>>();

    [SerializeField]
    Text txt;

    [SerializeField]
    string nextCustomer = "Next Customer";

    [SerializeField]
    string toWorkShopText = "To Workshop >";

    [SerializeField]
    string theEndText = "The Next Night";

    [SerializeField]
    AudioMixerSnapshot workshopSnapshot;
    [SerializeField]
    AudioMixerSnapshot customerOrderSnapshot;
    [SerializeField]
    AudioMixerSnapshot customerResponseSnapshot;
    [SerializeField]
    float fadeTime = 0.1f;

    [SerializeField]
    AudioClip toWorkshopSound;

    [SerializeField] int baseScore = 400;
    [SerializeField] int failScorePart = -50;
    [SerializeField] int criticalFail = -400;
    [SerializeField] int bonusPart = 50;
    [SerializeField] int completeBonus = 400;

    void Start () {

        LoadJSON();
                
        canvas = GetComponent<Canvas>();
        canvas.enabled = true;
        if (customerMode == CustomerMode.Order)
        {
            txt.text = toWorkShopText;
            SetCustomerFromLevel();
        } else
        {
            txt.text = nextCustomer;
            SetupResponse();
        }
	}

	void OnEnable() {
		World.OnNewLevel += World_OnNewLevel;
	}

	void OnDestroy() {
		World.OnNewLevel -= World_OnNewLevel;
	}

	void World_OnNewLevel (int lvl)
	{
		currentIndex = -1;
	}

    void LoadJSON()
    {
        if (dialogues.Count > 0)
        {
            return;
        }

        if (Workshop.ingredients.Count == 0)
        {
            Workshop.LoadJSON();
        }

        for (int part_index = 0, files = json_files.Count; part_index < files; part_index++)
        {
            TextAsset asset = Resources.Load(json_files[part_index]) as TextAsset;
            if (asset == null)
            {
                Debug.LogError("Missing file: " + json_files[part_index]);
                continue;
            } else
            {
                Debug.Log("Loading file: " + json_files[part_index]);
            }
            string json = asset.text;
            DialoguePart part = JsonUtility.FromJson<DialoguePart>(json);

            if (GetListIndex(part.identifier) < 0)
            {
                Debug.LogError(string.Format("Unknown customer identifier: {0} in file {1}", part.identifier, json_files[part_index]));
            }

            if (!dialogues.ContainsKey(part.level))
            {
                dialogues[part.level] = new List<DialoguePart>();
                usedDialogues[part.level] = new List<bool>();
            }

            dialogues[part.level].Add(part);
            usedDialogues[part.level].Add(false);

            Debug.Log(string.Format("Loaded {0} ({1}) at {2}", part.orderTitle, part.name, part.level));

            string[] invalid = GetInvalidCriteria(part.negativeCriteria);
            if (invalid.Length > 0)
            {
                Debug.LogError("These negative don't exist: " + string.Join(", ", invalid));
            }

            invalid = GetInvalidCriteria(part.positiveCriteria);
            if (invalid.Length > 0)
            {
                Debug.LogError("These positive don't exist: " + string.Join(", ", invalid));
            }
        }
        
    }

    void SetupResponse()
    {
        DialoguePart part = SetupCustomer(false);
        customerResponseSnapshot.TransitionTo(fadeTime);
        int positives;
        int negatives;
        int score = Score(part, out positives, out negatives);
        if (negatives > positives)
        {
            Debug.Log("Customer Negative");
            textTalk.Talk(part.negativeFeedback);
        } else if (positives > negatives + 1)
        {
            Debug.Log("Customer Positive");
            textTalk.Talk(part.positiveFeedback);
        } else
        {
            Debug.Log("Customer Neutral");
            textTalk.Talk(part.neutralFeedback);
        }
        World.AddScore(score, negatives > 0);
    }

    string[] GetInvalidCriteria(string[] criteria) {
        return criteria.Where(e => !Workshop.ingredients.ContainsKey(e)).ToArray();
    }

    public void HideCustomer()
    {
        if (customerMode == CustomerMode.Order)
        {
            SingleCam.ButtonSpeaker.PlayOneShot(toWorkshopSound);
            workshopSnapshot.TransitionTo(fadeTime);
            canvas.enabled = false;
        } else
        {
            if (!World.GameOver)
            {
                txt.text = toWorkShopText;
                World.RocketBlueprint.Clear();
                SetCustomerFromLevel();
            } else
            {
                if (World.LastWasNegative || World.Score < 0)
                {
                    SceneManager.LoadScene("failure");
                } else
                {
                    SceneManager.LoadScene("success");
                }
            }

        }
    }
		
    public void SetCustomerFromLevel()
    {
        customerMode = CustomerMode.Order;

        SetCustomerIndex();
        SetupCustomer();
    }

    DialoguePart SetupCustomer(bool doTalk=true)
    {
        DialoguePart part = dialogues[World.Level][currentIndex];
        if (doTalk)
        {
            textTalk.Talk(part);
        }
        nameArea.text = part.orderTitle;
        workshopNameArea.text = "Back to " + part.name;

        int listIndex = GetListIndex(part.identifier);
        if (listIndex < 0)
        {
            Debug.LogError(string.Format("Could not find identifier '{0}' ({1}) in lists.", part.identifier, part.name));
        }
        SingleCam.CustomerOrderSpeaker.clip = musics[listIndex];
        SingleCam.CustomerResponseSpeaker.clip = musics[listIndex];
        SingleCam.CustomerOrderSpeaker.Play();
        SingleCam.CustomerResponseSpeaker.Play();

        customerOrderSnapshot.TransitionTo(fadeTime);

        faceImage.sprite = faces[listIndex];
        avatar.sprite = sprites[listIndex];

        return part;
    }

    int GetListIndex(string identifier)
    {
        for (int i=0, l=identifiers.Count; i< l; i++)
        {
            if (identifiers[i] == identifier)
            {
                return i;
            }
        }
        return -1;
    }

    void SetCustomerIndex()
    {
        int lvl = World.Level;
        List<bool> used = usedDialogues[lvl];

        int[] candidates = used.Select((val, index) => new {index = index, used = val}).Where(item => !item.used).Select(item => item.index).ToArray();        
        if (candidates.Length > 0)
        {
            currentIndex = candidates[Random.Range(0, candidates.Length)];
        }
        else if (usedDialogues[lvl].Count == 1)
        {
			currentIndex = 0;
            Debug.LogWarning("Will repeat same dialigoue because only at level " + lvl);
        } else
        {
            for (int i=0, l=usedDialogues[lvl].Count; i< l; i++)
            {
                usedDialogues[lvl][i] = false;
            }

            int[] potentials = Enumerable.Range(0, usedDialogues[lvl].Count).Where((e, i) => i != currentIndex).ToArray();
            currentIndex = potentials[Random.Range(0, potentials.Length)];
            
        }
        usedDialogues[lvl][currentIndex] = true;
    }

    public int Score(DialoguePart part, out int positives, out int negatives)
    {
  
        int score = baseScore;
        List<string> lst = World.RocketBlueprint.Values.Select(kvp => kvp.Value).ToList();
        int shots = lst.Count;

        positives = part.positiveCriteria.Where(e => lst.Contains(e)).Count();
        negatives = part.negativeCriteria.Where(e => lst.Contains(e)).Count();

        score += positives * bonusPart;
        score -= negatives * failScorePart;

        if (negatives == part.negativeCriteria.Length || negatives == shots)
        {
            score -= criticalFail;
        }
        else if (positives == part.positiveCriteria.Length || positives == shots)
        {
            score += completeBonus;
        }
        return score;
   
    }
}
