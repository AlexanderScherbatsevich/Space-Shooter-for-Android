using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    // ФУНКЦИИ ДЛЯ РАБОТЫ С МАТЕРИАЛАМИ
    //Возвращает список всех материалов в данном игровом объекте
    //и его дочерних объектах
    static public Material[] GetAllMaterials(GameObject go)
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();
        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends)
        {
            if (rend.material.HasProperty("_Color"))
            {
                mats.Add(rend.material);
            }
        }
        return (mats.ToArray());
    }
}
