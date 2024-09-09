using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour
{
    private DefaultInputActions _input;
    [SerializeField]
    private GameObject _titleScreen;
    [SerializeField]
    private GameObject _pauseScreen;
    [SerializeField]
    private GameObject _helpScreen;
    [SerializeField]
    private GameObject _winScreen;
    [SerializeField]
    private GameObject _looseScreen;
    [SerializeField]
    private GameObject _buttons;
    
    [SerializeField]
    private TextMeshProUGUI _livesText;
    [SerializeField]
    private TextMeshProUGUI _levelText;
    [SerializeField]
    private TextMeshProUGUI _blocksLeftText;
    private GameObject _levelHolder;

    [SerializeField]
    private GameObject[] _levels;
    
    private int _level = 1;
    private int _lives = 3;
    private int _blocksLeft = 0;
    private  bool _waitingForNextLevel = false;
    
    public bool IsOnPause { get; private set; }
    
    void Start()
    {
        if(_input == null)
        {
            _input = new DefaultInputActions();
            _input.Enable();
            _input.DebugInputMap.Restart.performed += OnRestart;
        }
        
        _levelHolder = GameObject.Find("CurrentLevel");
        StartGame();
        IsOnPause = true;

    }
    
    void Update()
    {
        if(_waitingForNextLevel)
        {
            SetLevel(_level + 1);
            _waitingForNextLevel = false;
        }
    }

    private void OnRestart(InputAction.CallbackContext ctx)
    {
        Restart();
    }
    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
        Debug.Log("Restart");
    }

    private void StartGame()
    {
        SetLevel(1);
        SetLives(3);
        SetBlockCount();
    }
    
    private void SetLevel(int level)
    {
        _level = level;
        _levelText.text = "Level: " + _level;
        if (level <= _levels.Length)
        {
            Instantiate(_levels[level - 1], _levelHolder.transform);
            SetBlockCount();
            SetLives(3);
        }
        else
        {
            _winScreen.SetActive(true);
        }
    }

    private void SetLives(int lives)
    {
        _lives = lives;
        _livesText.text = "Lives: " + _lives;
    }

    private void SetBlockCount()
    {
        _blocksLeft = GameObject.FindGameObjectsWithTag("MustBeDestroyed").Length;
        UpdateBlockCount();
        
    }

    private void UpdateBlockCount()
    {
        _blocksLeftText.text = "Blocks Left: " + _blocksLeft;
    }

    public void OnBlockDestroyed()
    {
        _blocksLeft--; 
        UpdateBlockCount();
        if (_blocksLeft <= 0)
        {
            ClearLevel();
            _waitingForNextLevel = true;
        }
    }

    public void LooseLife()
    {
        SetLives(_lives - 1);
        if (_lives <= 0)
        {
            PauseGame();
            _looseScreen.gameObject.SetActive(true);
        }
    }

    void ClearLevel()
    {
        int i = 0;
        GameObject[] allChildren = new GameObject[_levelHolder.transform.childCount];
        
        foreach (Transform child in _levelHolder.transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }
        
        foreach (GameObject child in allChildren)
        {
            Destroy(child.gameObject);
        }
        
        BouncyBall[] oldBalls = FindObjectsOfType<BouncyBall>();
        foreach (var oldBall in oldBalls)
        {
            Destroy(oldBall.gameObject);
        }
    }
    
    public void PauseGame()
    {
        IsOnPause = true;
        BouncyBall[] balls = FindObjectsOfType<BouncyBall>();
        foreach (var ball in balls)
        {
            ball.OnGamePause();
        }
        Platform platform = FindObjectOfType<Platform>();
        platform.OnGamePause();
        _buttons.SetActive(false);
    }
    
    public void UnpauseGame()
    {
        IsOnPause = false;
        BouncyBall[] balls = FindObjectsOfType<BouncyBall>();
        foreach (var ball in balls)
        {
            ball.OnGameUnpause();
        }
        Platform platform = FindObjectOfType<Platform>();
        platform.OnGameUnpause();
        _buttons.SetActive(true);
        
    }
    public void HideTitleScreen()
    {
        _titleScreen.SetActive(false);
    }

    public void ShowPauseScreen()
    {
        _pauseScreen.SetActive(true);
    }

    public void ShowHelpScreen()
    {
        _helpScreen.SetActive(true);
    }

    public void HidePauseScreen()
    {
        _pauseScreen.SetActive(false);
    }

    public void HideHelpScreen()
    {
        _helpScreen.SetActive(false);
    }
}
