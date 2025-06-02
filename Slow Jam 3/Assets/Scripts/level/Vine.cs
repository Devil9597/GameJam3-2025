using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class Vine : MonoBehaviour
{
    [SerializeField] private float _rotationOffset;
    [SerializeField] private float _rotationRandomMin = -20;
    [SerializeField] private float _rotationRandomMax = 20;


    [SerializeField] private PlayerInputHandler _inputHandler;
    [SerializeField] private SplineContainer _splineToFollow;
    [SerializeField] private GameObject _spritePrefab;


    /// <summary>
    /// determines how many sprites to place over the entire spline
    /// </summary>
    [SerializeField] private int _splineResolution = 100;

    [SerializeField] private EdgeCollider2D _edgeCollider;

    /// <summary>
    /// how many collision points to place over the spline
    /// </summary>
    [SerializeField] private int _colliderRes = 100;


    [SerializeField] private float _currentProgres = 0;
    [SerializeField] private float _speed = 1;
    [SerializeField, Range(0, 1)] private float _targetMin = 0.25f;
    [SerializeField, Range(0, 1)] private float _targetMax = 0.75f;

    [SerializeField] private float _currentMin;

    [FormerlySerializedAs("_currnetMax")] [SerializeField]
    private float _currentMax;

    [SerializeField] private List<SpriteVisuals> _sprites = new();
    [SerializeField] private List<(Vector2 worldPos, float percent)> _colliderPoints = new();

    [SerializeField] private ParticleSystem _growParticlesMin;

    [SerializeField] private ParticleSystem _growParticlesMax;


    [Serializable]
    class SpriteVisuals
    {
        public GameObject Sprite;
        public float Percent;
    }


    void Start()
    {
        SegmentSpline();
        SegmentCollider();
    }

    private bool vineChanged = false;

    private void FixedUpdate()
    {
        var lastMin = _currentMin;
        var lastMax = _currentMax;
        _currentMin = Mathf.MoveTowards(_currentMin, _targetMin, _speed * Time.deltaTime);
        _currentMax = Mathf.MoveTowards(_currentMax, _targetMax, _speed * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.Q))
        {
            _targetMax = Random.Range(0f, 1f);
            _targetMin = Random.Range(0f, 1f);
        }


        if (!Mathf.Approximately(lastMax, _currentMax) || !Mathf.Approximately(lastMin, _currentMin))
        {
            UpdateVine();
        }
    }

    public void SetMaxTarget(float max)
    {
        _targetMax = max;
    }

    public void SetMinTarget(float min)
    {
        _targetMin = min;
    }
// be normal and make it an overload when you sober up


    [ContextMenu("Update Vine")]
    private void UpdateVine()
    {
        if (Application.isPlaying)
        {
            var colliderInRange = _colliderPoints.Where(p => p.percent > _currentMin && p.percent < _currentMax)
                .Select(p => p.worldPos).ToList();


            if (colliderInRange.Count <= 1)
            {
                _edgeCollider.SetPoints(new List<Vector2>() { Vector2.zero, Vector2.zero });
            }
            else
            {
                _edgeCollider.SetPoints(colliderInRange);
            }


            var visualInRange = _sprites.Where(p => p.Percent > _currentMin && p.Percent < _currentMax);
            var spriteVisualsEnumerable = visualInRange.ToList();
            var visualOutOfRange = _sprites.Except(spriteVisualsEnumerable);

            foreach (var visible in spriteVisualsEnumerable)
            {
                visible.Sprite.SetActive(true);
            }

            foreach (var notVisible in visualOutOfRange)
            {
                notVisible.Sprite.SetActive(false);
            }
        }
    }

    private void SegmentCollider()
    {
        _colliderPoints.Clear();
        for (int i = 1; i < _colliderRes + 1; i++)
        {
            var percent = (float)i / _colliderRes;

            var pos = _splineToFollow.EvaluatePosition(percent);
            pos = transform.InverseTransformPoint(pos);

            _colliderPoints.Add((new Vector2(pos.x, pos.y), percent));
        }
    }

    private void SegmentSpline()
    {
        for (int i = 0; i < _sprites.Count; i++)
        {
            Destroy(_sprites[i].Sprite);
        }

        _sprites.Clear();

        for (int i = 1; i < _splineResolution + 1; i++)
        {
            var percent = (float)i / _splineResolution;
            var sprite = Instantiate(_spritePrefab, transform);
            sprite.transform.position = _splineToFollow.EvaluatePosition(percent);
            sprite.SetActive(false);
            _sprites.Add(new SpriteVisuals() { Percent = percent, Sprite = sprite });

            var rotRandom = Random.Range(_rotationRandomMin, _rotationRandomMax);
            sprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, _rotationOffset + rotRandom));
        }
    }
}