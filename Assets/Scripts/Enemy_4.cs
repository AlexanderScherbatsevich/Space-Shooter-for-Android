using UnityEngine;


//Новый сериализуемый класс подобно WeaponDefinition, для хранения данных
[System.Serializable]
public class Part
{
    public string name;           //имя этой части
    public float health;          //степень стойкости этой части
    public string[] protectedBy;  //другие части, защищающие эту
    

    [HideInInspector] //скрыть в инспекторе
    public GameObject go; //игровой объект этой части
    [HideInInspector]
    public Material mat; //материал для отображения повреждений
}
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts; //массив частей корабля
    public GameObject dronePrefab;
    public int droneCount = 3;
    private Vector3 p0, p1;      //две точки для интерполяции
    private float timeStart;     //время создания этого корабля
    private float duration = 4;  //продолжительность перемещения

    static public Transform DRONE_ANCHOR;
    void Start()
    {
        SetWeapon();
        if (this.gameObject.name == "Enemy_Boss(Clone)")
        { 
            Invoke("SpawnDrone", 3f);
        }
        if (DRONE_ANCHOR == null)
        {
            GameObject go = new GameObject("_DroneAnchor");
            DRONE_ANCHOR = go.transform;
        }
        //начальная позиция уже выбрана в Main.SpawnEnemy(), поэтому запишем ее как начальные точки р0 и р1
        p0 = p1 = pos;
        InitMovement();
        //записать в кэш игровой объект и материал каждой части в parts
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

    public virtual void InitMovement()
    {
        p0 = p1;  //переписать р1 в р0
        // выбрать новую точку р1 на экране
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hdtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        if (this.gameObject.name == "Enemy_Boss(Clone)")
        {
            p1.y = Random.Range(0, hdtMinRad);
        }
        else
        {
            p1.y = Random.Range(-hdtMinRad, hdtMinRad);
        }
        //сбросить время
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

    //эти функции выполняют поиск части в массиве parts по имени или ссылке на игровой объект
    public Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n) return (prt);
        }
        return (null);
    }
    public Part FindPart(GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go) return (prt);
        }
        return (null);
    }

    //эти функции возвращают true, если данная часть уничтожена
    protected bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }
    protected bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }
    protected bool Destroyed(Part prt)
    {
        if (prt == null) return (true); //если ссылка на часть не была передана, вернуть true
        return (prt.health <= 0); //вернуть результат сравнения, true, если был уничтожен
    }

    //окрашивает только одну часть, а не весь корабль
    protected void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    //переопределяет метод из сценария Enemy.cs
    protected void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //если корабль за границей экрана, не повреждать его
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }
                //поразить вражеский корабль
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                //проверить, защищена ли еще эта часть корабля
                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        //если хоть одна, защищающая часть еще не разрушена, не наносить повреждения этой части
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }
                //эта часть не защищена, нанестии ей повреждение, урон взять из Projectile.type и Main.WEAP_DICT
                prtHit.health -= GameManager.GetWeaponDefinition(p.type).damageOnHit;
                //показать эффект попадания в часть
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    prtHit.go.SetActive(false); //вместо разрушения корабля, отключить уничтоженую часть
                }
                //проверить был ли корабль полностью разрушен
                bool allDestroyed = true; //предположить что разрушен
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))   //если какая-то часть еще существует, записать false и прервать цикл
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)   //если корабль полностью разрушен, уведомить Main.S
                {
                    GameManager.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                    GameManager.S.Explode(this.gameObject);
                    GameManager.S.AddScore(this);
                }
                Destroy(other); //уничтожить снаряд
                break;
        }
    }
    public override void SetWeapon()
    {
        foreach (Weapon wp in weapons)
        {
            wp.SetType(WeaponType.spread);
        }
    }

    void SpawnDrone()
    {
        for (int i = 0; i < droneCount; i++)
        {
            GameObject go = Instantiate<GameObject>(dronePrefab);
            go.transform.position = this.gameObject.transform.position;
            go.transform.SetParent(DRONE_ANCHOR, true);
        } 
        Invoke("SpawnDrone", 10f);

    }
}
