using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public DrawPile drawPile;
    public DiscardPile discardPile;
    public List<GameObject> masterCardList;
    public List<GameObject> cardsInHand;
    public List<GameObject> deck = new List<GameObject>();
    public GameObject selectedCard;

    RectTransform handRect;
    Vector2 handPosCheck;
    float horizontalCardSpacing;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Draw(1);
        if (Input.GetKeyDown(KeyCode.Return))
            DiscardHand();

        if (handPosCheck != (Vector2)handRect.position)
        {
            handPosCheck = handRect.position;
            foreach (GameObject card in cardsInHand)
                card.GetComponentInChildren<CardUI>().SetHomePositionToCurrent();
        }
    }

    private void OnEnable()
    {
        handRect = GetComponent<RectTransform>();
        handPosCheck = handRect.position;
        selectedCard = null;
        CreateDeck();
        CreateDrawPile(deck);
    }
    public void Discard(GameObject card)
    {
        if (cardsInHand.Contains(card))
        {
            Debug.Log("Discarding " + card.name);
            discardPile.AddCard(card);
            card.SetActive(false);
            cardsInHand.Remove(card);
            selectedCard = null;
        }
        else
            Debug.LogError("Tried to discard " + card.gameObject.name + " but it was not found in hand.");
    }
    void DiscardHand()
    { 
        for(int i = cardsInHand.Count-1; i >= 0; --i)
            Discard(cardsInHand[i]);       
    }

    void Draw(int numCards)
    {
        for(int i=0;i < numCards; ++i)
        {
            StartCoroutine(DrawCard());
        }
    }

    IEnumerator DrawCard()
    {
        GameObject nextCard = drawPile.NextCard();
        Image nextImage = nextCard.GetComponentInChildren<Image>();
        nextImage.raycastTarget = false;
        if (drawPile.cardsInDrawPile.Count > 0)
            AddCardToHand(nextCard);
        yield return new WaitForSeconds(.5f);
        nextImage.raycastTarget = true;
        yield break;
    }
    void AddCardToHand(GameObject c)
    {
        c.SetActive(true);
        c.transform.SetParent(gameObject.transform);
        CardUI newCardScript = c.GetComponentInChildren<CardUI>();
        float newCardWidth = newCardScript.CardImage.rectTransform.sizeDelta.x;
        if (cardsInHand.Count > 0)
        {
            CardUI prevCardScript = cardsInHand[cardsInHand.Count - 1].GetComponentInChildren<CardUI>();
            foreach(GameObject card in cardsInHand)
            {
                Vector2 prevCardPosition = prevCardScript.CardImage.rectTransform.position;
                prevCardScript.SetPosition(new Vector2(prevCardPosition.x - (newCardWidth + horizontalCardSpacing), prevCardPosition.y));
                prevCardScript.SetHomePositionToCurrent();
                prevCardScript = card.GetComponentInChildren<CardUI>();
            }
            newCardScript.SetPosition(handRect.position);
            newCardScript.SetHomePositionToCurrent();
        }
        else
        {
            newCardScript.SetPosition(handRect.position);
            newCardScript.SetHomePositionToCurrent();
        }
        cardsInHand.Add(c);
        int newCardSortingOrder = cardsInHand.IndexOf(c);
        newCardScript.DefaultSortingOrder = newCardSortingOrder;
        newCardScript.cardCanvas.sortingOrder = newCardSortingOrder;
    }

    //CreateDeck() and CreateDrawPile() will be moved to a more appropriate script once it exists. GameManager or something maybe.
    void CreateDeck()
    {
        for(int i=0;i<20;++i)
        {
            GameObject nextCard = Instantiate(masterCardList[Random.Range(0,masterCardList.Count)]);
            nextCard.transform.SetParent(drawPile.gameObject.transform);
            nextCard.SetActive(false);
            deck.Add(nextCard);
        }
    }
    void CreateDrawPile(List<GameObject> fromCardList)
    {
        foreach(GameObject card in fromCardList)
        {
            card.transform.SetParent(drawPile.gameObject.transform);
            drawPile.AddCard(card);
        }
    }
}
