using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniTutorial : MonoBehaviour
{
    [SerializeField] private GameObject content;
    
    public static MiniTutorial Get()
    {
        return FindObjectOfType<MiniTutorial>();
    }

    public void Show()
    {
        content.SetActive(true);    
    }

    public void Hide()
    {
        content.SetActive(false);
    }
}
