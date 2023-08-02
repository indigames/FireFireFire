using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RolatingPlatform : ActivePlatform
{
    [SerializeField] private Vector3 _rotateSpeed;
    [SerializeField] private float _rotateTime;

    private float _endRotatingTime;

    protected override IEnumerator CoActive()
    {
        _endRotatingTime = Time.time + _rotateTime;

        yield return null;
        while (Time.time < _endRotatingTime)
        {
            _objTransform.eulerAngles += _rotateSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
