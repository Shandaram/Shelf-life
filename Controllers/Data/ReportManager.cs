using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class ReportManager : MonoBehaviour
{
    public Transform contentParent; // The parent of the grid reports (a GridLayoutGroup)
    public GameObject detailViewPrefab;
    public CSVReportLoader csvLoader;
    private List<Report> reports;

    void Start()
    {
        reports = csvLoader.LoadReports();
        if (reports != null && reports.Count > 0)
        {
            PopulateInventory();
        }
    }

    void PopulateInventory()
    {
        foreach (Report report in reports)
        {
            GameObject reportPage = Instantiate(detailViewPrefab, contentParent);
            reportPage.transform.Find("Panel1/VerGroup1/ReportType").GetComponent<TMP_Text>().text = report.type;
            reportPage.transform.Find("VerGroup3/ReportDescription").GetComponent<TMP_Text>().text = report.description;
            reportPage.transform.Find("VerGroup5/ReportFollowup").GetComponent<TMP_Text>().text = report.followup;
            reportPage.transform.Find("Panel3/VerGroup1/ReportPatron").GetComponent<TMP_Text>().text = report.patron;
            reportPage.transform.Find("Panel2/VerGroup1/ReportDate").GetComponent<TMP_Text>().text = report.date;
       
        }
    }
}
