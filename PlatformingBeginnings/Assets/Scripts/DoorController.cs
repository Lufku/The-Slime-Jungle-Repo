using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject closedDoor;
    public GameObject openDoor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedDoor.SetActive(true);
        openDoor.SetActive(false);
    }
    public void OpenDoor()
    {
        closedDoor.SetActive(false);
        openDoor.SetActive(true);
    }
}
