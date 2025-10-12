using UnityEngine;

public class MoveRotate : MonoBehaviour
{
    [Header("ˆÚ“®")]
    [SerializeField] private Vector3 moveDirection = new Vector3(0,0,0); 
    [SerializeField] private bool bounceMovement = false;

    [Header("‰ñ“]")]
    [SerializeField] private Vector3 rotateSpeed = new Vector3(0, 0, 0); 
    [SerializeField] private bool bounceRotation = false; 

    private Vector3 startPosition;
    private Vector3 currentPosition;
    private Vector3 moveSign = Vector3.one;

    private Vector3 startRotation;
    private Vector3 currentRotation;
    private Vector3 rotateSign = Vector3.one;

    void Start()
    {
        startPosition = transform.position;
        currentPosition = startPosition;

        startRotation = transform.eulerAngles;
        currentRotation = startRotation;
    }

    void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector3 step = Vector3.Scale(moveDirection, moveSign) * Time.deltaTime;
        currentPosition += step;
        transform.position = currentPosition;

        if (bounceMovement)
        {
            if (Mathf.Abs(currentPosition.x - startPosition.x) >= Mathf.Abs(moveDirection.x))
                moveSign.x *= -1;
            if (Mathf.Abs(currentPosition.y - startPosition.y) >= Mathf.Abs(moveDirection.y))
                moveSign.y *= -1;
            if (Mathf.Abs(currentPosition.z - startPosition.z) >= Mathf.Abs(moveDirection.z))
                moveSign.z *= -1;
        }
    }

    private void Rotate()
    {
        Vector3 step = Vector3.Scale(rotateSpeed, rotateSign) * Time.deltaTime;
        currentRotation += step;
        transform.eulerAngles = currentRotation;

        if (bounceRotation)
        {
            if (Mathf.Abs(currentRotation.x - startRotation.x) >= Mathf.Abs(rotateSpeed.x))
                rotateSign.x *= -1;
            if (Mathf.Abs(currentRotation.y - startRotation.y) >= Mathf.Abs(rotateSpeed.y))
                rotateSign.y *= -1;
            if (Mathf.Abs(currentRotation.z - startRotation.z) >= Mathf.Abs(rotateSpeed.z))
                rotateSign.z *= -1;
        }
    }
}
