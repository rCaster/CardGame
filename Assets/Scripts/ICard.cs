using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard
{
    int Damage();
    bool IsSingleTargetCard();
    string CardTag();
    void Play();
    void Discard();
}
