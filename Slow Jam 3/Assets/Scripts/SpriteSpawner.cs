using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private float _targetPercent = 0.5f;

    [SerializeField] private SplineContainer _splineToFollow;
    [SerializeField] private GameObject _spritePrefab;

    /// <summary>
    /// spawns +1 to account for the end point of the spline
    /// </summary>
    [SerializeField] private float _spawns = 100;

    [SerializeField] private EdgeCollider2D _edgeCollider;
    [SerializeField] private int _colliderRes = 100;

    private List<(GameObject sprite, float percent)> _sprites = new();

    void Start()
    {
        DrawToPercent(_targetPercent);
        SetCollider();
    }

    private void SetCollider()
    {
        _edgeCollider.Reset();
        var points = new List<Vector2>();
        for (int i = 0; i < _colliderRes; i++)
        {
            var percent = i / (float)_colliderRes;
            if (percent < _targetPercent)
            {
                var pos = _splineToFollow.EvaluatePosition(percent);
                pos = transform.InverseTransformPoint(pos);
                points.Add((Vector3)pos);
            }
        }

        _edgeCollider.SetPoints(points);
    }

    // be normal and make it an overload when you sober up
    private void DrawToPercentRefresh()
    {
        for (int i = 0; i < _sprites.Count; i++)
        {
            var sprite = _sprites[i];
            Destroy(sprite.sprite);
            _sprites.Remove(sprite);
        }
    }

    [ContextMenu("Update Vine")]
    private void DrawToPercent()
    {
        if (Application.isPlaying)
        {
            DrawToPercent(_targetPercent);
        }
    }

    private void DrawToPercent(float percent)
    {
        if (_sprites.Count != 0)
        {
            var last = _sprites[_sprites.Count - 1];

            while (last.percent > percent)
            {
                _sprites.Remove(last);
                Destroy(last.sprite);
                last = _sprites[_sprites.Count - 1];
            }
        }


        var length = _splineToFollow.CalculateLength();

        for (int i = 0; i <= _spawns; i++)
        {
            var p = i / _spawns;
            if (i < _sprites.Count)
            {
                var existing = _sprites[i];
                if (existing.percent >= p)
                {
                    continue;
                }
            }


            if (p <= percent)
            {
                var pos = _splineToFollow.EvaluatePosition(p);
                var r = _splineToFollow.EvaluateTangent(p);


                var rotation = new Vector3(0, 0,
                    _rotationOffset + Random.Range(_rotationRandomMin, _rotationRandomMax));
                Debug.Log(rotation);
                var sprite = Instantiate(_spritePrefab, transform);
                sprite.transform.position = pos;
                sprite.transform.up = r;
                sprite.transform.rotation =
                    Quaternion.Euler(sprite.transform.rotation.eulerAngles + rotation);

                _sprites.Add((sprite, p));
            }
        }

        SetCollider();
        _targetPercent = percent;
        _lastTarget = _targetPercent;
    }

    private float _lastTarget;

    void Update()
    {
        if (!Mathf.Approximately(_lastTarget, _targetPercent))
        {
            DrawToPercent(_targetPercent);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
        }
    }
}