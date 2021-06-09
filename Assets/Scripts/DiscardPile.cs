using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardPile : MonoBehaviour
{
    public List<GameObject> cardsInDiscardPile = new List<GameObject>();
    public Text UI_text;

    void OnEnable()
    {
        UI_text = gameObject.GetComponent<Text>();
    }

    public void AddCard(GameObject card)
    {
        card.transform.SetParent(gameObject.transform);
        cardsInDiscardPile.Add(card);
        UpdateCounter();
    }

    void UpdateCounter()
    {
        UI_text.text = "Discard Pile\n" + cardsInDiscardPile.Count;
    }
}
