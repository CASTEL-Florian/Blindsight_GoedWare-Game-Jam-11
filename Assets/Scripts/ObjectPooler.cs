using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public int startCapacity;
        public int maxCapacity;
        public GameObject prefab;
    }
    [SerializeField] List<Pool> pools;

    public Dictionary<string, Stack<GameObject>> poolDictionary;
    public Dictionary<string, GameObject> prefabDictionary;
    public Dictionary<string, int> capacityDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        poolDictionary = new Dictionary<string, Stack<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();
        capacityDictionary = new Dictionary<string, int>();
        foreach (Pool pool in pools)
        {
            poolDictionary.Add(pool.tag, new Stack<GameObject>());
            prefabDictionary.Add(pool.tag, pool.prefab);
            capacityDictionary.Add(pool.tag, pool.maxCapacity);
            for (int i = 0; i < pool.startCapacity; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                poolDictionary[pool.tag].Push(obj);
            }
        }
    }


    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }
        if (poolDictionary[tag].Count > 0)
        {
            GameObject obj = poolDictionary[tag].Pop();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(prefabDictionary[tag], position, rotation);
    }

    public void ReturnObject(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return;
        }
        if (poolDictionary[tag].Count >= capacityDictionary[tag])
        {
            Destroy(obj);
            return;
        }
        obj.SetActive(false);
        poolDictionary[tag].Push(obj);
    }
}
