using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    //Un vector de prefabricate folosite sa fie create obiecte in lumea de joc
    [SerializeField]
    private GameObject[] objectPrefabs;

    private List<GameObject> pooledObjects = new List<GameObject>();

    /// <summary>
    /// Preia un obiect din colectia de obiecte
    /// </summary>
    /// <param name="type">Tipul de obiect preluat</param>
    /// <returns>Un GameObject de tip specific</returns>
    public GameObject GetObject(string type)
    {
        foreach(GameObject go in pooledObjects)
        {
            if (go.name==type && !go.activeInHierarchy)
            {
                go.SetActive(true);
                return go;
            }
        }

        //Daca colectia nu contine obiectul de care avem nevoie, atunci trebuie sa il cream
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            //Daca avem un prefabricat pentru crearea obiectului
            if (objectPrefabs[i].name==type)
            {
                //Instantiem prefabricatul de tipul corect
                GameObject newObject = Instantiate(objectPrefabs[i]);
                pooledObjects.Add(newObject);
                newObject.name = type;
                return newObject;
            }
        }

        return null;
    }    

    public void ReleaseObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

}
