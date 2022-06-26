using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntrancePortal : MonoBehaviour
{
    public enum PortalType
    {
        SameScene, DifferentScene
    }

    [Header("Portal Info")]

    public string connectSceneName;

    public PortalType portalType;

    public DestinationPortal.DestinationTag destinationTag;

    private bool isPortalEffecting;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPortalEffecting)
        {
            //´«ËÍ
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPortalEffecting = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPortalEffecting = false;
        }
    }
}
