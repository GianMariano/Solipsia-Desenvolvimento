using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public int money;
    public TMP_Text moneyText;
    
    
    void Start()
    {
        moneyText.text = "" + money;
    }
    public void UpdateMoney(int points)
    {
        money += points;
        moneyText.text = "" + money;
    }

}
