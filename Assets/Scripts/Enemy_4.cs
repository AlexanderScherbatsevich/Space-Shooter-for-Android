using UnityEngine;


//����� ������������� ����� ������� WeaponDefinition, ��� �������� ������
[System.Serializable]
public class Part
{
    public string name;           //��� ���� �����
    public float health;          //������� ��������� ���� �����
    public string[] protectedBy;  //������ �����, ���������� ���
    

    [HideInInspector] //������ � ����������
    public GameObject go; //������� ������ ���� �����
    [HideInInspector]
    public Material mat; //�������� ��� ����������� �����������
}
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts; //������ ������ �������
    public GameObject dronePrefab;
    public int droneCount = 3;
    private Vector3 p0, p1;      //��� ����� ��� ������������
    private float timeStart;     //����� �������� ����� �������
    private float duration = 4;  //����������������� �����������

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
        //��������� ������� ��� ������� � Main.SpawnEnemy(), ������� ������� �� ��� ��������� ����� �0 � �1
        p0 = p1 = pos;
        InitMovement();
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

    public virtual void InitMovement()
    {
        p0 = p1;  //���������� �1 � �0
        // ������� ����� ����� �1 �� ������
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
        //�������� �����
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
        u = 1 - Mathf.Pow(1 - u, 2);  //��������� ������� ����������
        pos = (1 - u) * p0 + u * p1;
    }

    //��� ������� ��������� ����� ����� � ������� parts �� ����� ��� ������ �� ������� ������
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

    //��� ������� ���������� true, ���� ������ ����� ����������
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
        if (prt == null) return (true); //���� ������ �� ����� �� ���� ��������, ������� true
        return (prt.health <= 0); //������� ��������� ���������, true, ���� ��� ���������
    }

    //���������� ������ ���� �����, � �� ���� �������
    protected void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    //�������������� ����� �� �������� Enemy.cs
    protected void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //���� ������� �� �������� ������, �� ���������� ���
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }
                //�������� ��������� �������
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                //���������, �������� �� ��� ��� ����� �������
                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        //���� ���� ����, ���������� ����� ��� �� ���������, �� �������� ����������� ���� �����
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }
                //��� ����� �� ��������, �������� �� �����������, ���� ����� �� Projectile.type � Main.WEAP_DICT
                prtHit.health -= GameManager.GetWeaponDefinition(p.type).damageOnHit;
                //�������� ������ ��������� � �����
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    prtHit.go.SetActive(false); //������ ���������� �������, ��������� ����������� �����
                }
                //��������� ��� �� ������� ��������� ��������
                bool allDestroyed = true; //������������ ��� ��������
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))   //���� �����-�� ����� ��� ����������, �������� false � �������� ����
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)   //���� ������� ��������� ��������, ��������� Main.S
                {
                    GameManager.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                    GameManager.S.Explode(this.gameObject);
                    GameManager.S.AddScore(this);
                }
                Destroy(other); //���������� ������
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
