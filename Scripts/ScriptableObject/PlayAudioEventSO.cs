using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/PlayAudioEventSO")]
public class PlayAudioEventSO : ScriptableObject{
    public UnityAction<AudioClip> onEventRasied;

    public void RaiseEvent(AudioClip audioClip){
        onEventRasied?.Invoke(audioClip);
    }
}