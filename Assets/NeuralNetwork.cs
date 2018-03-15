using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NueralNetworkClasses
{
    public static class NNHelper
    {
        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
    }

    public class Node
    {
        public List<double> Connectors;
        public double Bias;
        public double Value;

        public Node(int NodesInNextLayer) {
            Connectors = new List<double>();

            for (int i = 0; i < NodesInNextLayer; i++) {
                Connectors.Add(1.0);
            }
        }
    }

    public class Layer
    {
        public List<Node> Nodes;
        
        public Layer(int InNodeCount, int NodesInNextLayer)
        {
            Nodes = new List<Node>();

            for (int i = 0; i < InNodeCount; i++) {
                Nodes.Add(new Node(NodesInNextLayer));
            }
        }
    }

    public class NeuralNetwork
    {
        public List<Layer> Layers;

        public NeuralNetwork(List<int> LayerCounts, System.Random rnd) {
            Layers = new List<Layer>();

            for (int i = 0; i < LayerCounts.Count - 1; i++) {
                Layers.Add(new Layer(LayerCounts[i], LayerCounts[i + 1]));
            }
            Layers.Add(new Layer(LayerCounts[LayerCounts.Count - 1], 0));

            for (int i = 0; i < Layers.Count; i++) {
                for (int j = 0; j < Layers[i].Nodes.Count; j++) {
                    Layers[i].Nodes[j].Bias = rnd.NextDouble() * 5 - 2.5;

                    for (int q = 0; q < Layers[i].Nodes[j].Connectors.Count; q++) {
                        Layers[i].Nodes[j].Connectors[q] = rnd.NextDouble() * 5 - 2.5;
                    }
                }
            }
        }

        public List<double> Run(List<double> Inputs)
        {
            var output = new List<double>();

            for (int i = 0; i < Layers[0].Nodes.Count; i++) {
                Layers[0].Nodes[i].Value = Inputs[i] * Layers[0].Nodes[i].Bias;
            }
            for (int i = 1; i < Layers.Count; i++) {
                for (int q = 0; q < Layers[i].Nodes.Count; q++) {
                    Layers[i].Nodes[q].Value = 0;

                    for (int j = 0; j < Layers[i - 1].Nodes.Count; j++) {
                        Layers[i].Nodes[q].Value += Layers[i - 1].Nodes[j].Connectors[q] * Layers[i - 1].Nodes[j].Value * Layers[i].Nodes[q].Bias;
                    }
                }
            }
            for (int i = 0; i < Layers[Layers.Count - 1].Nodes.Count; i++) {
                output.Add(NNHelper.Sigmoid(Layers[Layers.Count - 1].Nodes[i].Value) - 0.5);
            }
            return output;
        }
    }
}