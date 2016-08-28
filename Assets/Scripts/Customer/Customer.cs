using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;

public enum CustomerMode {Order, Pay};

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

    public Image avatar;
    public Text nameArea;
    public Text textArea;
    public TextTalker textTalk;
    public Image faceImage;
    public Text workshopNameArea;
    public static CustomerMode customerMode = CustomerMode.Order;
    static int currentIndex = -1;
    public List<Sprite> sprites = new List<Sprite>();
    public List<Sprite> faces = new List<Sprite>();
    public List<string> identifiers = new List<string>();
    public List<AudioClip> musics = new List<AudioClip>();

    public List<string> json_files = new List<string>();

    static Dictionary<int, List<DialoguePart>> dialogues = new Dictionary<int, List<DialoguePart>>();
    static Dictionary<int, List<bool>> usedDialogues = new Dictionary<int, List<bool>>();

    [SerializeField]
    AudioSource speaker;
    [SerializeField]
    AudioSource speakerB;
    [SerializeField]
    AudioSource speakerButtons;

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
            SetCustomerFromLevel();
        } else
        {
            SetupResponse();
        }
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
                Debug.Log("Parsing JSON: " + json_files[part_index]);
            }
            string json = asset.text;
            DialoguePart part = JsonUtility.FromJson<DialoguePart>(json);
            if (!dialogues.ContainsKey(part.level))
            {
                dialogues[part.level] = new List<DialoguePart>();
                usedDialogues[part.level] = new List<bool>();
            }

            dialogues[part.level].Add(part);
            usedDialogues[part.level].Add(false);

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
    }

    string[] GetInvalidCriteria(string[] criteria) {
        return criteria.Where(e => !Workshop.ingredients.ContainsKey(e)).ToArray();
    }

    public void HideCustomer()
    {

        speakerButtons.PlayOneShot(toWorkshopSound);
        workshopSnapshot.TransitionTo(fadeTime);
        canvas.enabled = false;
    }

    public void Record()
    {
        if (currentIndex < 0)
        {

            sprites.Add(avatar.sprite);
            identifiers.Add(nameArea.text);
            faces.Add(faceImage.sprite);

        } else if (currentIndex < sprites.Count)
        {
            sprites[currentIndex] = avatar.sprite;
            identifiers[currentIndex] = nameArea.text;
            faces[currentIndex] = faceImage.sprite;

        } else
        {
            Debug.LogError("Index too large, " + currentIndex);
        }
    }

    public void SetCustomerFromLevel()
    {
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
        workshopNameArea.text = part.name;

        int listIndex = GetListIndex(part.identifier);
        if (listIndex < 0)
        {
            Debug.LogError(string.Format("Could not find identifier '{0}' ({1}) in lists.", part.identifier, part.name));
        }

        speaker.clip = musics[listIndex];
        speakerB.clip = musics[listIndex];
        speaker.Play();
        speakerB.Play();

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
        List<bool> used = usedDialogues[World.Level];

        int[] candidates = used.Select((val, index) => new {index = index, used = val}).Where(item => !item.used).Select(item => item.index).ToArray();        
        if (candidates.Length > 0)
        {
            currentIndex = candidates[Random.Range(0, candidates.Length)];
        }
        else
        {
            for (int i=0, l=usedDialogues[World.Level].Count; i< l; i++)
            {
                usedDialogues[World.Level][i] = false;
            }

            int[] potentials = Enumerable.Range(0, usedDialogues[World.Level].Count).Where((e, i) => i != currentIndex).ToArray();
            currentIndex = potentials[Random.Range(0, potentials.Length)];
            
        }
        usedDialogues[World.Level][currentIndex] = true;
    }

    public int Score
    {
        get
        {
            int score = baseScore;
            //TODO: Add stuff
            return score;
        }
    }
}
