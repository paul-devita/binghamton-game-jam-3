using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.WSA;
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
            Scene gameScene = SceneManager.GetSceneByName(GAME_SCENE);
            Scene menuScene = SceneManager.GetSceneByName(MENU_SCENE);
            
            SceneManager.LoadSceneAsync(gameScene.name);

            SceneManager.SetActiveScene(gameScene);

            SceneManager.UnloadSceneAsync(menuScene);
        });
        
        _leaveButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
