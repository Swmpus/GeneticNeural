using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithmClasses;
using System;

public class WorldController : MonoBehaviour {
    [SerializeField]
    private GameObject Char;
    private System.Random rnd;
    private GeneticAlgorithm Gen;
    private List<GameObject> Chars;
    private float EndCheckTime;
    private float MaxCheckTime;
    private int RestartCalls;

	void Start ()
    {
        Chars = new List<GameObject>();
        rnd = new System.Random();
        Gen = new GeneticAlgorithm(100, 0.0, 56, new List<int>() { 5, 4, 1 }, rnd);

        RestartCalls = 0;

        MaxCheckTime = 10f;
        EndCheckTime = 10f;

        for (int i = 0; i < Gen.Pop.Individuals.Count; i++) {
            Chars.Add(Instantiate(Char, new Vector3(35, 1, 0), new Quaternion(0, 0, 0, 0), null));
        }

        for (int i = 0; i < Chars.Count; i++) {
            Chars[i].GetComponent<PlayerController>().Brain = Gen.Pop.Individuals[i].Brain;
            Chars[i].GetComponent<PlayerController>().running = true;
        }
    }

    private void ReInit()
    {
        for (int i = 0; i < Gen.Pop.Individuals.Count; i++) {
            Gen.Pop.Individuals[i].Fitness = Chars[i].GetComponent<PlayerController>().Fitness;
        }

        Debug.Log("This Generation Had Average Fitness: " + Gen.Pop.GetAverageFitness());
        Gen.NextGeneration(rnd);

        for (int i = Chars.Count - 1; i >= 0; i--) {
            Destroy(Chars[i]);
            Chars.RemoveAt(i);
        }
        if (Chars.Count > 0) {
            throw new Exception("Chars Not Completely Cleaned Up");
        }
        Chars = new List<GameObject>();

        for (int i = 0; i < Gen.Pop.Individuals.Count; i++) {
            Chars.Add(Instantiate(Char, new Vector3(35, 1, 0), new Quaternion(0, 0, 0, 0), null));
        }

        for (int i = 0; i < Chars.Count; i++) {
            Chars[i].GetComponent<PlayerController>().Brain = Gen.Pop.Individuals[i].Brain;
            Chars[i].GetComponent<PlayerController>().running = true;
        }
    }

    private bool CheckForRestart()
    {
        for (int i = 0; i < Chars.Count; i++) {
            if (Chars[i].GetComponent<PlayerController>().running) {
                return false;
            }
        }
        return true;
    }

    void Update ()
    {
        EndCheckTime -= Time.deltaTime;

        if (EndCheckTime <= 0.0f) {
            RestartCalls += 1;

            if (RestartCalls == 1 && CheckForRestart()) {
                ReInit();
            }
            RestartCalls -= 1;
            EndCheckTime = MaxCheckTime;
        }
    }
}