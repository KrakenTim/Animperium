using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Provides Methods to step through a given InputMessageLog.
/// </summary>
public class InputMessageLogPlayer : MonoBehaviour
{
    [TextArea, SerializeField] string playedInputLog;

    private ReplayCameraPositioner cameraPositioner;

    private void Awake()
    {
        cameraPositioner = GetComponent<ReplayCameraPositioner>();
    }

    public string CallNext()
    {
        string nextLine = playedInputLog.Split('\n')[0];

        if (nextLine.Length < 1) return "";

        playedInputLog = playedInputLog.Substring(nextLine.Length).TrimStart();

        nextLine = nextLine.Trim();

        if (nextLine.Length > 0)
        {
            if (InputMessageInterpreter.TryParseMessage(nextLine, out InputMessage order))
            {
                if (cameraPositioner && order.IsOnHexGrid && cameraPositioner.IsShiftNeeded(order, out Vector3 position))
                    cameraPositioner.SetPosition(position);

                InputMessageExecuter.Execute(order);
            }
            else
                Debug.LogError("LogReplay\tCouldn't parse message.\n\t\t" + nextLine);
        }

        return nextLine;
    }

    public void CallAllUntilTurnEnd()
    {
        string next;
        do
        {
            next = CallNext();
        } while (next.Length > 0 && !next.Contains(ePlayeractionType.EndTurn.ToString()));
    }

    public void CallAllInLog()
    {
        string next;
        do
        {
            next = CallNext();
        } while (!string.IsNullOrWhiteSpace(next));
    }
}

#if UNITY_EDITOR
/// <summary>
/// This Class extends the default Unity Editor for the Class.
/// Buttons:
/// - perfom the next message in the log,
/// - perform all messages until the next player's turn
/// - perform all messages in the log
/// </summary>
[CustomEditor(typeof(InputMessageLogPlayer))]
public class InputMessageLogCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Next Input"))
            ((InputMessageLogPlayer)target).CallNext();

        if (GUILayout.Button("Complete Turn"))
            ((InputMessageLogPlayer)target).CallAllUntilTurnEnd();

        if (GUILayout.Button("Complete Log"))
            ((InputMessageLogPlayer)target).CallAllInLog();

        base.OnInspectorGUI();
    }
}
#endif // UNITY_EDITOR