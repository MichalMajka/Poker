
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Players : List<Player>
{
    int blindIndex = 0;
    public Player actual;
    public Player first;
    int readyCounter = 0;
    public int playerCounter = 10;

    public void NewGame(NetworkScript network)
    {
        playerCounter = Count;
        blindIndex = (blindIndex + 1) % Count;
        this[blindIndex].SmallBlind();
        this[(blindIndex+1)%Count].BigBlind();
        network.SetPlayersMoney(this[blindIndex], this[blindIndex].MoneyInString());
        network.SetPlayersBet(this[blindIndex], this[blindIndex].BetInString());
        network.SetPlayersMoney(this[(blindIndex + 1) % Count], this[(blindIndex + 1) % Count].MoneyInString());
        network.SetPlayersBet(this[(blindIndex + 1) % Count], this[(blindIndex + 1) % Count].BetInString());
        foreach (Player player in this)
            player.NewGame();
        first = this[blindIndex];
        actual = this[blindIndex];
        readyCounter = 0;
    }

    public bool NextPlayer()
    {
        
        if (AllAreChecking())
        {
            return false;
        }
        else
        {
            foreach (Player player in IterateFrom(actual))
            {
                if (Equals(player.State, "InGame") || Equals(player.State, "Raised"))
                {
                    actual = player;
                    return true;
                }
            }
        }
        return false;
    }

    bool AllAreChecking()
    {
        int maxBet = HighestBet();
        if (maxBet != 0)
        {
            foreach (Player player in this)
            {
                if (!PlayerInactive(player))
                {
                    if (player.Bet != maxBet)
                        return false;
                }
            }
            return true;
        }
        else
        {
            foreach (Player player in this)
            {
                if (!Equals(player.State, "Check"))
                {
                    return false;
                }
            }
            return true;
        }            
    }

    bool PlayerInactive(Player player)
    {
        if (Equals(player.State, "Pass") || Equals(player.State, "AllIn"))
            return true;
        else
            return false;
    }

    internal void Check(int v)
    {
        actual.Check(v);
    }

    internal void Raise(string decision)
    {
        Money decisionAsMoney = new Money(decision, GameScript.changeBlueprints);
        if(decisionAsMoney.Value() == HighestBet())
            Check(HighestBet());
        else {
            foreach(Player player in IterateFrom(actual))
                if(!PlayerInactive(player))
                    player.State = "InGame";
            actual.Raise(decision);
        }
    }

    internal void Pass()
    {
        actual.Pass();
    }

    IEnumerable<Player> IterateFrom(Player player)
    {
        int index = IndexOf(player);
        for (int i = (index+1)%Count; i != index;i=i%Count)
        {
            yield return this[i++];
        }
        yield return this[index];
    }

    internal void NextTurn()
    {
        foreach (Player player in this)
            player.NextTurn();
        actual = first;
        if (PlayerInactive(actual))
            NextPlayer();
    }

    internal Player Winner()
    {
        Player winner = null;
        foreach(Player player in this)
        {
            if (!String.Equals(player.State,"Pass"))
            {
                if (winner != null)
                {
                    winner = null;
                    return winner;
                }
                winner = player;
            }
        }
        return winner;
    }

    public Money GetBet(GameScript game)
    {
        Money bet = new Money("Empty", GameScript.changeBlueprints);
        foreach(Player player in this)
        {
            bet += player.GetBet();
        }
        return bet;
    }

    internal List<Player> Winner(List<Card> cards)
    {
        List<Player> winners = new List<Player>();
        List<Player> stillPlaying = new List<Player>();
        foreach (Player player in this)
            if (player.State != "Pass") stillPlaying.Add(player);
        List<Hand> handsOfPlayers = new List<Hand>();
        foreach (Player player in stillPlaying)
            handsOfPlayers.Add(player.ChooseBestHand(cards));
        Hand bestHand = handsOfPlayers[0];
        winners.Add(stillPlaying[0]);
        for(int i=1;i<handsOfPlayers.Count;i++)
        {
            if(Hand.Compare(bestHand,handsOfPlayers[i])<0)
            {
                bestHand = handsOfPlayers[i];
                winners = new List<Player>();
                winners.Add(stillPlaying[i]);
            }
            else if(Hand.Compare(bestHand,handsOfPlayers[i]) == 0)
            {
                winners.Add(stillPlaying[i]);
            }
        }
        return winners;
    }

    public Player GetPlayer(NetworkPlayer networkPlayer)
    {
        foreach (Player player in this)
            if (player.owner == networkPlayer)
                return player;
        return null;
    }

    public int HighestBet()
    {
        int ret = 0;
        foreach (Player player in this)
            if (player.Bet > ret)
                ret = player.Bet;
        return ret;
    }

    public void PlayerReady()
    {
        readyCounter += 1;
    }

    public void PlayerNotReady()
    {
        readyCounter -= 1;
    }

    public bool AllReady()
    {
        return readyCounter==playerCounter;
    }

    public Player ThereIsWinner() {
        Player playerThatDidntPass = null;
        foreach(Player player in this) {
            if(!PlayerInactive(player))
                if(playerThatDidntPass != null)
                    return null;
                else
                    playerThatDidntPass = player;
        }
        return playerThatDidntPass;
    }
}