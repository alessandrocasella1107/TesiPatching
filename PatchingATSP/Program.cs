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



            EseguiAlgoritmo(10, 10, list_errori10nodi);
            EseguiAlgoritmo(10, 15, list_errori15nodi);
            EseguiAlgoritmo(10, 20, list_errori20nodi);
            EseguiAlgoritmo(10, 25, list_errori25nodi);
            EseguiAlgoritmo(10, 30, list_errori30nodi);
            EseguiAlgoritmo(10, 35, list_errori35nodi);
            EseguiAlgoritmo(10, 40, list_errori40nodi);
            EseguiAlgoritmo(10, 45, list_errori45nodi);
            EseguiAlgoritmo(10, 50, list_errori50nodi);


            int[] nodi = { 10, 15, 20, 25, 30, 35, 40, 45, 50 };
            double[] errori = { CalculateAverage(list_errori10nodi),
                            CalculateAverage(list_errori15nodi),
                            CalculateAverage(list_errori20nodi),
                            CalculateAverage(list_errori25nodi),
                            CalculateAverage(list_errori30nodi),
                            CalculateAverage(list_errori35nodi),
                            CalculateAverage(list_errori40nodi),
                            CalculateAverage(list_errori45nodi),
                            CalculateAverage(list_errori50nodi) };

            Console.WriteLine("{0,-15} {1,-15}", "Numero nodi", "Errore medio");

            for (int i = 0; i < nodi.Length; i++)
            {
                Console.WriteLine("{0,-15} {1,-15}%", nodi[i], errori[i]);
            }
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

            return (sum / numbers.Count) * 100;
        }


        public static void EseguiAlgoritmo(int numeroIterazioni, int dimensioneMatrice, List<double> listaErrori)
        {
            for (int i = 0; i < numeroIterazioni; i++)
            {
                var c = GeneratoreMatrici.GenerateMatrix(dimensioneMatrice);
                var soluzioneAP = SolveAPCplex.solve(dimensioneMatrice, c);
                double result = (soluzioneAP.Item2 - soluzioneAP.Item1) / soluzioneAP.Item1;
                listaErrori.Add(result);
            }
        }


    }
}