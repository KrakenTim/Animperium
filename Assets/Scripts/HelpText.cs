using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HelpText : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text textField;

    string ReadMePath => Application.streamingAssetsPath + "/ReadMe.txt";

    private void Awake()
    {
        if (File.Exists(ReadMePath))
        {
            textField.text = File.ReadAllText(ReadMePath);
        }
    }
}
