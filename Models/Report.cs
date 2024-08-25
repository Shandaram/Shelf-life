using UnityEngine;

[System.Serializable]
public class Report : ScriptableObject
{
    public string type;
    public string description;
    public string followup;
    public string patron;
    public string date;
}