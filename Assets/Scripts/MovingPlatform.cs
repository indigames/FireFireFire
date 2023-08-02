
using System.Collections;
using UnityEngine;

public class MovingPlatform : ActivePlatform
{

    [Header("Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _moveTime;

    [SerializeField] private Vector3 _startMovePos;
    [SerializeField] private Vector3 _endMovePos;

    [SerializeField] private float _finishMoveOffset;

    private float _endMovingTime;
    private Vector3 _targetMovePos;
    private bool isStartPos = true;
    protected override IEnumerator CoActive()
    {
        _endMovingTime = Time.time + _moveTime;
        _targetMovePos = _startMovePos + _originPos;
        yield return null;
        while (Time.time < _endMovingTime)
        {

            if (Vector3.Distance(_objTransform.position, _targetMovePos) < _finishMoveOffset)
            {
                if (isStartPos)
                {
                    _targetMovePos = _endMovePos + _originPos;
                }
                else
                {
                    _targetMovePos = _startMovePos + _originPos;
                }
                isStartPos = !isStartPos;
            }
            _objTransform.position = Vector3.Lerp(_objTransform.position, _targetMovePos, .1f * Time.deltaTime * _moveSpeed);
            yield return null;
        }
    }

#if UNITY_EDITOR
    private Vector3 _originObjectPos;
    void OnValidate()
    {
        _originObjectPos = transform.position;
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_startMovePos + _originObjectPos, .5f);
        Gizmos.DrawWireSphere(_endMovePos + _originObjectPos, .5f);
    }
#endif
}
