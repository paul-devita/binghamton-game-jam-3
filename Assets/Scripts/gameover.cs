using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameover : MonoBehaviour
{
    private const string GAME_SCENE = "game";
    private const string OVER_SCENE = "end";
    
    [SerializeField] private Button _againButton;
    [SerializeField] private Button _leaveButton;
    
    void Start()
    {
        _againButton.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(GAME_SCENE);

            SceneManager.UnloadSceneAsync(OVER_SCENE);
        });
        
        _leaveButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
