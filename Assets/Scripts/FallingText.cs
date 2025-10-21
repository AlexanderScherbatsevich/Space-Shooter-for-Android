using UnityEngine;

public class FallingText : MonoBehaviour
{
    public float speed = 110f;

    private void Update()
    {
        Move();
    }

    public Vector3 pos
    {
        get { return (this.transform.position); }
        set { this.transform.position = value; }
    }
    public  void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

}
