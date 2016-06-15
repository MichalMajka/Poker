using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

    public GameObject name;
    public GameObject money;
    public GameObject bet;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMoney(string moneyToSet)
    {
        money.GetComponent<MoneyScript>().SetMoney(moneyToSet);
    }

    public void SetBet(string moneyToSet)
    {
        bet.GetComponent<MoneyScript>().SetMoney(moneyToSet);
    }

    public void SetWinner()
    {
        animator.SetBool("Winner", true);
    }
}
