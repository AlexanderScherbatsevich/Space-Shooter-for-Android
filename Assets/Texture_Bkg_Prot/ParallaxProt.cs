using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxProt : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject poi; //корабль игрока
    public GameObject[] panels; //прокручивание панели переднего плана
    public GameObject[] panelsNebula;
    public float scrollSpeed = -30f;
    public float scrollSpeedNebula = -20f;
    public float motionMult = 0.25f; //определяет степень реакции панелей на перемещение корабля игрока

    private float panelHt; //высота каждой панели
    private float depth; //глубина(позиция z)
    private float depthNebula;

    void Start()
    {
        panelHt = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;
        depthNebula = panelsNebula[0].transform.position.z;
        ////установить панели в начальные позиции
        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelHt, depth);
        panelsNebula[0].transform.position = new Vector3(0, 0, depthNebula);
        panelsNebula[1].transform.position = new Vector3(0, panelHt, depthNebula);
    }

    void Update()
    {
        float tY, tX = 0;
        tY = Time.time * scrollSpeed % panelHt + (panelHt * 0.5f);
        if (poi != null)
        {
            tX = -poi.transform.position.x * motionMult;
        }

        //сместить панель panels[0]
        panels[0].transform.position = new Vector3(tX, tY, depth);
        panelsNebula[0].transform.position = new Vector3(tX, tY, depthNebula);
        //сместить панель panels[1], чтобы создать эффект непрерывности звездного поля
        if (tY >= 0)
        {
            panels[1].transform.position = new Vector3(tX, tY - panelHt, depth);
            panelsNebula[1].transform.position = new Vector3(tX, tY - panelHt, depthNebula);
        }
        else
        {
            panels[1].transform.position = new Vector3(tX, tY + panelHt, depth);
            panelsNebula[1].transform.position = new Vector3(tX, tY + panelHt, depthNebula);
        }
    }
}

