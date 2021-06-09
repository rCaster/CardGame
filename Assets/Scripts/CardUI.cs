using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image CardImage;
    public List<GameObject> CardTextObjects;
    public List<RectTransform> CardTextRects;

    public Canvas CardCanvas { get; set; }
    //public Text CardText { get; set; }
    public int DefaultSortingOrder { get; set; }
    public float autoMoveSpeed = 10;
    public float growShrinkSpeed = 10;

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
        CardCanvas = GetComponentInParent<Canvas>();
        ColliderImage = GetComponent<Image>();
        ColliderRect = ColliderImage.rectTransform;
        CardTextRects = new List<RectTransform>();
        foreach (GameObject t in CardTextObjects)
            CardTextRects.Add(t.GetComponent<RectTransform>());
        hand = GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>();
        CardCanvas.overrideSorting = true;
        defaultColliderSize = ColliderRect.sizeDelta;
        defaultCardScale = ColliderRect.localScale;
        cardHomePosition = ColliderRect.position;
        DefaultSortingOrder = CardCanvas.sortingOrder;
    }

    public void SelectCard()
    {
        if (hand != null)
            hand.selectedCard = transform.parent.gameObject;
        CardCanvas.sortingOrder = 10;
        CardImage.rectTransform.position = new Vector2(CardImage.rectTransform.position.x, CardImage.rectTransform.position.y + CardImage.rectTransform.rect.height / 4);
        ColliderRect.position = new Vector2(ColliderRect.position.x, ColliderRect.position.y + ColliderRect.rect.height / 4);
        ColliderRect.sizeDelta = new Vector2(ColliderRect.rect.width, ColliderRect.rect.height * 1.25f);
    }
    public void DeselectCard()
    {
        ColliderRect.sizeDelta = defaultColliderSize;
        ColliderRect.position = cardHomePosition;
        CardCanvas.sortingOrder = DefaultSortingOrder;
        if (!cardAutoMoving)
            CardImage.rectTransform.position = cardHomePosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectCard();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        DeselectCard();
    }
    public void OnPointerDown(PointerEventData eventData)
    { 
        CardImage.rectTransform.localScale *= zoomMultiplier;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(ColliderRect, eventData.position, eventData.pressEventCamera, out Vector3 cardClickPosition))
        {
            Cursor.visible = false;
            cardClickPosition = new Vector3(cardClickPosition.x, cardClickPosition.y, ColliderRect.position.z);
            cardClickOffset = cardClickPosition - CardImage.rectTransform.position;
        }
    }
    void PlayCard()
    {
        cardBehaviour.Play();
        cardBehaviour.Discard();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        ColliderRect.sizeDelta = defaultColliderSize;
        CardImage.rectTransform.localScale = defaultCardScale;
        //if (cardBehaviour.IsSingleTargetCard())
        //{
        //    ReturnCardToSlot();
        //}
        //else
        //    PlayCard();

        ReturnCardToSlot();
        Cursor.visible = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(ColliderRect, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
        {
            CardImage.rectTransform.position = globalMousePos - cardClickOffset;
        }
    }

    public void SetPosition(Vector2 pos)
    {
        ColliderRect.position = pos;
        CardImage.rectTransform.position = pos;
        SetHomePositionToCurrent();
    }

    public void SetHomePositionToCurrent()
    {
        cardHomePosition = CardImage.rectTransform.position;
    }
    void ReturnCardToSlot()
    {
        StartCoroutine(AutoMoveCardToPos(cardHomePosition));
        CardCanvas.sortingOrder = DefaultSortingOrder;
    }
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
    public IEnumerator GrowShrink(float scale)
    {
        Vector2 newScale = defaultCardScale * scale;
        while (true)
        {
            Vector2 difference = newScale - (Vector2)ColliderRect.localScale;
            if (Vector2.Distance(newScale, ColliderRect.localScale) > .1f)
            {
                ColliderRect.localScale =(Vector2)ColliderRect.localScale + (difference * Time.deltaTime * growShrinkSpeed);
                yield return null;
            }        
            else
            {
                ColliderRect.localScale = newScale;
                yield break;
            }
        }
    }
    public void Discard(DiscardPile dp)
    {
        StartCoroutine(GrowShrink(.5f));
    }
}
