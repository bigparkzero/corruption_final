using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interaction_Gimmick : GimmickTrigger
{

    public LayerMask DetectionLayer; // �ߵ� ���� ���̾�

    public RectTransform InteractionImge; // UI �̹����� RectTransform
    Camera mainCamera; // ���� ī�޶�

    public Transform targetObject; // 3D ��ü�� Transform
    private bool isPlayerInTrigger;

    public enum einteractiontype
    {
        basic_Interaction,
        Gauge_Based_Interaction
    }
    [Header("��ȣ�ۿ� ����")]
    public einteractiontype InteractionType;

    [Header("Gauge_Based_Interaction�� ������������ ���Ǵ� ������")]
    public float maxGaugeValue = 100f; // ������ �ִ� ��
    public float gaugeFillRate = 10f; // ������ ä��� �ӵ�

    public Slider gaugeFillSlider;

    [HideInInspector]public float currentGaugeValue = 0f;
    private bool isInteracting = false;

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        if (InteractionImge != null)
        {
            InteractionImge.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        InteractionImge.gameObject.SetActive(false);

    }
    private void Update()
    {
        switch (InteractionType)
        {
            case einteractiontype.basic_Interaction:
                if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.G))
                {
                    InvokeEventRunOnTrigger();
                }
                if (isTriggered && isPlayerInTrigger)
                {
                    InteractionImge.gameObject.SetActive(false);
                }
                break;
            case einteractiontype.Gauge_Based_Interaction:
                if (isPlayerInTrigger && Input.GetKey(KeyCode.G))
                {
                    // �������� ä��ϴ�
                    currentGaugeValue += gaugeFillRate * Time.deltaTime;
                    currentGaugeValue = Mathf.Clamp(currentGaugeValue, 0, maxGaugeValue);

                    if (gaugeFillSlider != null)
                    {
                        gaugeFillSlider.maxValue = maxGaugeValue;
                        gaugeFillSlider.value = currentGaugeValue;
                    }

                    // �������� �ִ뿡 �����ϸ� ��ȣ�ۿ��� �����մϴ�
                    if (currentGaugeValue >= maxGaugeValue && !isInteracting)
                    {
                        isInteracting = true;
                        InvokeEventRunOnTrigger();
                    }
                }
                else
                {
                    // ��ȣ�ۿ� Ű�� ������ �������� �ʱ�ȭ�մϴ�
                    currentGaugeValue = 0;
                   isInteracting = false;
                }
                if (Input.GetKeyUp(KeyCode.G))
                {
                    gaugeFillSlider.value = 0;
                }
                break;
            default:
                break;
        }


    }



    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & DetectionLayer) != 0)
        {
            InteractionImge.gameObject.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & DetectionLayer) != 0 )
        {
            // 3D ��ü�� ��ġ�� ȭ�� ��ǥ�� ��ȯ
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetObject.position);

            // ȭ�� ��ǥ�� UI ��ǥ�� ��ȯ
            InteractionImge.position = screenPos;
            isPlayerInTrigger = true;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & DetectionLayer) != 0)
        {
            InteractionImge.gameObject.SetActive(false);
            isPlayerInTrigger = false;
            if(gaugeFillSlider != null)
                gaugeFillSlider.value = 0;
        }
    }
}
