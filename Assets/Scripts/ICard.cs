using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this interface will control all of our cards. every unique card behavior script we make will
//  derive from ICard and must implement these methods.
//also this interface can be in any file, i just made a seperate file for now, because whatever.
public interface ICard
{

    int Damage();
    bool SingleTargetCard();
    string CardTag();
    void Play();
    void Discard();
}
