using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    // ����� Ŭ���� ����ϱ� ���� ����� �ҽ� ����Ʈ
    private List<AudioSource> audioSources = new List<AudioSource>();

    public float SFXvolume;

    // ����� �ҽ� ������Ʈ�� ��
    public int initialAudioSourceCount = 10;


    // ��� ��Ʈ�ѷ�
    public float BGMvolume;

    public float fadeDuration = 1.0f; // ���̵� ��ȯ �ð�

    public AudioSource activeSource; // ���� Ȱ��ȭ�� ����� �ҽ�
    public AudioSource inactiveSource; // ���� ��Ȱ��ȭ�� ����� �ҽ�




    void Start()
    {
        // �ʱ�ȭ�� �� AudioSource ������Ʈ ����
        for (int i = 0; i < initialAudioSourceCount; i++)
        {
            GameObject soundObject = new GameObject("AudioSource_" + i);
            soundObject.transform.parent = this.transform;  // SoundManager�� �ڽ����� �߰�
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSources.Add(audioSource);
        }

        GameObject bgmObject1 = new GameObject("bgmObject " + 1);
        bgmObject1.transform.parent = this.transform;  // SoundManager�� �ڽ����� �߰�
        AudioSource audioSource1 = bgmObject1.AddComponent<AudioSource>();
        audioSource1.loop = true;

        GameObject bgmObject2 = new GameObject("bgmObject " + 2);
        bgmObject2.transform.parent = this.transform;  // SoundManager�� �ڽ����� �߰�
        AudioSource audioSource2 = bgmObject2.AddComponent<AudioSource>();
        audioSource2.loop = true;

        activeSource = audioSource1;
        inactiveSource = audioSource2;

        SetBGMvolume(BGMvolume);
    }

    public void SetBGMvolume(float value)
    {
        activeSource.volume = value;
        inactiveSource.volume = value;
    }

    public void SetFadeDuration(float value)
    {
        fadeDuration = value;
    }

    public void BGM(AudioClip newClip)
    {
        StartCoroutine(SwitchToNewClip(newClip));
    }

    private IEnumerator SwitchToNewClip(AudioClip newClip)
    {
        // ���� Ȱ��ȭ�� �ҽ��� ������ ���̱�
        float startVolume = activeSource.volume;

        // ���̵� �ƿ�
        while (activeSource.volume > 0)
        {
            activeSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = startVolume;

        // ��Ȱ��ȭ�� �ҽ��� �� Ŭ�� ���� �� ���
        inactiveSource.clip = newClip;
        inactiveSource.Play();
        inactiveSource.volume = 0;

        // ���̵� ��
        while (inactiveSource.volume < startVolume)
        {
            inactiveSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        inactiveSource.volume = startVolume;

        // �ҽ� ��ȯ
        // Ȱ��ȭ�� �ҽ��� ��Ȱ��ȭ�� �ҽ��� ��ȯ
        AudioSource temp = activeSource;
        activeSource = inactiveSource;
        inactiveSource = temp;
    }


    public void SFX(AudioClip clip)
    {
        PlaySound(clip);
    }
    public void SetSFXVolume(float volume)
    {
        SFXvolume = volume;
    }

    // ���带 ����ϴ� �Լ�, ��ġ�� ������ 0,0,0�� ����
    public void PlaySound(AudioClip clip, float volume = 0, Vector3? position = null)
    {
        // ��� ������ ����� �ҽ��� ã��
        AudioSource availableSource = GetAvailableAudioSource();

        if (availableSource != null)
        {
            // ����� �ҽ� ��ġ ���� (���� ������ 0,0,0 ���)
            availableSource.transform.position = position ?? Vector3.zero;

            // ����� Ŭ�� ���� �� ���
            availableSource.clip = clip;
            availableSource.volume = volume == 0 ? SFXvolume : volume;
            availableSource.Play();
        }
        else
        {
            Debug.LogWarning("��� ������ ����� �ҽ��� �����ϴ�.");
        }
    }

    // ��� ������ ����� �ҽ��� ã�� �Լ�
    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if (!audioSource.isPlaying)  // ��� ������ ���� ����� �ҽ� ã��
                return audioSource;
        }

        // ��� ����� �ҽ��� ��� ���� ��� null ��ȯ
        return null;
    }
}
