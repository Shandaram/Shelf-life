using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform mainCameraPosition; // Assign in Inspector
    public Transform customerViewPosition; 
    public Transform deskViewPosition; 
    public Transform loadingViewPosition; 
    public Transform PCViewPosition; 

    void Start()
    {
        SwitchToMainCamera();
    }

    public void SwitchToMainCamera()
    {
        Camera.main.transform.position = mainCameraPosition.position;
        Camera.main.transform.rotation = mainCameraPosition.rotation;
    }

    public void SwitchToCustomerView()
    {
        Camera.main.transform.position = customerViewPosition.position;
        Camera.main.transform.rotation = customerViewPosition.rotation;
    }

       public void SwitchToDeskView()
    {
        Camera.main.transform.position = deskViewPosition.position;
        Camera.main.transform.rotation = deskViewPosition.rotation;
    }

        public void SwitchToLoadingView()
    {
        Camera.main.transform.position = loadingViewPosition.position;
        Camera.main.transform.rotation = loadingViewPosition.rotation;
    }
      public void SwitchToPCView()
    {
        Camera.main.transform.position = PCViewPosition.position;
        Camera.main.transform.rotation = PCViewPosition.rotation;
    }
 
}

