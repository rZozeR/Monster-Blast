using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }


    private AudioSource _audioSource;

    private Camera _camera;


    [SerializeField] private List<AudioClip> _clipList;

    [SerializeField] private Vector2 _pitchRange;

    [SerializeField] private List<Color> _colors;

    private float _originalPitch;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        _audioSource = GetComponent<AudioSource>();
        _camera = Camera.main;

        _originalPitch = _audioSource.pitch;

        ChangeBackground();
    }

    public void PlaySound(int _index, bool randomPitch)
    {
        _audioSource.pitch = randomPitch ? Random.Range(_pitchRange.x, _pitchRange.y) : _audioSource.pitch;
        _audioSource.PlayOneShot(_clipList[_index]);

        ChangeBackground();
    }

    private void ChangeBackground()
    {
        _camera.backgroundColor = _colors[Random.Range(0, _colors.Count)];
    }

    public void PlayS(AudioClip _clip, bool _randomPitch)
    {
        _audioSource.pitch = _randomPitch ? Random.Range(_pitchRange.x, _pitchRange.y) : _audioSource.pitch;
        _audioSource.PlayOneShot(_clip);
        _audioSource.pitch = _originalPitch;
    }

    public void PlayS(List<AudioClip> _clips, bool _randomPitch)
    {
        _audioSource.pitch = _randomPitch ? Random.Range(_pitchRange.x, _pitchRange.y) : _audioSource.pitch;
        _audioSource.PlayOneShot(_clips[Random.Range(0, _clips.Count)]);
        _audioSource.pitch = _originalPitch;
    }
}
