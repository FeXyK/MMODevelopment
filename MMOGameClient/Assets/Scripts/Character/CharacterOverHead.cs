using UnityEngine;

public class CharacterOverHead : MonoBehaviour
{
    public Transform target;
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);

        transform.eulerAngles = Camera.main.transform.eulerAngles;
    }
}
