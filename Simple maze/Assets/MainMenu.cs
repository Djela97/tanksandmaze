using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioMixer musicMixer;
    public AudioMixer soundMixer;
    public Toggle musicToggle;
    public Toggle soundToggle;
    public Slider musicSlider;
    public Slider soundSlider;
    public InputField rowInput;
    public InputField colInput;
    public InputField numberOfObastacles;

    float previousMusic = 0.0f;
    float previousSound = 0.0f;

    public void SetVolumeMusic(float volume)
    {
        if (musicToggle.isOn)
        {
            previousMusic = volume;
            musicMixer.SetFloat("volume", volume);
        }
    }

    public void SetVolumeSound(float volume)
    {
        if (soundToggle.isOn)
        {
            previousSound = volume;
            soundMixer.SetFloat("volume", volume);
        }
    }

    public void ClickMusic()
    {
        if (musicToggle.isOn)
        {
            musicSlider.value = previousMusic;
            musicMixer.SetFloat("volume", previousMusic);
        }
        else
        {
            musicSlider.value = -80f;
            musicMixer.SetFloat("volume", -80f);
        }
    }

    public void ClickSound()
    {
        if (soundToggle.isOn)
        {
            soundSlider.value = previousSound;
            soundMixer.SetFloat("volume", previousSound);
        }
        else
        {
            soundSlider.value = -80f;
            soundMixer.SetFloat("volume", -80f);
        }
    }

    void Awake()
    {
        rowInput.text = PlayerPrefs.GetInt("numberOfRows").ToString();
        colInput.text = PlayerPrefs.GetInt("numberOfColumns").ToString();
        numberOfObastacles.text = PlayerPrefs.GetInt("numberOfObstacles").ToString();
    }

    public void PlayGame()
    {
        if (rowInput.text.Length == 0)
            rowInput.text = "10";
        PlayerPrefs.SetInt("numberOfRows", int.Parse(rowInput.text));

        if (colInput.text.Length == 0)
            colInput.text = "10";
          PlayerPrefs.SetInt("numberOfColumns", int.Parse(colInput.text));

        if (numberOfObastacles.text.Length == 0)
            numberOfObastacles.text = "3";
        PlayerPrefs.SetInt("numberOfObstacles", int.Parse(numberOfObastacles.text));

        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
