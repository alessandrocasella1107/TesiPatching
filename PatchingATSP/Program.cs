using ILOG.CPLEX;
using PatchingATSP;
using System;
using System.Collections.Generic;

namespace ATSP_Patching
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double> list_errori10nodi = new List<double>();
            List<double> list_errori15nodi = new List<double>();
            List<double> list_errori20nodi = new List<double>();
            List<double> list_errori25nodi = new List<double>();
            List<double> list_errori30nodi = new List<double>();
            List<double> list_errori35nodi = new List<double>();
            List<double> list_errori40nodi = new List<double>();
            List<double> list_errori45nodi = new List<double>();
            List<double> list_errori50nodi = new List<double>();

            int n = 9;
            double[][] c = new double[][]
            {
                new double[] { double.MaxValue, 7, 3, 4, 10, 6, 7, 7, 4 },
                new double[] { 9, double.MaxValue, 10, 6, 9, 5 , 4, 4, 6 },
                new double[] { 5, 4, double.MaxValue, 1, 10, 6, 7, 5, 4 },
                new double[] { 4, 8, 7, double.MaxValue, 9, 8, 9, 10, 8 },
                new double[] { 6, 5, 9, 5, double.MaxValue, 10, 6, 6, 3 },
                new double[] { 8, 3, 5, 4, 8, double.MaxValue, 7, 5, 8 },
                new double[] { 5, 5, 7, 7, 6, 8,double.MaxValue, 3, 6 },
                new double[] { 6, 3, 9, 5, 12, 8, 7, double.MaxValue, 7 },
                new double[] { 5, 6, 8, 8, 6, 9, 3, 3, double.MaxValue},
            };



            var soluzioneAP = SolveAPCplex.solve(n, c);
            double result = (soluzioneAP.Item2 - soluzioneAP.Item1) / soluzioneAP.Item1;

            Console.WriteLine($"La deviazione dal lower bound è: {result}");
            //EseguiAlgoritmo(10, 10, list_errori10nodi);
            //EseguiAlgoritmo(10, 15, list_errori15nodi);
            //EseguiAlgoritmo(10, 20, list_errori20nodi);
            //EseguiAlgoritmo(10, 25, list_errori25nodi);
            //EseguiAlgoritmo(10, 30, list_errori30nodi);
            //EseguiAlgoritmo(10, 35, list_errori35nodi);
            //EseguiAlgoritmo(10, 40, list_errori40nodi);
            //EseguiAlgoritmo(10, 45, list_errori45nodi);
            //EseguiAlgoritmo(10, 50, list_errori50nodi);

            //Console.WriteLine("L'errore medio in 10 matrici composte da 10 nodi è: " + CalculateAverage(list_errori10nodi));
            //Console.WriteLine("L'errore medio in 10 matrici composte da 15 nodi è: " + CalculateAverage(list_errori15nodi));
            //Console.WriteLine("L'errore medio in 10 matrici composte da 20 nodi è: " + CalculateAverage(list_errori20nodi));
            //Console.WriteLine("L'errore medio in 10 matrici composte da 25 nodi è: " + CalculateAverage(list_errori25nodi));
            //Console.WriteLine("L'errore medio in 10 matrici composte da 30 nodi è: " + CalculateAverage(list_errori30nodi));
            //Console.WriteLine("L'errore medio in 10 matrici composte da 35 nodi è: " + CalculateAverage(list_errori35nodi));
            //Console.WriteLine("L'errore medio in 10 matrici composte da 40 nodi è: " + CalculateAverage(list_errori40nodi));
            //Console.WriteLine("L'errore medio in 10 matrici composte da 45 nodi è: " + CalculateAverage(list_errori45nodi));
            //Console.WriteLine("L'errore medio in 10 matrici composte da 50 nodi è: " + CalculateAverage(list_errori50nodi));
        }



        public static double CalculateAverage(List<double> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
                throw new ArgumentException("La lista dei numeri è vuota o nulla.");
            }

            double sum = 0;
            foreach (double number in numbers)
            {
                sum += number;
            }

            return sum / numbers.Count;
        }


        public static void EseguiAlgoritmo(int numeroIterazioni, int dimensioneMatrice, List<double> listaErrori)
        {
            for (int i = 0; i < numeroIterazioni; i++)
            {
                var c = GeneratoreMatrici.GenerateMatrix(dimensioneMatrice);
                //var soluzionePatching = SolveATSPPatchCplex.solve(dimensioneMatrice, c);
                var soluzioneAP = SolveAPCplex.solve(dimensioneMatrice, c);
                double result = (soluzioneAP.Item2 - soluzioneAP.Item1) / soluzioneAP.Item1;
                //Console.WriteLine($"La deviazione dal lower bound è: {result}");
                listaErrori.Add(result);
            }
        }


    }
}