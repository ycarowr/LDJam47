using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private Button buttonContinue;
    [SerializeField] private string nextScene;

    private void Awake()
    {
        buttonContinue.onClick.AddListener(OnPressContinue);
        Hide();
    }

    private void OnPressContinue()
    {
        SceneManager.LoadScene(nextScene);
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
