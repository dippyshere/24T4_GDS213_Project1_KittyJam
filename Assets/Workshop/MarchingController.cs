using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingController : MonoBehaviour
{
    public List<GameObject> targetPositions = new List<GameObject>();
    public List<GameObject> spawnedEntities = new List<GameObject>();
    public GameObject marchingPrefab;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject target in targetPositions)
        {
            GameObject entity = Instantiate(marchingPrefab, target.transform.position, Quaternion.identity);
            spawnedEntities.Add(entity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject entity in spawnedEntities)
        {
            entity.transform.position = Vector3.Lerp(entity.transform.position, targetPositions[spawnedEntities.IndexOf(entity)].transform.position, Time.deltaTime * 5f);
        }
    }
}
