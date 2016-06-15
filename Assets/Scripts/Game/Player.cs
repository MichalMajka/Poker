using UnityEngine;
using System.Collections.Generic;
using System;

public class Player {

    GameScript game = Camera.main.GetComponent<GameScript>();
    public NetworkPlayer owner;

    string state = "Pass";
    string name = "Player";
    Card[] hand = new Card[2];
    Money money;
    Money bet;

    public void NewGame()
    {
        state = "InGame";
    }

    public Player(NetworkPlayer player)
    {
        owner = player;
        money = new Money("Player", GameScript.changeBlueprints);
        bet = new Money("Empty", GameScript.changeBlueprints);
    }

    public string State
    {
        get { return state; }
        set { state = value; }
    }

    public void Activate()
    {
        throw new NotImplementedException();
    }

    public List<Card> takeCards()
    {
        List<Card> ret = new List<Card>(hand);
        hand = new Card[2];
        return ret;
    }

    internal void NextTurn()
    {
        if (!(Equals(state , "Pass") || Equals(state, "AllIn"))) state = "InGame";
    }

    public Money GetBet()
    {
        Money ret = new Money(bet.ToString(), GameScript.changeBlueprints);
        bet = new Money("Empty", GameScript.changeBlueprints);
        return ret;
    }

    internal void TakeAward(Money money)
    {
        this.money += money;
    }

    public Hand ChooseBestHand(List<Card> dealerCards)
    {
        List<Card> allCards = new List<Card>(hand);
        allCards.AddRange(dealerCards);
        return HandChecker.BestHand(allCards);
    }

    internal void GiveCard(Card card)
    {
        if (hand[0] == null)
            hand[0] = card;
        else
            hand[1] = card;
    }

    public string[] GetCards()
    {
        string[] ret = new string[2];
        try {
            ret[0] = hand[0].ToString();
            ret[1] = hand[1].ToString();
        }
        catch {
            ret[0] = "00";
            ret[1] = "00";
        }
        finally {
        }
        return ret;
    }

    public int Bet
    {
        get { return bet.Value(); }
    }

    public int Money
    {
        get { return money.Value(); }
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public string MoneyInString()
    {
        return money.ToString();
    }

    public string BetInString()
    {
        return bet.ToString();
    }

    public void BigBlind()
    {
        if(money.Value()>=2* GameScript.changeBlueprints[1].Value)
        {
            money -= new Money("0 2 0 0 0 0", GameScript.changeBlueprints);
            bet += new Money("0 2 0 0 0 0", GameScript.changeBlueprints);
        }
        else
            AllIn();
    }

    public void SmallBlind()
    {
        if (money.Value() >= GameScript.changeBlueprints[1].Value)
        {
            money -= new Money("0 1 0 0 0 0", GameScript.changeBlueprints);
            bet += new Money("0 1 0 0 0 0", GameScript.changeBlueprints);
        }
        else
            AllIn();
    }

    public void AllIn()
    {
        state = "AllIn";
        bet = money;
        money = new Money("Empty", GameScript.changeBlueprints);
    }

    public void Pass()
    {
        state = "Pass";
    }

    public void Check(int value)
    {
        if (value >= money.Value())
            AllIn();
        else
        {
            state = "Check";
            money += bet;
            Money checkingValue = money.CountChanges(value);
            money -= checkingValue;
            bet = checkingValue;
        }
    }

    public void Raise(string raiseValue)
    {
        Money raise = new Money(raiseValue, GameScript.changeBlueprints);
        if (raise.Value() > (money + bet).Value())
            AllIn();
        else if (raise.Value() == (money + bet).Value())
            Check(raise.Value());
        else
        {
            state = "Raised";
            money += bet;
            bet = raise;
            money -= bet;
        }
    }
}
