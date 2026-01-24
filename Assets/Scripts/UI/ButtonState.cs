using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

[Serializable]
public class StateData
{
    public string stateName;
    public GameObject stateVisual;
}

public class ButtonState : MonoBehaviour
{
    [Header("Button States")]
    [SerializeField] private int defaultStateIndex = 0;
    [SerializeField] private List<StateData> states;

    private void Start()
    {
        SetState(defaultStateIndex);
    }

    public void SetState(int stateIndex)
    {
        if (stateIndex < 0 || stateIndex >= states.Count)
        {
            Debug.LogWarning("Invalid state index");
            return;
        }

        // Deactivate all state visuals
        foreach (var state in states)
        {
            if (state.stateVisual != null)
                state.stateVisual.SetActive(false);
        }

        // Activate the selected state visual
        var selectedState = states[stateIndex];
        if (selectedState.stateVisual != null)
            selectedState.stateVisual.SetActive(true);
    }

    public void SetState(string stateName)
    {
        int index = states.FindIndex(s => s.stateName.Equals(stateName, StringComparison.OrdinalIgnoreCase));
        if (index != -1)
        {
            SetState(index);
        }
        else
        {
            Debug.LogWarning("State name not found: " + stateName);
        }
    }

}
