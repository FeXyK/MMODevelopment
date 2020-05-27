using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_FireballScript : MonoBehaviour
{
    public Transform source;
    public Transform target;
    public float speed;
    void Update()
    {
        this.transform.LookAt(target);
        this.transform.Translate(Vector3.forward * Time.deltaTime * speed);
        if (Vector3.Distance(this.transform.position, target.position) < 0.2f)
        {
            Destroy(this.gameObject);
        }
    }
    public void Set(Transform source, Transform target)
    {
        this.source = source;
        this.target = target;
        this.transform.position = source.position;
    }
}
