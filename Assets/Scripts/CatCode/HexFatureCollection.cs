using UnityEngine;

[System.Serializable]
public struct HexFeatureCollection
{

	public Transform[] prefabs;

	public Transform Pick(float choice)
	{
		return prefabs[(int)(choice * prefabs.Length)];
	}

	public Transform PickRandom()
	{
		return prefabs[Random.Range(0,prefabs.Length)];
	}
}