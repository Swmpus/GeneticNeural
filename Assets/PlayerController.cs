using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NueralNetworkClasses;

public class PlayerController : MonoBehaviour
{
    public NeuralNetwork Brain;
    public bool running = false;
    public double Fitness = 0;

    private int LM;
    private int LapCount = 0;
    private float movementSpeed = 15f;
    private float rotationSpeed = 90f;
    private List<double> Out;

    private void Start()
    {
        LM = ~(1 << 8);
        LM |= ~(1 << 11);
    }

    void Update ()
    {
        if (running) {
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            var L = new RaycastHit();
            var FL = new RaycastHit();
            var F = new RaycastHit();
            var FR = new RaycastHit();
            var R = new RaycastHit();

            var Left = new Ray(transform.position, transform.TransformDirection(Vector3.left));
            var ForwardLeft = new Ray(transform.position, transform.TransformDirection(Vector3.left + Vector3.forward));
            var Forward = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
            var ForwardRight = new Ray(transform.position, transform.TransformDirection(Vector3.right + Vector3.forward));
            var Right = new Ray(transform.position, transform.TransformDirection(Vector3.right));

            Physics.Raycast(Left, out L, 1000f, LM, QueryTriggerInteraction.Ignore);
            Physics.Raycast(ForwardLeft, out FL, 1000f, LM, QueryTriggerInteraction.Ignore);
            Physics.Raycast(Forward, out F, 1000f, LM, QueryTriggerInteraction.Ignore);
            Physics.Raycast(ForwardRight, out FR, 1000f, LM, QueryTriggerInteraction.Ignore);
            Physics.Raycast(Right, out R, 1000f, LM, QueryTriggerInteraction.Ignore);
            /*
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 100f, Color.black, 0f);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left + Vector3.forward) * 100f, Color.black, 0f);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100f, Color.black, 0f);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right + Vector3.forward) * 100f, Color.black, 0f);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 100f, Color.black, 0f);
            */
            Fitness += Time.deltaTime * 0.4 * ((L.distance + FL.distance + F.distance + FR.distance + R.distance) / 5.0);
            Out = Brain.Run(new List<double>() { L.distance, FL.distance, F.distance, FR.distance, R.distance });
            if (Out[0] < 0) {
                RotateLeft();
            } else if (Out[0] > 0) {
                RotateRight();
            }
        }
    }

    private void RotateLeft()
    {
        transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void RotateRight()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11) {
            LapCount += 1;
            Fitness *= 2;
        }
        if (LapCount == 2) {
            running = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer ==  9) {
            running = false;
        }
    }
}