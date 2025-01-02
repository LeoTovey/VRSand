using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

// public enum HandPose
// {
//     None = 0,
//     HandUpward = 1,
//     HandDownward = 2,
//     SkinnyPouring = 3,
//     ScatterPouring = 4,
//     FingertipTracing = 5,
//     FingerCarving = 6,
//     HandSweeping = 7,
//     PalmRubbing = 8,
//     UIActivation = 9,
//     ToolHolding = 10,
//     ToolRemoving = 11,
//     Fist = 12,
// }

public class AudioController : MonoBehaviour
{
    [Serializable]
    public struct HandPoseAudio
    {
        public HandPose HandPose;
        public AudioClip[] Sounds;
        public float MaxVolume;
        public float MinVolume;
        public float MaxPitch;
        public float MinPitch;
    }

    public List<HandPoseAudio> HandPoseAudios = new List<HandPoseAudio>();
    private Dictionary<HandPose, HandPoseAudio> _audios = new Dictionary<HandPose, HandPoseAudio>();

    
    public AudioSource AudioSource;
    public HandController HandController;
    
   
    private AudioClip _currentSound;
    private float _currentMaxVolume;
    private float _currentMinVolume;
    private float _currentMaxPitch;
    private float _currentMinPitch;
    
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var pair in HandPoseAudios)
        {
            _audios.Add(pair.HandPose, pair);
        }
    }

    void Update()
    {
        if (HandController.RightHand.CurrentHandPose == HandPose.SkinnyPouring ||
            HandController.RightHand.CurrentHandPose == HandPose.ScatterPouring)
        {
            PlayDrawSound(HandController.RightHand.Strength);
        }
        else
        {
            PlayDrawSound(HandController.RightHand.PalmVelocity.magnitude);
        }
        
    }

    public void PlaySound(float speed)
    {
        PlayDrawSound(speed);
    }
    

    private void PlayDrawSound(float speed)
    {
        if (HandController.RightHand.IsInteracting)
        {
            if (!AudioSource.isPlaying)
            {
                if (_audios.ContainsKey(HandController.RightHand.CurrentHandPose))
                {
                    AudioClip[] sounds = _audios[HandController.RightHand.CurrentHandPose].Sounds;
                    int randomIndex = Random.Range(0, sounds.Length);
                    _currentSound = sounds[randomIndex];
                    
                    _currentMaxVolume = _audios[HandController.RightHand.CurrentHandPose].MaxVolume;
                    _currentMinVolume = _audios[HandController.RightHand.CurrentHandPose].MinVolume;
                    _currentMaxPitch = _audios[HandController.RightHand.CurrentHandPose].MaxPitch;
                    _currentMinPitch = _audios[HandController.RightHand.CurrentHandPose].MinPitch;
                    
                    AudioSource.clip = _currentSound;
                    AudioSource.loop = true;
                    AudioSource.Play();
                }
            }
            
            AudioSource.volume = (AudioSource.volume * 0.8f + Mathf.Lerp(_currentMinVolume, _currentMaxVolume, speed) * 0.2f) ;
            AudioSource.pitch = (AudioSource.pitch * 0.8f + Mathf.Lerp(_currentMinPitch, _currentMaxPitch, speed) * 0.2f) ; 
        }
        else
        {
            AudioSource.Stop();
        }
    }
}
