using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class GameScript : MonoBehaviour {

    private delegate void Timer();

    NetworkScript network;

    bool gameRunning = false;
    public bool allowNewGame = false;

    public Players players = new Players();
    DealerCards dealerCards = new DealerCards();
    Deck mainDeck = new Deck();
    static public Change[] changeBlueprints = new Change[5];
    Money mainPile;
    public float timer = 0f;
    Timer counter;

    public UIScript ui;

    // Use this for initialization
    void Start () {
        Network.isMessageQueueRunning = true;
        network = gameObject.GetComponent<NetworkScript>();
        counter = TimerOff;
        changeBlueprints[0] = new Change(1);
        changeBlueprints[1] = new Change(5);
        changeBlueprints[2] = new Change(10);
        changeBlueprints[3] = new Change(25);
        changeBlueprints[4] = new Change(100);
        if (Network.isServer)
        {
            mainPile = new Money("Empty", this.ChangeBlueprints);
            players.Add(new Player(Network.player));
            players[0].Name = PlayerPrefs.GetString("Nickname");
            network.RPCSetPlayersMoney(Network.player, players.GetPlayer(Network.player).MoneyInString());
        }
}

    void Update()
    {
        counter();
        if (players.AllReady())
            allowNewGame = true;
        if (allowNewGame && !gameRunning && Network.isServer && players.Count>1)
        {
            counter = TimerOff;
            allowNewGame = false;
            NewGame();
        }
    }

    public void NewGame()
    {
        gameRunning = true;
        network.ResetTable();
        players.NewGame(network);
        dealerCards.GiveCards(mainDeck);
        foreach(Player player in players)
        {
            player.GiveCard(mainDeck.TakeCard());
            player.GiveCard(mainDeck.TakeCard());
        }
        foreach(Player player in players)
        {
            string[] cardsOfPlayer = player.GetCards();
            network.LoadHand(player, cardsOfPlayer[0], cardsOfPlayer[1]);
        }
        network.PlayerActivate(players.actual);
        return;
    }

    public void Decision(NetworkPlayer player, string decision)
    {
        if(players.actual.owner == player)
        {
            switch (decision)
            {
                case "Pass":
                    players.Pass();
                    break;

                case "Check":
                    players.Check(players.HighestBet());
                    break;

                default:
                    players.Raise(decision);
                    break;
           }
           BetDone(players.actual);
        }
        return;
    }

    void NextTurn()
    {
        players.NextTurn();
        network.PlayerActivate(players.actual);
    }

    private void EndTurn()
    {
        TakeBets();
        network.SetPile(mainPile.ToString());
        dealerCards.UncoverNext();
        network.SetDealerCards(dealerCards.CardsToString());
        if (dealerCards.FullHand())
            EndGame(null);
        else
            NextTurn();
        foreach(Player player in players) {
            network.SetPlayersMoney(player, player.MoneyInString());
            network.SetPlayersBet(player, player.BetInString());
        }

        return;
    }

    private void EndGame(Player winner)
    {
        if (winner==null)
        {
            List<Player> winners = new List<Player>();
            winners = players.Winner(dealerCards.whatCards());
            GiveAward(winners);
            foreach(Player player in winners)
            {
                network.SetWinner(player.owner);
                foreach (Player player2 in players)
                    network.SetWinningCards(player2, player, player.GetCards()[0], player.GetCards()[1]);
            }
        }
        else
        {
            GiveAward(winner);
            network.SetWinner(winner.owner);
            foreach (Player player in players)
                network.SetWinningCards(player,winner, winner.GetCards()[0], winner.GetCards()[1]);
        }
        foreach (Player player in players)
        {
            List<Card> takeCards = player.takeCards();
            if (takeCards.Count > 0)
                mainDeck.Add(takeCards);
        }
        foreach(Player player in players) {
            network.SetPlayersMoney(player, player.MoneyInString());
            network.SetPlayersBet(player, player.BetInString());
        }
        mainDeck.Add(dealerCards.takeCards());
        mainDeck.Shuffle();
        timer = 30f;
        gameRunning = false;
        counter = TimerOn;
        return;
    }
    
    private void GiveAward(List<Player> winners)
    {
        Money awards;
        awards = mainPile.SplitAward(winners.Count);
        foreach(Player winner in winners)
        {
            winner.TakeAward(awards);
        }
        mainPile = new Money("Empty", this.ChangeBlueprints);
    }

    void BetDone(Player player)
    {
        network.SetPlayersMoney(player,player.MoneyInString());
        network.SetPlayersBet(player, player.BetInString());
        Player winner = players.Winner();
        if (winner != null)
        {
            TakeBets();
            network.SetPile(mainPile.ToString());
            EndGame(winner);
            return;
        }
        if (players.NextPlayer())
        {
            network.PlayerActivate(players.actual);
        }
        else
        {
            EndTurn();
        }
            
        return;
    }

    public void AddPlayer(NetworkPlayer player)
    {
        if(gameRunning)
            players.playerCounter++;
        players.Add(new Player(player));
    }

    public void RemovePlayer(NetworkPlayer player) {
        players.playerCounter--;
        Player playerToRemove = players.GetPlayer(player);
        playerToRemove.Pass();
        Money betOfPlayerToRemove = playerToRemove.GetBet();
        mainPile += betOfPlayerToRemove;
        network.SetPile(mainPile.ToString());
        List<Card> takeCards = playerToRemove.takeCards();
        if(takeCards.Count > 0)
            mainDeck.Add(takeCards);
        if(players.Count < 3) {
            players.Remove(playerToRemove);
            allowNewGame = false;
            if(gameRunning) {
                TakeBets();
                EndGame(players[0]);
                counter = TimerOff;
                ui.ClearUI();
            }
        }
        else {
            if(UnityEngine.Object.Equals(players.actual, playerToRemove)) {
                if(players.NextPlayer()) {
                    players.Remove(playerToRemove);
                    network.PlayerActivate(players.actual);
                }
                else {
                    players.Remove(playerToRemove);
                    EndTurn();
                }
            }
            else {
                players.Remove(playerToRemove);
                Player winner = players.ThereIsWinner();
                if(winner != null) {
                    EndGame(winner);
                }
            }
        }
    }

    public Change[] ChangeBlueprints
    {
        get { return changeBlueprints; }
    }

    private void TakeBets()
    {
        mainPile += players.GetBet(this);
    }

    private void GiveAward(Player winner)
    {
        winner.TakeAward(mainPile);
        mainPile = new Money("Empty", GameScript.changeBlueprints);
    }

    private void TimerOff()
    {

    }

    private void TimerOn()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
            allowNewGame = true;
        else
            allowNewGame = false;
    }

    public void PlayerReady()
    {
        players.PlayerReady();
    }

    public void PlayerNotReady()
    {
        players.PlayerNotReady();
    }
}
