using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestFeedbackUIManager : MonoBehaviour
{
    [SerializeField] GameObject feedbackCanvasGameObject;

    [SerializeField] TMP_InputField inputField;

    [SerializeField] Button storeFeedbackButton;

    TMP_InputField.OnChangeEvent changeEvent;

    private void Awake()
    {
        inputField.onValueChanged.AddListener((x) => UpdateStoreFeedbackPossible(x));
        inputField.text = string.Empty;

        UpdateStoreFeedbackPossible(inputField.text);
    }

    private void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener((x) => UpdateStoreFeedbackPossible(x));
    }

    public void StoreFeedback()
    {
        if (!string.IsNullOrWhiteSpace(inputField.text))
        {
            TestFeedbackCollector.AddFeedback(inputField.text);
            inputField.text = string.Empty;
        }

        Close();
    }

    public void Open()
    {
        feedbackCanvasGameObject.SetActive(true);
    }

    public void Close()
    {
        feedbackCanvasGameObject.SetActive(false);
    }

    private void UpdateStoreFeedbackPossible(string newFeedbackText)
    {
        storeFeedbackButton.interactable = !string.IsNullOrWhiteSpace(newFeedbackText);
    }
}
