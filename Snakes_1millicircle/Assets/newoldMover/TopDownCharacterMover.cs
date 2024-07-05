using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TopDownCharacterMover : MonoBehaviour
{
    private InputHandler2 _input;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private bool rotateTowardsMouse;
    public float MoveSpeed() {
        if(_input.InputVector!=Vector2.zero)
        {
            return 1;
        }
        return 0;
    }
    private void Awake()
    {
        _input = GetComponent<InputHandler2>();
    }
    void Update()
    {
        var targetVector = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);

        var movementVector = MoveTowardTarget(targetVector);
        if (!rotateTowardsMouse)
            RotateTowardMovementVector(movementVector);
        else
            RotateTowardMouseVector();
    }
    private void RotateTowardMouseVector()
    {
        Ray ray = Camera.main.ScreenPointToRay(_input.MousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }
    private void RotateTowardMovementVector(Vector3 movementVector)
    {
        if (movementVector.magnitude == 0) { return; }

        var rotation = Quaternion.LookRotation(movementVector);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
    }

    private Vector3 MoveTowardTarget(Vector3 targetVector)
    {
        var speed = moveSpeed * Time.deltaTime;

        targetVector = Quaternion.Euler(0, Camera.main.gameObject.transform.eulerAngles.y, 0) * targetVector;
        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
        return targetVector;
    }
}
