using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SoundController : MonoBehaviour {

    [SerializeField] private Image[] soundObjects = null;
    [SerializeField] private Sprite[] soundSprite = null;
    [SerializeField] private AudioMixerGroup mixer = null;

    [SerializeField] private AudioSource buffSimple = null;
    [SerializeField] private AudioSource shoot = null;
    [SerializeField] private AudioSource exitButton = null;
    [SerializeField] private AudioSource clickOpen = null;
    [SerializeField] private AudioSource clickClose = null;
    [SerializeField] private AudioSource charCollision = null;
    [SerializeField] private AudioSource death = null;
    [SerializeField] private AudioSource premiumBuffs = null;
    [SerializeField] private AudioSource buyCristal = null;
    [SerializeField] private AudioSource allmusic = null;
    private bool musicOn = true;
    private bool soundOn = true;


    private void Start() 
    {
        UpdateSound();
    }

    public void PlayBuffSimple() 
    {
        buffSimple.Play();
    }

    public void PlayShoot() 
    {
        shoot.PlayOneShot(shoot.clip);
    }

    public void PlayCharCollision() 
    {
        charCollision.Play();
    }

    public void PlayExitButton()
    {
        exitButton.Play();
    }

    public void PlayClickClose()
    {
        clickClose.Play();
    }

    public void PlayClickOpen()
    {
        clickOpen.Play();
    }

    
    public void PlayDeath()
    {
        death.Play();
    }
    public void PlayPremiumBuffs()
    {
        premiumBuffs.Play();
    }

    public void PlayBuyCrystal()
    {
        buyCristal.Play();
    }

    private void UpdateSound()
    {
        if(!SaveGame.sv.sounds)
        {
            soundObjects[0].sprite = soundSprite[1];
            soundOn = false;
            mixer.audioMixer.SetFloat("SoundVolume", -80);
            mixer.audioMixer.SetFloat("UIVolume", -80);
        }
        if(!SaveGame.sv.music)
        {
            soundObjects[1].sprite = soundSprite[3];
            musicOn = false;
            allmusic.Stop();
        }
    }


    public void ToggleMusic()
    {
        if(musicOn)
        {
            soundObjects[1].sprite = soundSprite[3];
            musicOn = false;
            SaveGame.sv.music = false;
            allmusic.Stop();
        }
        else
        {
            soundObjects[1].sprite = soundSprite[2];
            musicOn = true;
            SaveGame.sv.music = true;
            allmusic.Play();
        }
    }

    public void ToggleSound()
    {
        if(soundOn)
        {
            soundObjects[0].sprite = soundSprite[1];
            soundOn = false;
            SaveGame.sv.sounds = false;
            mixer.audioMixer.SetFloat("SoundVolume", -80);
            mixer.audioMixer.SetFloat("UIVolume", -80);
        }
        else
        {
            soundObjects[0].sprite = soundSprite[0];
            soundOn = true;
            SaveGame.sv.sounds = true;
            mixer.audioMixer.SetFloat("SoundVolume", -27);
            mixer.audioMixer.SetFloat("UIVolume", 0);
        }
    }
}
