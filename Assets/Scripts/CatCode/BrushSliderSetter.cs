using UnityEngine;
using UnityEngine.UI;

public class BrushSliderSetter : MonoBehaviour
{
    [SerializeField] Slider MySlider;

    public void ValueChanged()
    {
        HexMapEditor.Instance.SetBrushSize(MySlider.value);
    }

    private void SetSlider(int NewValue)
    {
        MySlider.SetValueWithoutNotify(NewValue);
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
