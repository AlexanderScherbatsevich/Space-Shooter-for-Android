using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : Enemy_4
{
    private Vector3 pos0, pos1;      //��� ����� ��� ������������
    private float timeStartBoss;     //����� �������� ����� �������
    private float durationMovement = 5f;

    void Start()
    {
        SetWeapon();
        Invoke("InitMovement", 0.5f);
        pos0 = pos1 = pos;
        timeStartBoss = Time.time;

        //�������� � ��� ������� ������ � �������� ������ ����� � parts
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }
    private void Update()
    {
        Move();
    }
    public override void InitMovement()
    {
        pos0 = pos1;  //���������� �1 � �0
                  // ������� ����� ����� �1 �� ������
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hdtMinRad = bndCheck.camHeight - bndCheck.radius;
        pos1.x = Random.Range(-widMinRad, widMinRad);
        pos1.y = Random.Range(0, hdtMinRad);

        timeStartBoss = Time.time;
    }
    public override void Move()
    {

        float u = (Time.time - timeStartBoss) / durationMovement;
        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2);  //��������� ������� ����������
        pos = (1 - u) * pos0 + u * pos1;
    }
}
