using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassUiScript : Singleton<CompassUiScript>
{
    public CompassBarClass CompassBar;
    public float CompassBarCurrentDegrees;
    public Transform rotationReference;

    [System.Serializable]
    public class CompassBarClass
    {
        public GameObject Panel;
        public RawImage Compass;
        public Transform ElementContainer;
    }

    void Start()
    {
        InitCompassBar();
    }

    void Update()
    {
        if (rotationReference != null)
        {
            UpdateCompassBar(rotationReference);
            //CompassBar.ElementContainer.GetComponent<CompassElementAdder>().UpdateElements(CompassBarCurrentDegrees);
        }
    }

    public void InitCompassBar()
    {
        if (CompassBar.Panel == null || CompassBar.Compass == null || CompassBar.ElementContainer == null)
        {
            ReferencesMissing("Compass Bar");
            return;
        }

        ShowCompassBar(true);
    }

    public void ShowCompassBar(bool value)
    {
        if (CompassBar.Panel != null)
            CompassBar.Panel.gameObject.SetActive(value);
    }

    public void UpdateCompassBar(Transform rotationReference)
    {
        CompassBar.Compass.uvRect = new Rect((rotationReference.eulerAngles.y / 360f) - .5f, 0f, 1f, 1f);
        Vector3 perpDirection = Vector3.Cross(Vector3.forward, rotationReference.forward);
        float angle = Vector3.Angle(new Vector3(rotationReference.forward.x, 0f, rotationReference.forward.z), Vector3.forward);
        CompassBarCurrentDegrees = (perpDirection.y >= 0f) ? angle : 360f - angle;
    }

    private void ReferencesMissing(string componentName)
    {
        Debug.LogError($"{componentName}에 필요한 참조가 없습니다.");
    }
}
