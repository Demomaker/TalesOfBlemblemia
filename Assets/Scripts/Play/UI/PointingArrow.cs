using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class PointingArrow : MonoBehaviour
{
    [SerializeField] private int highestYPositionDifference = 10;
    [SerializeField] private int lowestYPositionDifference = -10;
    [SerializeField] private YDirection startingYDirection = YDirection.Up;
    [SerializeField] private Transform transformToPoint;
    private int yPositionDifference;
    private YDirection yDirection;

    private void Awake()
    {
        yDirection = startingYDirection;
        this.transform.position = transformToPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        doPointingMovement();
    }

    private void doPointingMovement()
    {
        switch (yDirection)
        {
            case YDirection.Up:
                yPositionDifference--;
                break;
            case YDirection.Down:
                yPositionDifference++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (yPositionDifference >= highestYPositionDifference)
        {
            yDirection = YDirection.Up;
        }

        if (yPositionDifference <= lowestYPositionDifference)
        {
            yDirection = YDirection.Down;
        }

        transform.position += transform.up * Math.Sign(yPositionDifference);
    }

    private enum YDirection
    {
        Up,
        Down
    }
}
