using NueralNetworkClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithmClasses
{
    public class Population
    {
        public List<Individual> Individuals;
        public int MaxCount;

        public Population(int Count, List<int> LayerCounts, System.Random rnd)
        {
            Individuals = new List<Individual>();
            MaxCount = Count;

            for (int i = 0; i < Count; i++) {
                Individuals.Add(new Individual(LayerCounts, rnd));
            }
        }

        public double GetAverageFitness()
        {
            double totalFitness = 0.0;

            for (int i = 0; i < Individuals.Count; i++) {
                totalFitness += Individuals[i].Fitness;
            }
            return (totalFitness / Individuals.Count);
        }

        public void Mutate(System.Random rnd, double MutationThresh)
        {
            for (int i = 0; i < Individuals.Count; i++) {
                Individuals[i].Mutate(rnd, MutationThresh);
            }
        }
    }

    public class Individual
    {
        public double Fitness;
        public NeuralNetwork Brain;

        public Individual(List<int> LayerCounts, System.Random rnd)
        {
            Fitness = 0;
            Brain = new NeuralNetwork(LayerCounts, rnd);
        }

        public void Mutate(System.Random rnd, double thresh) {
            for (int i = 0; i < Brain.Layers.Count; i++) {
                for (int j = 0; j < Brain.Layers[i].Nodes.Count; j++) {
                    if (rnd.NextDouble() <= thresh) {
                        Brain.Layers[i].Nodes[j].Bias = rnd.NextDouble() * 5 - 2.5;
                    }
                    for (int q = 0; q < Brain.Layers[i].Nodes[j].Connectors.Count; q++) {
                        if (rnd.NextDouble() <= thresh) {
                            Brain.Layers[i].Nodes[j].Connectors[q] = rnd.NextDouble() * 5 - 2.5;
                        }
                    }
                }
            }
        }
    }

    public class GeneticAlgorithm
    {
        public Population Pop;
        private double mutationRate;
        private int deathRate;
        private List<int> Layers;

        public GeneticAlgorithm(int PopCount, double inMutation, int inDeath, List<int> LayerCounts, System.Random rand)
        {
            Pop = new Population(PopCount, LayerCounts, rand);
            mutationRate = inMutation;
            Layers = LayerCounts;
            deathRate = inDeath;

            if (deathRate < (PopCount / 2)) {
                throw new Exception("Death rate should be > (PopCount / 2)");
            //} else if (elitism >= PopCount || elitism < 0) {
                //throw new Exception("Elitism should be 0 <= elitism < PopCount");
            } else if (mutationRate > 1 || mutationRate < 0) {
                throw new Exception("Mutation Rate should be 0 <= M <= 1");
            }
        }

        public void NextGeneration(System.Random rand)
        {
            Pop.Individuals = SortList(Pop.Individuals, 0, Pop.Individuals.Count - 1);

            int Total = Pop.Individuals.Count - deathRate;

            Kill();
            Total += Crossover(Pop.Individuals, rand);
            Rebirth(Total, rand);
            Pop.Mutate(rand, mutationRate);
        }

        private void Kill()
        {
            /*
            var ToRemove = new List<Individual>();
            for (int i = 0; i < deathRate; i++) {
                ToRemove.Add(Pop.Individuals[i]);
            }
            for (int i = 0; i < ToRemove.Count; i++) {
                Pop.Individuals.Remove(ToRemove[i]);
            }
            */
            
            int C = Pop.Individuals.Count - 1;
            for (int i = C; i > (C - deathRate); i--) {
                Pop.Individuals.RemoveAt(i);
            }
            
        }

        private void Rebirth(int StartCount, System.Random rnd)
        {
            if (StartCount > Pop.MaxCount) {
                throw new Exception("Too many new individuals have ben added to the population");
            }
            for (int i = StartCount; i < Pop.MaxCount; i++) {
                Pop.Individuals.Add(new Individual(Layers, rnd));
            }
            if (Pop.Individuals.Count != Pop.MaxCount) {
                throw new Exception("Incorrect Population Size: " + Pop.MaxCount);
            }
        }

        private int Crossover( List<Individual> Popp, System.Random rand)
        {
            int Count = 0;
            for (int i = 0; i < Pop.MaxCount - deathRate - 1; i += 2) {
                Count += 2;
                var output = CrossoverTwoIndividuals(i, i + 1, Popp, rand);
                Pop.Individuals.Add(output[0]);
                Pop.Individuals.Add(output[1]);
            }
            return Count;
        }

        private Individual[] CrossoverTwoIndividuals(int PointerOne, int PointerTwo,List<Individual> Popp, System.Random rnd)
        {
            int Changed = 0;
            int Total = 0;

            var Parents = new Individual[2] { new Individual(Layers, rnd), new Individual(Layers, rnd) };
            var Children = new Individual[2] { new Individual(Layers, rnd), new Individual(Layers, rnd) };

            for (int i = 0; i < Popp[0].Brain.Layers.Count; i++) {
                for (int j = 0; j < Popp[0].Brain.Layers[i].Nodes.Count; j++) {
                    for (int q = 0; q < Popp[0].Brain.Layers[i].Nodes[j].Connectors.Count; q++) {
                        Total += 1;
                    }

                    Parents[0].Brain.Layers[i].Nodes[j] = Popp[PointerOne].Brain.Layers[i].Nodes[j];
                    Parents[1].Brain.Layers[i].Nodes[j] = Popp[PointerTwo].Brain.Layers[i].Nodes[j];

                    Children[0].Brain.Layers[i].Nodes[j] = Popp[PointerOne].Brain.Layers[i].Nodes[j];
                    Children[1].Brain.Layers[i].Nodes[j] = Popp[PointerTwo].Brain.Layers[i].Nodes[j];
                }
            }

            for (int i = 0; i < Parents[0].Brain.Layers.Count; i++) {
                for (int j = 0; j < Parents[0].Brain.Layers[i].Nodes.Count; j++) {
                    if (rnd.NextDouble() <= 0.5) {
                        Children[0].Brain.Layers[i].Nodes[j].Bias = Parents[1].Brain.Layers[i].Nodes[j].Bias;
                        Children[1].Brain.Layers[i].Nodes[j].Bias = Parents[0].Brain.Layers[i].Nodes[j].Bias;

                        Changed += 1;
                    }
                    if (Changed >= Total / 2) {
                        return Children;
                    }
                    for (int q = 0; q < Parents[0].Brain.Layers[i].Nodes[j].Connectors.Count; q++) {
                        if (rnd.NextDouble() <= 0.5) {
                            Children[0].Brain.Layers[i].Nodes[j].Connectors[q] = Parents[1].Brain.Layers[i].Nodes[j].Connectors[q];
                            Children[1].Brain.Layers[i].Nodes[j].Connectors[q] = Parents[0].Brain.Layers[i].Nodes[j].Connectors[q];

                            Changed += 1;
                        }
                        if (Changed >= Total / 2) {
                            return Children;
                        }
                    }
                }
            }
            return Children;
        }

        private List<Individual> SortList(List<Individual> elements, int left, int right)
        { // QuickSort
            int i = left;
            int j = right;
            Individual pivot = elements[(left + right) / 2];

            while (i <= j) {
                while (elements[i].Fitness > pivot.Fitness) {
                    i++;
                }

                while (elements[j].Fitness < pivot.Fitness) {
                    j--;
                }

                if (i <= j) {
                    Individual tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }
            
            if (left < j) {
                SortList(elements, left, j);
            }

            if (i < right) {
                SortList(elements, i, right);
            }
            return elements;
        }
    }
}