using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationTest : MonoBehaviour
{
    private enum Anis
    {
        DoodleDance,
        Loli,
    }

    [SerializeField] private Animator[] targetAni;

    public TMP_Dropdown myDropdown;

    private int aniIndex = 0;

    void Start()
    {
        myDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDropdownValueChanged(int index)
    {
        aniIndex = index;
        Debug.Log("인덱스: " + index);
        Debug.Log("옵션: " + myDropdown.options[index].text);
    }

    public void PlayAni()
    {
        int currentIndex = myDropdown.value;
        Debug.Log("현재 인덱스: " + currentIndex);

        foreach (var ani in targetAni)
        {
            ani.SetTrigger(((Anis)aniIndex).ToString());
        }
    }
}
