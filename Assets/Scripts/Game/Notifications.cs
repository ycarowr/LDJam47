using UnityEngine;
using Random = UnityEngine.Random;

public class Notifications : MonoBehaviour
{
    [SerializeField] private GameObject[] notifications;

    public static Notifications Get()
    {
        return FindObjectOfType<Notifications>();
    }
    
    private void Awake()
    {
        HideAll();
    }

    public void ShowRandom()
    {
        var random = Random.Range(0, notifications.Length);
        Show(random);
    }

    public void HideAll()
    {
        foreach (var notification in notifications) 
            notification.SetActive(false);
    }

    private void Show(int index)
    {
        notifications[index].SetActive(true);
    }
}
