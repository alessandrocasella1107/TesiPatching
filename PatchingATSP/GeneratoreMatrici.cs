using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchingATSP
{
    public class GeneratoreMatrici
    {
        private static Random random = new Random();

        public static double[][] GenerateMatrix(int numNodes)
        {
            double[][] matrix = new double[numNodes][];

            for (int i = 0; i < numNodes; i++)
            {
                matrix[i] = new double[numNodes];
                for (int j = 0; j < numNodes; j++)
                {
                    if (i == j)
                    {
                        matrix[i][j] = double.MaxValue;
                    }
                    else
                    {
                        matrix[i][j] = random.Next(5, 50);
                    }
                }
            }

            return matrix;
        }

        public static void PrintMatrix(double[][] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    Console.Write(matrix[i][j] + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}
