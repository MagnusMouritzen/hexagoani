using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject help;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text sizeText;
    [SerializeField] private Slider sizeSlider;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource audioPlayer;

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
        menu.SetActive(false);
    }
    
    private void ToggleMenu() {
        menu.SetActive(!menu.activeSelf);
    }
    
    private void CloseHelp() {
        help.SetActive(false);
    }
    
    private void ToggleHelp() {
        help.SetActive(!help.activeSelf);
    }

    private void NewGame() {
        gameManager.CleanUpGame();
        StartGame();
    }

    private void StartGame() {
        CloseMenu();
        CloseHelp();
        gameManager.StartNewGame((int)sizeSlider.value);
    }

    private void OnSizeChange() {
        sizeText.text = sizeSlider.value.ToString();
    }

    private void OnVolumeChange() {
        audioPlayer.volume = volumeSlider.value;
    }
}
