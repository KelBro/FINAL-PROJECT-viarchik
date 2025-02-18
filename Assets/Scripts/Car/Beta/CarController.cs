using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    // Настройки
    public float MoveSpeed = 50; // Скорость движения
    public float MaxSpeed = 15; // Максимальная скорость
    public float Drag = 0.98f; // Сопротивление движению
    public float SteerAngle = 20; // Угол поворота
    public float Traction = 1; // Сцепление

// Переменные
    private Vector3 MoveForce; // Сила движения

// Update вызывается каждый кадр
    void Update()
    {

        // Движение
        MoveForce += transform.forward * MoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += MoveForce * Time.deltaTime;

        // Управление
        float steerInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerInput * MoveForce.magnitude * SteerAngle * Time.deltaTime);

        // Сопротивление и ограничение максимальной скорости
        MoveForce *= Drag;
        MoveForce = Vector3.ClampMagnitude(MoveForce, MaxSpeed);

        // Сцепление
        Debug.DrawRay(transform.position, MoveForce.normalized * 3); // Отрисовка луча для отладки
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue); // Отрисовка луча для отладки
        MoveForce = Vector3.Lerp(MoveForce.normalized, transform.forward, Traction * Time.deltaTime) *
                    MoveForce.magnitude;
    }
}
