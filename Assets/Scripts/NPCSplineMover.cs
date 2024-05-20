using UnityEngine;
using DG.Tweening;
using System;

public class NPCSplineMover : MonoBehaviour
{
    private Animator _animator;
    public Transform[] pathPoints;
    public float moveSpeed = 5f;
    public float waitTime = 2f;
    private bool isReturning = false;
    private Action onCompleteCallback;

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator component is not found on " + gameObject.name);
        }
    }

    public void Move(bool reverse = false, Action onComplete = null)
    {
        if (pathPoints == null || pathPoints.Length == 0)
        {
            Debug.LogError("Path points are not assigned.");
            return;
        }

        isReturning = reverse;
        onCompleteCallback = onComplete;
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        Sequence sequence = DOTween.Sequence();

        Transform[] points = isReturning ? ReverseArray(pathPoints) : pathPoints;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 nextPosition = points[i].position;
            Quaternion nextRotation;

            if (i < points.Length - 1)
            {
                nextRotation = Quaternion.LookRotation(GetDirectionToNextPoint(points, i));
            }
            else
            {
                if (isReturning)
                {
                    Vector3 eulerAngles = points[i].rotation.eulerAngles;
                    eulerAngles.y += 180f;
                    nextRotation = Quaternion.Euler(eulerAngles);
                }
                else
                {
                    nextRotation = points[i].rotation;
                }
            }

            sequence.Append(transform
                .DOMove(nextPosition, Vector3.Distance(transform.position, nextPosition) / moveSpeed)
                .SetEase(Ease.Linear));
            sequence.Join(transform
                .DORotateQuaternion(nextRotation, Vector3.Distance(transform.position, nextPosition) / moveSpeed)
                .SetEase(Ease.Linear));

            if (i < points.Length - 1)
            {
                sequence.AppendInterval(waitTime);
            }
        }

        sequence.OnComplete(() =>
        {
            _animator.SetBool("isWalking", false);
            Debug.Log("Finished moving along path!");
            onCompleteCallback?.Invoke();
        });

        sequence.Play();
        _animator.SetBool("isWalking", true);
    }

    private Vector3 GetDirectionToNextPoint(Transform[] points, int index)
    {
        if (index < points.Length - 1)
        {
            return (points[index + 1].position - points[index].position).normalized;
        }

        if (index > 0)
        {
            return (points[index].position - points[index - 1].position).normalized;
        }

        return transform.forward;
    }

    private Transform[] ReverseArray(Transform[] array)
    {
        Transform[] reversedArray = new Transform[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            reversedArray[i] = array[array.Length - 1 - i];
        }

        return reversedArray;
    }
}