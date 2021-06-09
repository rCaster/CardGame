using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawPile : MonoBehaviour
{
    public List<GameObject> cardsInDrawPile = new List<GameObject>();
    public Text UI_text;

    void OnEnable()
    {
        UI_text = gameObject.GetComponent<Text>();
        //Debug.Log(UI_text.text);
    }

    public void AddCard(GameObject card)
    {
        card.transform.SetParent(gameObject.transform);
        cardsInDrawPile.Add(card);
        UpdateCounter();
    }

    public GameObject NextCard()
    {
        if (cardsInDrawPile.Count > 0)
        {
            int nextIndex = cardsInDrawPile.Count - 1;
            GameObject nextCard = cardsInDrawPile[nextIndex];
            cardsInDrawPile.RemoveAt(nextIndex);
            UpdateCounter();
            return nextCard;
        }
        else
        {
            Debug.Log("No cards to draw");
            return null;
        }
    }

    void UpdateCounter()
    {
        UI_text.text = "Draw Pile\n" + cardsInDrawPile.Count;
    }
}
