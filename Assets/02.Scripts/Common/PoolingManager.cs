using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager p_Instance;

    [Header("ExpEffect Pool")]
    public GameObject expEffect;
    public List<GameObject> expPool;

    private void Awake()
    {
        if (p_Instance == null)
            p_Instance = this;
        else if (p_Instance != this)
            Destroy(gameObject);

        StartCoroutine(CreateExpEffect());
    }


    IEnumerator CreateExpEffect()
    {
        yield return new WaitForSeconds(0.5f);
        expEffect = Resources.Load<GameObject>("Effects/BigExplosionEffect");
        GameObject obj = new GameObject("ObjExpEffect");
        for (int i = 0; i < 10; i++)
        {
            var exp = Instantiate(expEffect, obj.transform);
            exp.name = $"{i + 1}¹ø exp";
            exp.SetActive(false);
            expPool.Add(exp);
        }
    }

    public GameObject GetExp()
    {
        for (int i = 0; i < expPool.Count; i++)
        {
            if (!expPool[i].activeSelf)
            {
                return expPool[i];
            }
        }
        return null;
    }
}
