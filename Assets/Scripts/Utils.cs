using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    // ������� ��� ������ � �����������
    //���������� ������ ���� ���������� � ������ ������� �������
    //� ��� �������� ��������
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
