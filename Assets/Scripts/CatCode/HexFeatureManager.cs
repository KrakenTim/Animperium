using UnityEngine;

public class HexFeatureManager : MonoBehaviour
{
    //public Transform[][] urbanPrefabs;
    public HexFeatureCollection[] structureCollections, plantCollections;
    public Transform[] special;

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

    Transform PickPrefab(HexFeatureCollection[] collection, int level, float hash, float choice)
    {
        if (level > 0)
        {
            float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
            for (int i = 0; i < thresholds.Length; i++)
            {
                if (hash < thresholds[i])
                {
                    return collection[level - 1].Pick(choice);
                }
            }
        }
        return null;
    }

    public void AddFeature(HexCell cell, Vector3 position)
    {
        if (cell.IsSpecial)
        {
            return;
        }

        HexHash hash = HexMetrics.SampleHashGrid(position);

        Transform prefab = PickPrefab(plantCollections, cell.PlantLevel, hash.a, hash.d);
        Transform otherPrefab = PickPrefab(structureCollections, cell.DecoLevel, hash.b, hash.d);
        float usedHash = hash.a;
        if (prefab)
        {
            if (otherPrefab && hash.b < hash.a)
            {
                prefab = otherPrefab;
                usedHash = hash.b;
            }
        }
        else if (otherPrefab)
        {
            prefab = otherPrefab;
            usedHash = hash.b;
        }
        else
        {
            return;
        }
        Transform instance = Instantiate(prefab);
        //position.y += instance.localScale.y * 0.5f;
        instance.localPosition = HexMetrics.Perturb(position);
        instance.localRotation = Quaternion.Euler(instance.rotation.eulerAngles.x, 360f * hash.e, instance.rotation.eulerAngles.z);
        instance.SetParent(container, false);
    }

    public void AddSpecialFeature(HexCell cell, Vector3 position)
    {
        if (cell.SpecialIndex < 1 || cell.SpecialIndex > special.Length)
        {
            Debug.LogError($"Unknown Special index({cell.SpecialIndex}) for cell at {cell.transform.position}.\n", cell);
            return;
        }

        //Transform instance = Instantiate(special[cell.SpecialIndex - 1]);
        Transform instance = Instantiate(special[cell.SpecialIndex - 1], cell.ObjectPosition, Quaternion.identity, container);
        //instance.localPosition = HexMetrics.Perturb(position);
        //HexHash hash = HexMetrics.SampleHashGrid(position);
        //instance.localRotation = Quaternion.Euler(0f, 360f * hash.e, 0f); //rotates buildings randomly
        //instance.SetParent(container, false);
    }
}
