using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    PlayerPawn actingPawn;
    HexCell targetCell;

    [SerializeField] float flyingTime = 0.5f;

    [SerializeField] Vector3 startOffset = new Vector3(0f, 4f, 0f);
    [SerializeField] Vector3 targetOffset = new Vector3(0f, 4f, 0f);

    private void Awake()
    {
        actingPawn = FeedbackManager.LastActingPawn;
        targetCell = FeedbackManager.LastTargetCell;

        StartCoroutine(Fly());
    }

    private IEnumerator Fly()
    {
        float elapsed = 0;

        Vector3 startPosition = actingPawn.HexCell.ObjectPosition + startOffset;
        Vector3 targetPosition = targetCell.ObjectPosition + targetOffset;

        while (elapsed < flyingTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / flyingTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
