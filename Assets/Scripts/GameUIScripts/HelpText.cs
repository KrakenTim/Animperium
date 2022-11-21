using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HelpText : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text textField;

    string ReadMePath => Path.Combine(Application.streamingAssetsPath, "ReadMe.txt");

    [SerializeField] ScrollRect scrollRect;

    private Color oldColor;
    private void Awake()
    {
        if (File.Exists(ReadMePath))
        {
            oldColor = textField.color;

            textField.color = Vector4.zero;
            textField.text = File.ReadAllText(ReadMePath).Trim();
        }

        StartCoroutine(SetToStart());
    }

    IEnumerator SetToStart()
    {
        yield return new WaitForEndOfFrame();

        textField.color = oldColor;
        scrollRect.verticalNormalizedPosition = 1.0f;
    }

    private void OnEnable()
    {
        HexMapCamera.Locked = true;
    }

    private void OnDisable()
    {
        HexMapCamera.Locked = false;
    }
}
