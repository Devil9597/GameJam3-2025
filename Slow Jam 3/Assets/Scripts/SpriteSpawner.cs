using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class SpriteSpawner : MonoBehaviour
{
    [SerializeField] private float _rotationOffset;
    [SerializeField] private float _rotationRandomMin = -20;
    [SerializeField] private float _rotationRandomMax = 20;


    [SerializeField] private SplineContainer _splineToFollow;
    [SerializeField] private GameObject _spritePrefab;

    [SerializeField] private Vector2 Size = new Vector2(16, 16);

    
    /// <summary>
    /// determines how many sprites to place over the entire spline
    /// </summary>
    [SerializeField] private int _splineResolution = 100;

    [SerializeField] private EdgeCollider2D _edgeCollider;
    /// <summary>
    /// how many collision points to place over the spline
    /// </summary>
    [SerializeField] private int _colliderRes = 100;


    [SerializeField] private Transform test;

    [SerializeField] private float _currentProgres = 0;
    [SerializeField] private float _speed = 1;
    [SerializeField, Range(0, 1)] private float _target = 0.5f;

    [SerializeField] private List<SpriteVisuals> _sprites = new();

    [FormerlySerializedAs("_particleSystem")] [SerializeField]
    private ParticleSystem _growParticles;


    [Serializable]
    class SpriteVisuals
    {
        public GameObject Sprite;
        public float Percent;
    }

    void Start()
    {
        SegmentSpline();
        SetCollider();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _target = 0;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _target = 1;
        }

        var step = _speed * Time.deltaTime;
        _currentProgres = Mathf.MoveTowards(_currentProgres, _target, step);
        var worldPos = _splineToFollow.EvaluatePosition(_currentProgres);

        SetCollider();
        if (_currentProgres < _target)
        {
            // moving forward
            for (int i = 0; i < _sprites.Count; i++)
            {
                var sprite = _sprites[i];
                if (sprite.Percent > _currentProgres)
                {
                    // reached end of out current progress
                    break;
                }

                if (sprite.Percent < _currentProgres)
                {
                    sprite.Sprite.SetActive(true);
                }
            }
        }

        if (_currentProgres > _target)
        {
            // moving backward
            for (int i = _sprites.Count - 1; i >= 0; i--)
            {
                var sprite = _sprites[i];

                if (sprite.Percent < _currentProgres)
                {
                    //reached end of current prog
                    break;
                }

                if (sprite.Percent > _currentProgres)
                {
                    sprite.Sprite.SetActive(false);
                }
                else
                {
                    // breaking early here should be ok since any sprites after this will always be active 
                    // assuming the order isnt broken
                    break;
                }
            }
        }


        _growParticles.transform.position = worldPos;
    }


    // be normal and make it an overload when you sober up


    [ContextMenu("Update Vine")]
    private void DrawToPercent()
    {
        if (Application.isPlaying)
        {
            foreach (var s in _sprites)
            {
            }
        }
    }

    private void SetCollider()
    {
        _edgeCollider.Reset();
        List<Vector2> points = new();
        for (int i = 1; i < _colliderRes + 1; i++)
        {
            var percent = (float)i / _colliderRes;

            if (percent >= _currentProgres)
            {
                break;
            }


            var pos = _splineToFollow.EvaluatePosition(percent);
            pos = transform.InverseTransformPoint(pos);

            points.Add(new Vector2(pos.x, pos.y));
        }

        _edgeCollider.points = points.ToArray();
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