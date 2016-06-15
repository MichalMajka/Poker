using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

public class Deck {

    List<Card> cards = new List<Card>();

    public Deck()
    {
        for (int i = 0; i < 13; i++)
            for (int j = 0; j < 4; j++)
                cards.Add(new Card(j, i));
        Shuffle();
    }

    public void Shuffle()
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = cards.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    public void Add(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            if(!GameObject.Equals(card,null))
            {
                card.Cover();
                this.cards.Add(card);
            }
        }
    }

    public Card TakeCard()
    {
        Card taken = cards[0];
        cards.RemoveAt(0);
        return taken;
    }
}
