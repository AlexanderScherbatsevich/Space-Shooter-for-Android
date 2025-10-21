using UnityEngine;

public class Enemy_Drone : Enemy
{
    private Vector3 p0, p1;      //две точки для интерполяции
    private float timeStart;     //время создания этого корабля
    private float duration = 2f;

    private void Start()
    {
        SetWeapon();
        Invoke("InitMovement", 1f);
        p0 = p1 = pos;
        timeStart = Time.time;
    }
    void InitMovement()
    {
        p0 = p1;  //переписать р1 в р0
                  // выбрать новую точку р1 на экране
        float widMinRad = bndCheck.camWidth - bndCheck.radius * 3;
        float hdtMinRad = bndCheck.camHeight - bndCheck.radius*3;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(0, hdtMinRad);

        timeStart = Time.time;
    }
    public override void Move()
    {

        float u = (Time.time - timeStart) / duration;
        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2);  //применить плавное замедление
        pos = (1 - u) * p0 + u * p1;
    }
    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        if (go.tag == "Hero")
        {
            Hero.S.shieldLevel--;
            Hero.S.removeShieldSound.Play();
            if (Hero.S.shield.gameObject.activeInHierarchy == false)
            {
                Destroy(Hero.S.gameObject);
                GameManager.S.Explode(Hero.S.gameObject);
                GameManager.S.DelayedRestart(Hero.S.gameRestartDelay);
            }
            Destroy(this.gameObject);
            GameManager.S.Explode(this.gameObject);
        }
    }
}
