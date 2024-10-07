using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopUp : GimmickOutput
{
    [Header("튜토리얼 팝업에 뜰 이미지 \ninputgimmick에 event 에서 SetImage()로 설정하실 수 있습니다")]
    public Sprite image;
    [Header("튜토리얼 팝업에 뜰 텍스트 \ninputgimmick에 event 에서 SetString()로 설정하실 수 있습니다")]
    [Multiline(4)]
    public string info;
    public void open()
    {
        TutorialUi.Instance.OpenPopUp(info, image);
    }
    public void close()
    {
        TutorialUi.Instance.CloesPopUp();
    }
    public void SetImage(Sprite sprite)
    {
        image = sprite;
    }
    public void SetString(string egsw)
    {
        info = egsw;
    }
}
