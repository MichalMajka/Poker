using System;
using System.Collections.Generic;

public class DealerCards
{
    List<Card> hand = new List<Card>();
    bool delay = true;

    public DealerCards()
    {

    }

    internal bool FullHand()
    {
        foreach(Card card in hand)
        {
            if (!card.IsUncovered)
                return false;
        }
        if (delay)
        {
            delay = false;
            return false;
        }

        return true;
    }

    internal List<Card> takeCards()
    {
        foreach (Card card in hand)
            card.Cover();
        List<Card> returnHand = hand;
        hand = new List<Card>();
        return returnHand;
    }

    internal void UncoverNext()
    {
        if (!hand[0].IsUncovered)
            for (int i = 0; i < 3; i++)
                hand[i].Uncover();
        else
            foreach(Card card in hand)
            {
                if(!card.IsUncovered)
                {
                    card.Uncover();
                    return;
                }
            }
    }

    internal List<Card> whatCards()
    {
        return hand;
    }

    internal string CardsToString()
    {
        string ret = "";
        for(int i=0;i<5 && hand[i].IsUncovered; i++)
        {
            ret += hand[i].ToString();
            if (i < 4 && hand[i + 1].IsUncovered)
                ret += " ";
        }
        return ret;
    }

    public void GiveCards(Deck deck)
    {
        delay = true;
        for (int i = 0; i < 5; i++)
            hand.Add(deck.TakeCard());
    }
}
