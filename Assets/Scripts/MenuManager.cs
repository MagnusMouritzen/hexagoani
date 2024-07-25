using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [SerializeField] private GameObject menu;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text sizeText;
    [SerializeField] private Slider sizeSlider;
    
    private bool _isOpen;

    private void Start() {
        OnSizeChange();
        StartGame();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleMenu();
        }
    }

    private void CloseMenu() {
        _isOpen = false;
        menu.SetActive(false);
    }
    
    private void ToggleMenu() {
        _isOpen = !_isOpen;
        menu.SetActive(_isOpen);
    }

    private void NewGame() {
        gameManager.CleanUpGame();
        StartGame();
    }

    private void StartGame() {
        CloseMenu();
        gameManager.StartNewGame((int)sizeSlider.value);
    }

    private void OnSizeChange() {
        sizeText.text = sizeSlider.value.ToString();
    }
}
