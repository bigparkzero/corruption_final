using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUi : Singleton<TutorialUi>
{
    public GameObject panel;
    public Image informationImage;
    public TextMeshProUGUI informationtextMeshPro;
    public void OpenPopUp(string info = "" , Sprite infoimage = null)
    {
        informationtextMeshPro.text = info;
        informationImage.sprite = infoimage;
        panel.SetActive(true);
    }
    public void CloesPopUp()
    {
        panel.SetActive(false);
    }
}
