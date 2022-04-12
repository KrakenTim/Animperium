using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{

	public Color[] colors;

	public HexGrid hexGrid;

	private Color activeColor;

	void Awake()
	{
		SelectColor(0);
	}

	void Update()
	{
		if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
		{
			HandleInput();
		}
	}

	void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit))
		{
			hexGrid.ColorCell(hit.point, activeColor);
		}
	}

	public void SelectColor(int index)
	{
		activeColor = colors[index];
	}
}