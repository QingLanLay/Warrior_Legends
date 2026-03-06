using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Event/FloatEventSO")]
public class FloatEventSO : ScriptableObject{
    public UnityAction<float> OnEventRaised;

    public void Raise(float amount){
        OnEventRaised?.Invoke(amount);
    }
}