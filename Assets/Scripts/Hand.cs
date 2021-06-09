using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public int maxCards;
    public int horizontalCardSpacing;
    public int verticalCardSpacing;

    public DrawPile drawPile;
    public DiscardPile discardPile;
    public List<GameObject> masterCardList;
    public List<GameObject> cardsInHand;
    public GameObject selectedCard;
    public List<GameObject> deck = new List<GameObject>();

    RectTransform handRect;
    int horizontalSpacingCheck;
    int verticalSpacingCheck;
    Vector2 handPosCheck;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Draw(1);
        if (Input.GetKeyDown(KeyCode.Return))
            DiscardHand();

        if(handPosCheck != (Vector2)handRect.position)
        {
            handPosCheck = handRect.position;
            ArrangeHand();
        }
        if (cardsInHand.Count > 0 && (horizontalSpacingCheck != horizontalCardSpacing || verticalSpacingCheck != verticalCardSpacing))
        {
            verticalSpacingCheck = verticalCardSpacing;
            horizontalSpacingCheck = horizontalCardSpacing;
            ArrangeHand();    
        }
    }

    private void OnEnable()
    {
        horizontalSpacingCheck = horizontalCardSpacing;
        handRect = GetComponent<RectTransform>();
        handRect.position = new Vector2(Screen.width/2, handRect.position.y);
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
    void ArrangeHand()
    {
        for (int i = 0; i < cardsInHand.Count; ++i)
        {
            CardUI script = cardsInHand[i].GetComponentInChildren<CardUI>();
            float cardWidth = script.CardImage.rectTransform.rect.width;
            float xPos = handRect.position.x + ((cardWidth + horizontalCardSpacing) * i);
            float yPos = handRect.position.y + (verticalCardSpacing * i);
            script.SetPosition(new Vector2(xPos, yPos));
            script.CardCanvas.sortingOrder = i;
            script.DefaultSortingOrder = i;
        }
        Vector2 lastCardPos = cardsInHand[cardsInHand.Count-1].GetComponentInChildren<CardUI>().CardImage.rectTransform.position;
        Vector2 firstCardPos = cardsInHand[0].GetComponentInChildren<CardUI>().CardImage.rectTransform.position;
        float handWidth = Vector2.Distance(lastCardPos, firstCardPos);
        handRect.sizeDelta = new Vector2(handWidth, handRect.sizeDelta.y);
        handRect.position = new Vector2((Screen.width / 2) - (handWidth / 2), handRect.position.y);
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
    void CreateDrawPile(List<GameObject> from)
    {
        foreach(GameObject card in from)
        {
            card.transform.SetParent(drawPile.gameObject.transform);
            drawPile.AddCard(card);
        }
    }
    void AddCardToDeck(GameObject card)
    {
        deck.Add(card);
    }
    void AddCardToHand(GameObject c)
    {
        c.SetActive(true);
        c.transform.SetParent(gameObject.transform);
        CardUI script = c.GetComponentInChildren<CardUI>();
        script.SetPosition(handRect.position);
        cardsInHand.Add(c);
        if(cardsInHand.Count > 1)
        ArrangeHand();
    }








}
