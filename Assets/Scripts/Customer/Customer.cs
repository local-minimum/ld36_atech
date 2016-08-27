using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Customer : MonoBehaviour {

    Canvas canvas;

    public Image avatar;
    public Text textArea;

    public int currentIndex = -1;
    public static List<int> metCustomers = new List<int>();
    public List<Sprite> sprites = new List<Sprite>();
    public List<string> greetings = new List<string>();
    public List<int> customerLvl = new List<int>();

    void Start () {
        canvas = GetComponent<Canvas>();
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
        } else if (currentIndex < sprites.Count)
        {
            sprites[currentIndex] = avatar.sprite;
            greetings[currentIndex] = textArea.text;
        } else
        {
            Debug.LogError("Index too large, " + currentIndex);
        }
    }

    public void SetCustomerFromLevel()
    {
        SetCustomerIndex();
        avatar.sprite = sprites[currentIndex];
        textArea.text = greetings[currentIndex];
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
