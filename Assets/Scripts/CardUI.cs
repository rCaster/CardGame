using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public float autoMoveSpeed = 10;
    public float growShrinkSpeed = 10;
    public Canvas cardCanvas;
    public int DefaultSortingOrder;
    public Image CardImage;

    Image ColliderImage;
    RectTransform ColliderRect;
    ICard cardBehaviour;
    Hand hand;
    Vector2 cardHomePosition;
    Vector3 cardClickOffset;
    Vector2 defaultColliderSize;
    Vector2 defaultCardScale;
    bool cardAutoMoving;
    readonly float zoomMultiplier = 1.5f;

    void OnEnable()
    {
        cardBehaviour = GetComponentInParent<ICard>();
        if (cardBehaviour == null)
            Debug.LogError("No behaviour for " + gameObject.transform.parent.name + " found!");
        cardCanvas = GetComponentInParent<Canvas>();
        ColliderImage = GetComponent<Image>();
        ColliderRect = ColliderImage.rectTransform;
        hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();
        cardCanvas.overrideSorting = true;
        defaultColliderSize = ColliderRect.sizeDelta;
        defaultCardScale = ColliderRect.localScale;
        cardHomePosition = ColliderRect.position;
        DefaultSortingOrder = cardCanvas.sortingOrder;
    }

    public void SelectCard()
    {
        if (hand != null)
            hand.selectedCard = transform.parent.gameObject;
        cardCanvas.sortingOrder = 10;
        CardImage.rectTransform.position = new Vector2(CardImage.rectTransform.position.x, CardImage.rectTransform.position.y + CardImage.rectTransform.rect.height / 4);
        ColliderRect.position = new Vector2(ColliderRect.position.x, ColliderRect.position.y + ColliderRect.rect.height / 8);
        ColliderRect.sizeDelta = new Vector2(ColliderRect.rect.width, ColliderRect.rect.height * 1.25f);
    }
    public void DeselectCard()
    {
        ColliderRect.sizeDelta = defaultColliderSize;
        ColliderRect.position = cardHomePosition;
        cardCanvas.sortingOrder = DefaultSortingOrder;
        if (!cardAutoMoving)
            CardImage.rectTransform.position = cardHomePosition;
    }

    //for OnPointerEnter and OnPointerExit i added methods for selecting and deselecting cards.
    //the only reason i did this is for the future if we want to add controller/touchscreen/whatever support.
    //we can just call these methods when we want to select a card and it would behave the exact same as mousing over the card.
    //also, "select" might not be the best term here, just the first thing i thought of. strictly means you're just looking at this specific card. not playing it or anything.
    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectCard();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        DeselectCard();
    }

    //when we click on this card we resize it by zoomMultiplier to make it easier to see/read
    //then we make the cursor invisible just for aesthetics
    //lastly, we get the position our mouse is at when we click and get the offset to the cards origin.
    //this is so we can move the card around and the mouse will always be where you clicked it on the card.
    public void OnPointerDown(PointerEventData eventData)
    { 
        CardImage.rectTransform.localScale *= zoomMultiplier;
        Cursor.visible = false;
        Vector2 cardClickPosition = Input.mousePosition;
        cardClickOffset = (Vector3)cardClickPosition - CardImage.rectTransform.position;
    }

    //when we release the click from this 
    public void OnPointerUp(PointerEventData eventData)
    {
        ColliderRect.sizeDelta = defaultColliderSize;
        CardImage.rectTransform.localScale = defaultCardScale;
        ReturnCardToSlot();
        Cursor.visible = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPos = Input.mousePosition - cardClickOffset;
        CardImage.rectTransform.position = newPos;
        ColliderRect.position = newPos;
    }

    public void SetPosition(Vector2 pos)
    {
        ColliderRect.position = pos;
        CardImage.rectTransform.position = pos;
    }

    public void SetHomePositionToCurrent()
    {
        cardHomePosition = CardImage.rectTransform.position;
    }

    void ReturnCardToSlot()
    {
        StartCoroutine(AutoMoveCardToPos(cardHomePosition));
        cardCanvas.sortingOrder = DefaultSortingOrder;
    }

    //slides a card from wherever it is currently at to the target position. speed can be adjusted with the autoMoveSpeed variable.
    //this is basically just for looks. so the card doesn't just suddendly disappear and reappear in its new position in 1 frame.
    //the only implemented use for this so far is returning the card to its spot in your hand if you drag and let go without playing the card.
    //this will be used to move cards all over though. from the draw pile into your hand, from your hand to discard pile, etc...
    IEnumerator AutoMoveCardToPos(Vector2 targetPos)
    {
        while(true)
        {
            float distToTarget = Vector2.Distance(CardImage.rectTransform.position, targetPos);
            cardAutoMoving = distToTarget > .001f;
            Vector2 direction = targetPos - (Vector2)CardImage.rectTransform.position;
            if (distToTarget < .5f)
            {
                CardImage.rectTransform.position = targetPos;
                ColliderRect.position = targetPos;
                ColliderImage.raycastTarget = true;
                cardAutoMoving = false;
                yield break;
            }
            else
            {
                cardAutoMoving = true;
                ColliderImage.raycastTarget = false;
                CardImage.rectTransform.position = (Vector2)CardImage.rectTransform.position + direction * Time.deltaTime * autoMoveSpeed;
                yield return null;
            }
        }
        
    }
}
