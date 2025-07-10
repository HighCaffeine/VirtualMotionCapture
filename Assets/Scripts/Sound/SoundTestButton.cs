using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTestButton : MonoBehaviour
{
    public void TestSound()
    {
        GameObject obj = transform.gameObject;

        SoundManager.Instance.PlaySound(obj.name, false);   
    }
}
