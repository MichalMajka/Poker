using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;

public class UIScript : MonoBehaviour {

    public GameObject playerBar;
    public GameObject array;
    public GameObject actionBar;
    public GameObject firstTurnBar;
    public GameObject nextTurnBar;
    public GameObject hand;
    public GameObject dealer;
    public GameObject changes;
    public GameObject pile;
    public GameObject bet;
    public GameObject winningCards;
    public GameObject winningBanner;
    public GameObject readyButton;
    public GameObject notReadyButton;
    public GameObject waitForTurnBanner;
    public GameObject betErrorBanner;
    public GameObject waitingForPlayersBanner;
    public GameObject waitingForNewGameBanner;
    public GameObject startGameButton;
    public Text messagePrefab;
    public GameObject messageHolder;
    public GameObject mainMenu;

    public NetworkScript network;
    public GameScript game;

    Dictionary<string, Sprite> cards = new Dictionary<string,Sprite>();
    Dictionary<NetworkPlayer, GameObject> players = new Dictionary<NetworkPlayer, GameObject>();

    string savedBet;
    string savedMoney;

    void Start()
    {
        foreach (Sprite card in Resources.LoadAll<Sprite>("Cards/"))
            cards.Add(card.name, card);
        if(!Network.isServer) {
            waitingForNewGameBanner.SetActive(true);
        }
        else {
            waitingForPlayersBanner.SetActive(true);
        }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape))
            mainMenu.SetActive(!mainMenu.active);
    }

    public void AddPlayer(NetworkPlayer player, string name)
    {
        Text newMessage = Instantiate(messagePrefab);
        newMessage.transform.SetParent(messageHolder.transform, false);
        newMessage.text = "Player " + name + " joined the game.";
        GameObject newPlayer = null;
        foreach(Transform child in array.transform)
        {
            if (!child.gameObject.activeInHierarchy)
            {
               newPlayer = child.gameObject;
               break;
            }
        }
        newPlayer.SetActive(true);
        newPlayer.GetComponentInChildren<Text>().text = name;
        players.Add(player,newPlayer);
    }

    public void RemovePlayer(NetworkPlayer player) {
        Text newMessage = Instantiate(messagePrefab);
        newMessage.transform.SetParent(messageHolder.transform, false);
        newMessage.text = "Player "+ players[player].GetComponentInChildren<Text>().text+" left the game.";
        players[player].SetActive(false);
        players.Remove(player);
    }

    public void SetNickname(NetworkPlayer player, string name)
    {
        if(player != Network.player) {
            players[player].GetComponentInChildren<Text>().text = name;
        }
    }

    internal void ActivatePlayer(bool state)
    {
        StartCoroutine("CoActivePlayer",state);
    }

    IEnumerator CoActivePlayer(bool state)
    {
        if(state==true)
            for(int i=0;i<5;i++)
                yield return new WaitForEndOfFrame();
        actionBar.SetActive(state);
        firstTurnBar.SetActive(state);
        waitForTurnBanner.SetActive(!state);
    }

    public void LoadHand(string firstCard, string secondCard)
    {
        waitingForNewGameBanner.SetActive(false);
        Image[] places = hand.GetComponentsInChildren<Image>();
        places[1].enabled = true;
        places[2].enabled = true;
        places[1].sprite = cards[firstCard];
        places[2].sprite = cards[secondCard];
    }

    internal void SetMoneyOfPlayer(NetworkPlayer player, string money)
    {
        if(players.ContainsKey(player))
            players[player].GetComponent<PlayerScript>().SetMoney(money);
    }

    internal void SetMoney(string money){
        changes.GetComponent<MoneyScript>().SetMoney(money);
    }

    internal void SetBetOfPlayer(NetworkPlayer player, string bet)
    {
        if(players.ContainsKey(player))
            players[player].GetComponent<PlayerScript>().SetBet(bet);
    }

    internal void SetBet(string money)
    {
        bet.GetComponent<MoneyScript>().SetMoney(money);
    }

    public void SetPile(string money)
    {
        pile.GetComponent<MoneyScript>().SetMoney(money);
    }

    public void SetDealerCards(string cards)
    {
        Image[] places = dealer.GetComponentsInChildren<Image>();
        string[] images = cards.Split(' ');
        for(int i=1;i<images.Length+1 && i<places.Length;i++)
        {
            places[i].enabled = true;
            places[i].sprite = this.cards[images[i-1]];
        }
    }

    public void ResetTable()
    {
        Image[] places = hand.GetComponentsInChildren<Image>();
        places[1].enabled = false;
        places[2].enabled = false;
        places = dealer.GetComponentsInChildren<Image>();
        for (int i = 1; i < places.Length; i++)
            places[i].enabled = false;
        SetPile("0 0 0 0 0");
        winningCards.SetActive(false);
        readyButton.SetActive(false);
        notReadyButton.SetActive(false);
        winningBanner.SetActive(false);
    }

    public void SetWinner(NetworkPlayer winner)
    {
        actionBar.SetActive(false);
        if(winner != Network.player)
            players[winner].GetComponent<PlayerScript>().SetWinner();
        else
            winningBanner.SetActive(true);
    }

    public void SetWinningCards(NetworkPlayer winner, string firstCard, string secondCard)
    {
        if(winner != Network.player)
        {
            winningCards.SetActive(true);
            Image[] places = winningCards.GetComponentsInChildren<Image>();
            places[1].enabled = true;
            places[2].enabled = true;
            places[1].sprite = cards[firstCard];
            places[2].sprite = cards[secondCard];
        }
        else
        {
            winningBanner.SetActive(true);
        }
        readyButton.SetActive(true);
        notReadyButton.SetActive(false);
        waitForTurnBanner.SetActive(false);
    }

    //Button functions

    public void PlayerPass()
    {
        network.PlayerPass();
        ActivatePlayer(false);
    }

    public void PlayerCheck()
    {
        network.PlayerCheck();
        ActivatePlayer(false);
    }

    public void PlayerRaise()
    {
        network.PlayerRaise(bet.GetComponent<MoneyScript>().GetMoney());
        ActivatePlayer(false);
    }

    public void SetReady()
    {
        network.Ready();
    }

    public void SetNotReady()
    {
        network.NotReady();
    }

    public void SavePreviousBet() {
        savedBet = bet.GetComponent<MoneyScript>().GetMoney();
        savedMoney = changes.GetComponent<MoneyScript>().GetMoney();
    }

    public void PlayerTriesToRise() {
        Money actualBet = new Money(bet.GetComponent<MoneyScript>().GetMoney(), GameScript.changeBlueprints);
        foreach(KeyValuePair<NetworkPlayer, GameObject> entry in players) {
            Money moneyToCompare = new Money(entry.Value.GetComponentsInChildren<MoneyScript>()[1].GetMoney(), GameScript.changeBlueprints);
            if(actualBet.Value() < moneyToCompare.Value()) {
                bet.GetComponent<MoneyScript>().SetMoney(savedBet);
                changes.GetComponent<MoneyScript>().SetMoney(savedMoney);
                betErrorBanner.SetActive(true);
                actionBar.SetActive(true);
                firstTurnBar.SetActive(true);
                return;
            }
        }
        PlayerRaise();
    }

    public void ActivateNewGameButton() {
        startGameButton.SetActive(true);
        waitingForPlayersBanner.SetActive(false);
    }

    public void OnNewGameButtonClicked() {
        game.allowNewGame = true;
        startGameButton.SetActive(false);
    }

    internal void StartWaitingForNewGame() {
        game.allowNewGame = false;
        waitingForPlayersBanner.SetActive(true);
    }

    public void ClearUI() {
        ResetTable();
        actionBar.SetActive(false);
    }

    public void BackToMainMenu() {
        Network.Disconnect();
        Application.LoadLevel(0);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
