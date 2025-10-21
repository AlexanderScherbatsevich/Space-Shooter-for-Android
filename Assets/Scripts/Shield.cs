using UnityEngine;

public class Shield : MonoBehaviour
{

    //[Header("Set in Inspector")]
    //public float rotationsPerSecond = 0.1f;

    //[Header("Set Dynamically")]
    //public int levelShown = 0;


    //Color red = new Color(7, 0, 0, 0);
    //Color borderRed = new Color(255, 0, 0, 0);

    //Color yellow = new Color(7, 7, 0, 0);
    //Color borderYellow = new Color(255, 255, 0, 0);

    //Color green = new Color(0, 7, 0, 0);
    //Color borderGreen = new Color(0, 255, 0, 0);

    //Color blue = new Color(0, 0, 7, 0);
    //Color borderBlue = new Color(0, 0, 255, 0);

    Color red = new Color(0.027f, 0, 0, 0);
    Color borderRed = new Color(1f, 0, 0, 0);

    Color yellow = new Color(0.027f, 0.027f, 0, 0);
    Color borderYellow = new Color(1f, 1f, 0, 0);

    Color green = new Color(0, 0.027f, 0, 0);
    Color borderGreen = new Color(0, 1f, 0, 0);

    Color blue = new Color(0, 0, 0.027f, 0);
    Color borderBlue = new Color(0, 0, 1f, 0);


    float duration = 1f;


    Material mat;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        ////прочитать текущую мощность защитного поля из объекта-одиночки Hero
        //int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        //if (levelShown != currLevel)
        //{
        //    levelShown = currLevel;
        //    //скорректировать смещение в текстуре, для отображения поля другой мощности
        //    mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        //}
        //float rZ = -(rotationsPerSecond * Time.time * 360) % 360f;
        //transform.rotation = Quaternion.Euler(0, 0, rZ);
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);       
        switch (currLevel)
        {
            case 0:
                this.gameObject.SetActive(false);
                //mat.color = Color.clear;
                break;
            case 1:
                //mat.color = Color.red;
                mat.SetColor("_Color", red);
                mat.SetColor("_Boarder_Color", borderRed);
                break;
            case 2:
                //mat.color = Color.yellow;
                mat.SetColor("_Color", yellow);
                mat.SetColor("_Boarder_Color", borderYellow);
                break;
            case 3:
                //mat.color = Color.green;
                mat.SetColor("_Boarder_Color", borderGreen);
                mat.SetColor("_Color", green);
                break;
            case 4:
                //mat.color = Color.blue;
                mat.SetColor("_Color", blue);
                mat.SetColor("_Boarder_Color", borderBlue);
                break;
            default:
                break;
        }
    }

    void ChangeColor(Color a, Color b)
    {

    }
}
