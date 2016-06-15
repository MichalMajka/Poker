using UnityEngine;
using System.Collections;
using System;

public class NetworkScript : MonoBehaviour {
    string ServerRealName = "Redox_Poker_Server_09122015";
    string serverName;
    string nickname;
    public GameScript game;
    public NetworkView networkView;
    public UIScript ui;

    void Start() {
        MasterServer.ipAddress = "127.0.0.1";
        MasterServer.port = 23466;
        Network.natFacilitatorIP = "127.0.0.1";
        Network.natFacilitatorPort = 50005;
    }

    public HostData[] GetServerList() {
        HostData[] hostData;
        MasterServer.RequestHostList(ServerRealName);
        hostData = MasterServer.PollHostList();
        return hostData;
    }

    public void StartSever() {
        System.Random rng = new System.Random();
        Network.InitializeServer(8, rng.Next(1025, 5500), false);
        MasterServer.RegisterHost(ServerRealName.ToString(), serverName, "Standard poker server");
    }

    public void ConnectToServer(HostData server) {
        Network.Connect(server);
        Application.LoadLevel(1);
    }

    ///////////////////////////////////////////////////////////////////////////
    ////////////////////////Callbacks from server////////////////////////////
    //////////////////////////////////////////////////////////////////////////

    void OnServerInitialized() {
        Application.LoadLevel(1);
    }

    public void OnMasterServerEvent(MasterServerEvent masterServerEvent) {
        if(masterServerEvent == MasterServerEvent.RegistrationSucceeded)
            if(Application.loadedLevel != 2)
                Application.LoadLevel(2);
    }

    internal void PlayerActivate(Player actual) {
        if(actual.owner != Network.player)
            networkView.RPC("RPCPlayerActivate", actual.owner);
        else
            RPCPlayerActivate();
    }

    internal void LoadHand(Player actual, string firstCard, string secondCard) {
        if(actual.owner != Network.player)
            networkView.RPC("RPCLoadHand", actual.owner, firstCard, secondCard);
        else
            RPCLoadHand(firstCard, secondCard);
    }

    internal void SetWinningCards(Player actual, Player winner, string firstCard, string secondCard) {
        if(actual.owner != Network.player)
            networkView.RPC("RPCSetWinningCards", actual.owner, winner.owner, firstCard, secondCard);
        else
            RPCSetWinningCards(winner.owner, firstCard, secondCard);
    }

    public void SetPlayersMoney(Player player, string money) {
        networkView.RPC("RPCSetPlayersMoney", RPCMode.All, player.owner, money);
    }

    public void SetPlayersBet(Player player, string bet) {
        networkView.RPC("RPCSetPlayersBet", RPCMode.All, player.owner, bet);
    }

    public void PlayerPass() {
        if(!Network.isServer)
            networkView.RPC("RPCPlayerPass", RPCMode.Server, Network.player);
        else
            RPCPlayerPass(Network.player);
    }

    public void PlayerCheck() {
        if(!Network.isServer)
            networkView.RPC("RPCPlayerCheck", RPCMode.Server, Network.player);
        else
            RPCPlayerCheck(Network.player);
    }

    public void PlayerRaise(string raise) {
        if(!Network.isServer)
            networkView.RPC("RPCPlayerRaise", RPCMode.Server, Network.player, raise);
        else
            RPCPlayerRaise(Network.player, raise);
    }

    public void SetPile(string pile) {
        networkView.RPC("RPCSetPile", RPCMode.All, pile);
    }

    public void SetDealerCards(string cards) {
        networkView.RPC("RPCSetDealerCards", RPCMode.All, cards);
    }

    internal void SetWinner(NetworkPlayer winner) {
        ui.SetWinner(winner);
        networkView.RPC("RPCSetWinner", RPCMode.All, winner);
    }

    public void ResetTable() {
        networkView.RPC("RPCResetTable", RPCMode.All);
    }

    [RPC]
    public void RPCPlayerActivate() {
        ui.ActivatePlayer(true);
    }

    [RPC]
    public void RPCLoadHand(string firstCard, string secondCard) {
        ui.LoadHand(firstCard, secondCard);
    }

    [RPC]
    public void RPCSetWinningCards(NetworkPlayer winner, string firstCard, string secondCard) {
        ui.SetWinningCards(winner, firstCard, secondCard);
    }

    [RPC]
    public void RPCSetPlayersMoney(NetworkPlayer player, string money) {
        if(player == Network.player)
            ui.SetMoney(money);
        else
            ui.SetMoneyOfPlayer(player, money);
    }

    [RPC]
    public void RPCSetPlayersBet(NetworkPlayer player, string bet) {
        if(player != Network.player)
            ui.SetBetOfPlayer(player, bet);
        else
            ui.SetBet(bet);
    }

    [RPC]
    public void RPCSetReady() {
        game.PlayerReady();
    }

    [RPC]
    public void RPCSetNotReady() {
        game.PlayerNotReady();
    }

    [RPC]
    void RPCPlayerJoined(NetworkPlayer player, string name) {
        if(player != Network.player)
            ui.AddPlayer(player, name);
    }

    [RPC]
    void RPCPlayerLeft(NetworkPlayer player) {
        if(player != Network.player)
            ui.RemovePlayer(player);
    }

    [RPC]
    void RPCPlayerPass(NetworkPlayer player) {
        game.Decision(player, "Pass");
    }

    [RPC]
    void RPCPlayerRaise(NetworkPlayer player, string money) {
        game.Decision(player, money);
    }

    [RPC]
    void RPCPlayerCheck(NetworkPlayer player) {
        game.Decision(player, "Check");
    }

    [RPC]
    void RPCSetPile(string pile) {
        ui.SetPile(pile);
    }

    [RPC]
    void RPCSetDealerCards(string cards) {
        ui.SetDealerCards(cards);
    }

    [RPC]
    void RPCResetTable() {
        ui.ResetTable();
    }

    [RPC]
    void RPCSetNickname(NetworkPlayer player, string nickname) {
        ui.SetNickname(player, nickname);
    }

    [RPC]
    void RPCSetNameOnServer(NetworkPlayer player, string nickname) {
        Player added = game.players.GetPlayer(player);
        added.Name = nickname;
        networkView.RPC("RPCPlayerJoined", RPCMode.All, player, nickname);
        foreach(Player playerContainer in game.players) {
            if(playerContainer.owner != Network.player)
                networkView.RPC("RPCSetPlayersMoney", playerContainer.owner, player, added.MoneyInString());
            else
                RPCSetPlayersMoney(player, added.MoneyInString());
            networkView.RPC("RPCPlayerJoined", player, playerContainer.owner, playerContainer.Name);
            networkView.RPC("RPCSetPlayersMoney", player, playerContainer.owner, playerContainer.MoneyInString());
        }
        if(game.players.Count == 2 && Network.isServer)
            ui.ActivateNewGameButton();
    }

    [RPC]
    void RPCForceIdentification() {
        if(!Network.isServer)
            networkView.RPC("RPCSetNameOnServer", RPCMode.Server, Network.player, PlayerPrefs.GetString("Nickname"));
        else
            RPCSetNameOnServer(Network.player, PlayerPrefs.GetString("Nickname"));
    }

    [RPC]
    void RPCSetWinner(NetworkPlayer player) {
        ui.SetWinner(player);
    }

    public void Ready() {
        if(!Network.isServer)
            networkView.RPC("RPCSetReady", RPCMode.Server);
        else
            RPCSetReady();
    }

    public void NotReady() {
        if(!Network.isServer)
            networkView.RPC("RPCSetNotReady", RPCMode.Server);
        else
            RPCSetNotReady();
    }

    void OnPlayerConnected(NetworkPlayer player) {
        game.AddPlayer(player);
        networkView.RPC("RPCForceIdentification", player);
    }

    void OnPlayerDisconnected(NetworkPlayer player) {
        networkView.RPC("RPCPlayerLeft", RPCMode.All, player);
        game.RemovePlayer(player);
        if(game.players.Count < 2 && Network.isServer)
            ui.StartWaitingForNewGame();
    }

    void OnConnectedToServer() {
        Network.isMessageQueueRunning = false;
        Application.LoadLevel(2);
    }

    void OnFailedToConnectToMasterServer() {
        SceneManager.LoadLevelWithError(0, "Cannot establish the connection to Master Server.\nAre you sure of Your connection settings?.");
    }

    void OnFailedToConnect() {
        SceneManager.LoadLevelWithError(0, "Sorry, server is unreachable.");
    }

    void OnDisconnectedFromServer() {
        SceneManager.LoadLevelWithError(0, "You have been disconnected from the server.");
    }

    public string ServerName {
        get {
            return serverName;
        }
        set {
            serverName = value;
        }
    }

    public string Nickname {
        get {
            return nickname;
        }
        set {
            nickname = value;
            PlayerPrefs.SetString("Nickname", value);
        }
    }
}
