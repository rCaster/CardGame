using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Blank", order = 1)]
public class Card : ScriptableObject
{
    public string cardName;
    public int cardCost;
    public Sprite sprite;

    [TextArea(15, 20)]
    public string description;

}
