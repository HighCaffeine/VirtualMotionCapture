using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    [SerializeField] private Transform prefab;

    [Header("BGM")]
    [SerializeField] private string[] bgms;
    [SerializeField] private Transform bgmList;


    [Header("Effect")]
    [SerializeField] private string[] effects;
    [SerializeField] private Transform effectList;

    void Start()
    {
        bgms = SoundManager.Instance.TestGetSound(SoundManager.SoundType.Bgm);
        effects = SoundManager.Instance.TestGetSound(SoundManager.SoundType.Effect);

        CreateTestButtons(SoundManager.SoundType.Bgm);
        CreateTestButtons(SoundManager.SoundType.Effect);
    }

    private void CreateTestButtons(SoundManager.SoundType type)
    {
        int count = type == SoundManager.SoundType.Effect ? effects.Length : bgms.Length;

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab).gameObject;
            TextMeshProUGUI text = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            obj.transform.name = type == SoundManager.SoundType.Effect ? effects[i] : bgms[i];
            text.SetText(obj.name);

            

            obj.transform.SetParent(type == SoundManager.SoundType.Effect ? effectList : bgmList);
        }
    }
}
