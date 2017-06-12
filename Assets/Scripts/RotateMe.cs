using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMe : MonoBehaviour
{
    public float TurnRate = 60f;

    void Update()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * TurnRate);

    }
}
