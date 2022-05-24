using UnityEngine;

public class HexFeatureManager : MonoBehaviour
{
    public Transform[] urbanPrefabs;

	Transform container;

	public void Clear()
	{
		if (container)
		{
			Destroy(container.gameObject);
		}
		container = new GameObject("Features Container").transform;
		container.SetParent(transform, false);
	}

	public void Apply() { }

	public void AddFeature(HexCell cell, Vector3 position)
	{
		HexHash hash = HexMetrics.SampleHashGrid(position);
		if (hash.a >= cell.UrbanLevel * 0.25f)// tutorial sais * 0.25 for 4 urban levels
		{
			return;
		}
		Transform instance = Instantiate(urbanPrefabs[cell.UrbanLevel - 1]);
		position.y += instance.localScale.y * 0.5f;
        instance.localPosition = position;
		instance.localRotation = Quaternion.Euler(0f, 360f * hash.b, 0f);
		instance.SetParent(container, false);
	}
}
