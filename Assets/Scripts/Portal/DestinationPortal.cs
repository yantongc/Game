using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationPortal : MonoBehaviour
{
    public enum DestinationTag
    {
        Enter, RoomDoor, A, B, C
    }

    public DestinationTag destinationTag;
}
