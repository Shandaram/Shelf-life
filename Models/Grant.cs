using UnityEngine;

[System.Serializable]
public class Grant : ScriptableObject
{
    public string grantName;
    public string description;
    public string objective;
    public string location;
    public string tier;
    public int pepRequired;
    public int repRequired;
    public int coinsRequired;
    public int bookCountRequired;
    public string bookGenreRequired;
    public int bookGenreCountRequired;
    public string bookGenreRequired2;
    public int bookGenreCountRequired2;
    public string repairTypeRequired;
    public int repairCountRequired;
    public string upgradeTypeRequired;
    public int upgradeCountRequired;
    public string authorNameRequired;
    public int authorCountRequired;
    public int rewardCoins;
    public string rewardType;
    public int rewardCount;
    public string funded;
    public bool received;
}