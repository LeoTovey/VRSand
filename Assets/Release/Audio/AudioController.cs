using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioController : MonoBehaviour
{
    public AudioClip buttonClickSound;
    public AudioClip buttonSelectSound;
    public AudioSource AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        AudioSource.PlayOneShot(buttonClickSound);
    }

    public void OnButtonSelected()
    {
        AudioSource.PlayOneShot(buttonSelectSound);
    }
}
