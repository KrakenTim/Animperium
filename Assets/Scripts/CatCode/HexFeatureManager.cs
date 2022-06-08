using UnityEngine;

public class HexFeatureManager : MonoBehaviour
{
	//public Transform[][] urbanPrefabs;
	public HexFeatureCollection[] structureCollections;

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

	Transform PickPrefab(int level, float hash, float choice)
	{
		if (level > 0)
		{
			float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
			for (int i = 0; i < thresholds.Length; i++)
			{
				if (hash < thresholds[i])
				{
					return structureCollections[i].Pick(choice);
				}
			}
		}
		return null;
	}
	public bool onlyone =true;
	public void AddFeature(HexCell cell, Vector3 position)
	{
		HexHash hash = HexMetrics.SampleHashGrid(position);

		//if (hash.a >= cell.UrbanLevel * 0.25f)
		//{
		//	return;
		//}
		//Transform instance = Instantiate(urbanPrefabs[cell.UrbanLevel - 1]);
		//position.y += instance.localScale.y * 0.5f;
		Transform prefab = PickPrefab(cell.UrbanLevel, hash.a, hash.b);
		if (!prefab)
		{
			return;
		}
		Transform instance = Instantiate(prefab);
		position.y += instance.localScale.y * 0.5f;
		instance.localPosition = HexMetrics.Perturb(position);
		instance.localRotation = Quaternion.Euler(0f, 360f * hash.c, 0f);
		instance.SetParent(container, false);
	}
}
