using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1")]
    //число секунд полного цикла синусоиды
    public float waveFrequency = 2;
    //ширина синусоиды в метрах
    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0;  //начальное значание координаты х
    private float birthTime;

    void Start()
    {
        x0 = pos.x;
        birthTime = Time.time;
        SetWeapon();
    }
    //переопределить функцию Move() базового класса Enemy
    public override void Move()
    {
        Vector3 tempPos = pos;
        //значение theta изменяется с течением времени
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        //повернуть немного относительно оси Y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        //обрабатывает движение вниз, вдоль оси х
        base.Move();

       // print(bndCheck.isOnScreen);
    }
}
