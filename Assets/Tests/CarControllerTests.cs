using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class CarControllerTests
{

    [Test]
    public void ResetStaticData_DoesReset()
    {
        void MyEventHandler(object sender, EventArgs e)
        {
            // Dummy
        }


        Assert.True(CarController.IsOnAnyPlayerSpawnedNull());
        CarController.OnAnyPlayerSpawned += MyEventHandler;
        Assert.False(CarController.IsOnAnyPlayerSpawnedNull());

        CarController.ResetStaticData();
        Assert.True(CarController.IsOnAnyPlayerSpawnedNull());
    }

    [Test]
    public void IsBreaking_ReturnsFalseIfSpeedAndBrakeForceZeroes()
    {
        CarController carController = new CarController();
        float speed = 0;
        float breakForce = 0;

        Assert.False(carController.IsBreaking(speed, breakForce));
    }

    [Test]
    public void IsBreaking_ReturnsFalseOnAcceletaring()
    {
        CarController carController = new CarController();
        float speed = 10;
        float breakForce = 0;

        Assert.False(carController.IsBreaking(speed, breakForce));
    }

    [Test]
    public void IsBreaking_ReturnsTrueIfBraking()
    {
        CarController carController = new CarController();
        float speed = 10;
        float breakForce = -5;

        Assert.True(carController.IsBreaking(speed, breakForce));
    }

    [Test]
    public void CalculateSpeed_ReturnsZeroWhenNotMoving()
    {
        var obj = new GameObject();
        obj.AddComponent<CarController>();
        CarController carController = obj.GetComponent<CarController>();

        Vector3 velocity = new Vector3(0, 0, 0);

        Assert.AreEqual(0, carController.CalculateSpeed(velocity));
    }

    [Test]
    public void CalculateSpeed_ReturnsSameSpeedIfMoving_Xaxis()
    {
        var obj = new GameObject();
        obj.AddComponent<CarController>();
        CarController carController = obj.GetComponent<CarController>();

        Vector3 velocity = new Vector3(10, 0, 0);

        Assert.AreEqual(10, carController.CalculateSpeed(velocity));
    }

    [Test]
    public void CalculateSpeed_ReturnsSameSpeedIfMoving_Zaxis()
    {
        var obj = new GameObject();
        obj.AddComponent<CarController>();
        CarController carController = obj.GetComponent<CarController>();

        Vector3 velocity = new Vector3(0, 0, 10);

        Assert.AreEqual(10, carController.CalculateSpeed(velocity));
    }

    [Test]
    public void ApplyBrakes_ReturnsSameBreakingForce()
    {
        var obj = new GameObject();
        obj.AddComponent<CarController>();
        CarController carController = obj.GetComponent<CarController>();
        carController.ApplyBrakes(0);


        Assert.AreEqual(0, carController.currentBrakeForce);
    }
    [Test]
    public void ApplyAcceleration_ReturnSameAcceleration()
    {
        var obj = new GameObject();
        obj.AddComponent<CarController>();
        CarController carController = obj.GetComponent<CarController>();
        carController.ApplyAcceleration(0);


        Assert.AreEqual(0, carController.currentAcceleration);
    }
    [Test]
    public void ApplySteering_ReturnSameStearing()
    {
        var obj = new GameObject();
        obj.AddComponent<CarController>();
        CarController carController = obj.GetComponent<CarController>();
        carController.ApplySteering(0);


        Assert.AreEqual(0, carController.currentTurnAngle);
    }

}
