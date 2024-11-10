using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Application = UnityEngine.Application;

public class MenuBehavior : MonoBehaviour
{
    private const string GAME_SCENE = "game";
    private const string MENU_SCENE = "menu";
    
    [SerializeField] private Button _beginButton;
    [SerializeField] private Button _leaveButton;
    
    void Start()
    {
        _beginButton.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(GAME_SCENE);

            SceneManager.UnloadSceneAsync(MENU_SCENE);
        });
        
        _leaveButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
