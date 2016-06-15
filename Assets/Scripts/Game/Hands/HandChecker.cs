using System;
using System.Collections.Generic;
using UnityEngine;

public static class HandChecker
{
    public static Hand BestHand(List<Card> cards)
    {
        Hand toReturn = null;
        cards.Sort(Card.CompareByHierarchy);
        toReturn = isFlush(cards);
        if (toReturn == null)
            toReturn = isStraight(cards);
        if (toReturn == null)
            toReturn = isFour(cards);
        return toReturn;
    }
    private static Hand isStraight(List<Card> cards)
    {
        List<Card> temp = new List<Card>();
        temp.AddRange(cards);
        for(int i=0;i<temp.Count-2;)
        {
            if (temp[i].Hierarchy == temp[i + 1].Hierarchy)
            {
                temp.RemoveAt(i);
            }
            else
                i++;
        }

        for (int i = 0; i < temp.Count - 1 && i<5;)
        {
            if (temp[i].Hierarchy - 1 != temp[i + 1].Hierarchy)
            {
                for (int j = 0; j < i + 1; j++)
                    temp.RemoveAt(0);
                i = 0;
            }
            else
                i++;
        }

        while (temp.Count > 5)
        {
            temp.RemoveAt(temp.Count - 1);
        }
        if (temp.Count < 5)
        {
            if (temp.Count < 4)
            {
                return null;
            }
            else
            {
                if (temp[temp.Count - 1].Hierarchy == 0 && cards[0].Hierarchy == 12)
                    return new Straight(3);
                else
                    return null;
            }
        }
        return new Straight(temp[0].Hierarchy);
    }

    private static Hand isFlush(List<Card> cards)
    {
        List<Card> temp = new List<Card>();
        temp.AddRange(cards);
        int[] cardOfColour = new int[4];
        foreach (Card card in temp)
        {
            cardOfColour[card.Colour]+=1;
        }
        bool isFlush = false;
        int trueColor = 0;
        for (int i = 0; i < 4; i++)
        {
            if (cardOfColour[i] >= 5)
            {
                isFlush = true;
                trueColor = i;
            }
        }
        if (isFlush)
        {
            for (int i = 0; i < temp.Count;)
            {
                if (temp[i].Colour != trueColor)
                    temp.RemoveAt(i);
                else
                    i++;
            }
            while(temp.Count>5)
            {
                temp.RemoveAt(temp.Count - 1);
            }
            int hierarchy = 0;
            foreach (Card card in temp)
                if (card.Colour == trueColor && card.Hierarchy > hierarchy) hierarchy = card.Hierarchy;
            if (isStraight(temp)!=null)
                return new StraightFlush(hierarchy);
            else
                return new Flush(hierarchy);
        }
        else
            return null;
    }

    private static Hand isFour(List<Card> cards)
    {
        int[] combinations = new int[13];
        foreach(Card card in cards)
        {
            combinations[card.Hierarchy] += 1;
        }
        Pair bestPair = null;
        Pair secondPair = null;
        Triple bestTriple = null;
        Four bestFour = null;
        for(int i=12;i>=0;i--)
        {
            switch (combinations[i])
            {
                case 2:
                    if (bestPair == null)
                        bestPair = new Pair(i);
                    else if (secondPair == null)
                        secondPair = new Pair(i);
                    break;
                case 3:
                    if (bestTriple == null)
                        bestTriple = new Triple(i);
                    break;
                case 4:
                    if (bestFour == null)
                        bestFour = new Four(i);
                    break;
                default:
                    break;
            }
        }

        if (bestFour != null)
        {
            int highest=0;
            foreach (Card card in cards)
            {
                if (card.Hierarchy != bestFour.CardPower && card.Hierarchy > highest)
                    highest = card.Hierarchy;
            }
            bestFour.LooseCardPower = highest;
            return bestFour;
        }
        if(bestTriple != null)
        {
            if (bestPair != null)
                return new FullHouse(bestTriple, bestPair);
            else
            {
                int[] minorCards = new int[2];
                int j = 0;
                for(int i=12;i>=0 && j<2;)
                {
                    if(combinations[i]>0 && i!=bestTriple.CardPower)
                    {
                        minorCards[j++] = i;
                        combinations[i]--;
                    }
                    else
                    {
                        i--;
                    }
                }
                bestTriple.LooseCardsPower = minorCards;
                return bestTriple;
            }
        }
        if(secondPair != null)
        {
            int highest = 0;
            foreach (Card card in cards)
            {
                if (card.Hierarchy != bestPair.CardPower && card.Hierarchy != secondPair.CardPower && card.Hierarchy > highest)
                    highest = card.Hierarchy;
            }
            TwoPairs twoPairs = new TwoPairs(bestPair, secondPair);
            twoPairs.LooseCardPower = highest;
            return twoPairs;
        }
        if(bestPair != null)
        {
            int[] minorCards = new int[3];
            int j = 0;
            for (int i = 12; i >= 0 && j < 3;)
            {
                if (combinations[i] > 0 && i != bestPair.CardPower)
                {
                    minorCards[j++] = i;
                    combinations[i]--;
                }
                else
                {
                    i--;
                }
            }
            bestPair.LooseCardsPower = minorCards;
            return bestPair;
        }
        One bestCard = new One();
        int[] highestCards = new int[5];
        for (int i = 0; i < 5; i++)
            highestCards[i] = cards[i].Hierarchy;
        return bestCard;
    }
}
