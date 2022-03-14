using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClickMoveEffect : MonoBehaviour
{
    [SerializeField] private ClickMoveEffect clickMoveEffectPrefab;

    private List<ClickMoveEffect> clickMoveEffectPool = new List<ClickMoveEffect>();

    private void Awake()
    {
        if(clickMoveEffectPrefab == null)
        {
            Debug.LogWarning("Click move effect prefab is not assigned on: " + name + ". Click move effect will not work!");
            enabled = false;
            return;
        }
    }

    private void GenerateEffectAndAddToPool(Vector3 pos)
    {
        ClickMoveEffect clickEffect = Instantiate(clickMoveEffectPrefab, pos, Quaternion.identity);
        if (!clickMoveEffectPool.Contains(clickEffect)) clickMoveEffectPool.Add(clickEffect);
    }

    public void SpawnClickMoveEffectAtPos(Vector3 pos)
    {
        if(clickMoveEffectPool.Count == 0)
        {
            GenerateEffectAndAddToPool(pos);
            return;
        }

        for(int i = 0; i < clickMoveEffectPool.Count; i++)
        {
            if (!clickMoveEffectPool[i].gameObject.activeInHierarchy)
            {
                clickMoveEffectPool[i].transform.position = pos;
                clickMoveEffectPool[i].gameObject.SetActive(true);
                return;
            }
        }

        GenerateEffectAndAddToPool(pos);
    }
}
