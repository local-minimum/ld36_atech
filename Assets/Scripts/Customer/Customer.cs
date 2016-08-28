using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public enum CustomerMode {Order, Pay};

public class DialoguePart
{
    public int level;
    public string name;
    public string orderTitle;
    public string text;
    public List<string> positiveCriteria;
    public List<string> negativeCriteria;
    public float wordSpeed;
    public float endOfSentenceSpeed;
    public float endOfSentenceDelays;
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
    public int currentIndex = -1;
    public static List<int> metCustomers = new List<int>();
    public List<Sprite> sprites = new List<Sprite>();
    public List<Sprite> faces = new List<Sprite>();
    public List<string> names = new List<string>();
    [HideInInspector] public List<string> greetings = new List<string>();
    [HideInInspector] public List<int> customerLvl = new List<int>();

    public List<string> json_files = new List<string>();

    static Dictionary<int, List<DialoguePart>> dialogues = new Dictionary<int, List<DialoguePart>>();

    void Start () {

        LoadJSON();

        canvas = GetComponent<Canvas>();
        canvas.enabled = true;
        if (customerMode == CustomerMode.Order)
        {
            SetCustomerFromLevel();
        }
	}
	
    void LoadJSON()
    {
        if (dialogues.Count > 0)
        {
            return;
        }

        for (int part_index = 0, files = json_files.Count; part_index < files; part_index++)
        {
            TextAsset asset = Resources.Load<TextAsset>(json_files[part_index]);
            if (asset == null)
            {
                Debug.LogError("Missing file: " + json_files[part_index]);
                continue;
            }
            string json = asset.text;
            List<DialoguePart> dialogueParts = JsonUtility.FromJson<List<DialoguePart>>(json);
            for (int i = 0, l = dialogueParts.Count; i < l; i++)
            {
                DialoguePart part = dialogueParts[i];
                if (!dialogues.ContainsKey(part.level))
                {
                    dialogues[part.level] = new List<DialoguePart>();
                }
                dialogues[part.level].Add(part);
            }
        }
    }

    public void HideCustomer()
    {
        canvas.enabled = false;
    }

    public void Record()
    {
        if (currentIndex < 0)
        {

            sprites.Add(avatar.sprite);
            greetings.Add(textArea.text);
            names.Add(nameArea.text);
            faces.Add(faceImage.sprite);

        } else if (currentIndex < sprites.Count)
        {
            sprites[currentIndex] = avatar.sprite;
            greetings[currentIndex] = textArea.text;
            names[currentIndex] = nameArea.text;
            faces[currentIndex] = faceImage.sprite;

        } else
        {
            Debug.LogError("Index too large, " + currentIndex);
        }
    }

    public void SetCustomerFromLevel()
    {
        SetCustomerIndex();
        avatar.sprite = sprites[currentIndex];
        textTalk.Talk(greetings[currentIndex]);
        nameArea.text = names[currentIndex];
        workshopNameArea.text = names[currentIndex];
        faceImage.sprite = faces[currentIndex];
    }

    void SetCustomerIndex()
    {
        int[] potentials = customerLvl.Select((lvl, index) => new { lvl = lvl, index = index }).Where(item => item.lvl == World.Level).Select(item => item.index).ToArray();
        int[] candidates = potentials.Where(index => !metCustomers.Contains(index)).ToArray();
        if (candidates.Length > 0)
        {
            currentIndex = candidates[Random.Range(0, candidates.Length)];
            metCustomers.Add(currentIndex);
        }
        else
        {
            metCustomers.RemoveAll(val => potentials.Contains(val));
            currentIndex = potentials[Random.Range(0, potentials.Length)];
            metCustomers.Add(currentIndex);

        }

    }
}
