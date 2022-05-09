using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMenuManager : MonoBehaviour
{
    private static InteractionMenuManager instance;

    [SerializeField] InteractionContextButton buttonPrefab;
    [Space]
    [SerializeField] GameObject buttonFrame;
    [SerializeField] GameObject mouseRaycastBlocker;

    List<InteractionContextButton> buttonList;

    private void Awake()
    {
        instance = this;

        buttonList = new List<InteractionContextButton>(buttonFrame.GetComponentsInChildren<InteractionContextButton>());

        SetVisible(false);
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }

    // destroy old Buttons
    public static void OpenBuildMenu()
    {
        foreach (var button in instance.buttonList)
        {
            Destroy(button.gameObject);
        }
        instance.buttonList.Clear();

        // create new Buttons
        foreach (var pawnData in GameManager.GetBuildingData())
        {
            InteractionContextButton nextButton = Instantiate(instance.buttonPrefab, instance.buttonFrame.transform);

            nextButton.Initialise(pawnData);

            instance.buttonList.Add(nextButton);
        }

        instance.SetVisible(true);
    }

    public static void Close()
    {
        instance.SetVisible(false);
    }

    private void SetVisible(bool isVisible)
    {
        instance.buttonFrame.SetActive(isVisible);
        instance.mouseRaycastBlocker.SetActive(isVisible);
    }
}
