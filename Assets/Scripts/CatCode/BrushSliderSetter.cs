using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BrushSliderSetter : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI text;

    public void ValueChanged()
    {
        HexMapEditor.Instance.SetBrushSize(slider.value);
        text.text = slider.value.ToString();
    }

    private void SetSlider(int NewValue)
    {
        slider.SetValueWithoutNotify(NewValue);
        text.text = slider.value.ToString();
    }

    private void OnEnable()
    {
        SetSlider(HexMapEditor.Instance.brushSize);
        HexMapEditor.BrushSizeChanged += SetSlider;
    }

    private void OnDisable()
    {
        HexMapEditor.BrushSizeChanged -= SetSlider;
    }
}
