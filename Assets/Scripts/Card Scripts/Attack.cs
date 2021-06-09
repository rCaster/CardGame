using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, ICard
{
    public string cardName = "Basic Attack";
    public int damage;
    CardUI cardUI;
    Hand hand;

    void OnEnable()
    {
        gameObject.name = cardName;
        cardUI = GetComponentInChildren<CardUI>();
        if (cardUI == null)
            Debug.LogError("No Card UI found for this card!");
        hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();
    }
    public void Play()
    {
        Debug.Log("Played " + cardName);
    }

    public void Discard()
    {
        hand.Discard(gameObject);
        Debug.Log("Discarded " + cardName);
    }

    public bool IsSingleTargetCard()
    {
        return false;
    }

    public string CardTag()
    {
        return gameObject.tag;
    }

    public int Damage()
    {
        return damage;
    }

}
