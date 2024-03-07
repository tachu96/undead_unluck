using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuukoBehaviour : MonoBehaviour
{
    public void PrepareForCarry() {
        gameObject.SetActive(false);
    }
    public void PrepareForRelease()
    {
        gameObject.SetActive(true);
    }
}
