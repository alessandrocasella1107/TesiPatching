using ATSP_Patching;
using ILOG.Concert;
using ILOG.CPLEX;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.AccessControl;

namespace PatchingATSP
{
    public class SolveAPCplex
    {
        public static Tuple<double, double> solve(int n, double[][] m)
        {
            try
            {
                using (Cplex cplex = new Cplex())
                {
                    // Variabili
                    //x matrice di variabili booleane che rappresenta se un arco tra due nodi è selezionato
                    //u array di variabili continue che rappresentano il costo accumulato
                    INumVar[][] x = new INumVar[n][];
                    for (int i = 0; i < n; i++)
                    {
                        x[i] = cplex.BoolVarArray(n);
                    }
                    INumVar[] u = new INumVar[n];
                    for (int i = 0; i < n; i++)
                    {
                        u[i] = cplex.NumVar(0, double.MaxValue);
                    }

                    // Obiettivo
                    //Per ogni coppia di nodi (i, j), si crea una variabile x[i][j] e si aggiunge il termine corrispondente alla funzione obiettivo moltiplicando il costo dell'arco (i, j) per la variabile x[i][j].
                    ILinearNumExpr objective = cplex.LinearNumExpr();
                    for (int i = 0; i < n; i++)
                    {
                        int pos_i = i + 1;
                        for (int j = 0; j < n; j++)
                        {
                            int pos_j = j + 1;
                            x[i][j] = cplex.BoolVar($"x[{pos_i}][{pos_j}]");
                            objective.AddTerm(m[i][j], x[i][j]);
                        }
                    }
                    cplex.AddMinimize(objective);

                    // Vincoli

                    //vincoli di grado primi due
                    for (int j = 0; j < n; j++)
                    {
                        ILinearNumExpr exp = cplex.LinearNumExpr();
                        for (int i = 0; i < n; i++)
                        {
                            if (i != j)
                            {
                                exp.AddTerm(1, x[i][j]);
                            }
                        }
                        cplex.AddEq(exp, 1);
                    }

                    for (int i = 0; i < n; i++)
                    {
                        ILinearNumExpr exp = cplex.LinearNumExpr();
                        for (int j = 0; j < n; j++)
                        {
                            if (j != i)
                            {
                                exp.AddTerm(1, x[i][j]);
                            }
                        }
                        cplex.AddEq(exp, 1);
                    }


                    if (cplex.Solve())
                    {


                        Console.WriteLine();
                        Console.WriteLine("Solution status = " + cplex.GetStatus());
                        Console.WriteLine();
                        Console.WriteLine("Optimal value = " + cplex.ObjValue);
                        Console.WriteLine();
                        double[][] soluzione = new double[n][];
                        for (int i = 0; i < n; i++)
                        {
                            soluzione[i] = new double[n];
                            for (int j = 0; j < n; j++)
                            {

                                if (x[i][j] != null && cplex.GetValue(x[i][j]) != 0)
                                {
                                    soluzione[i][j] = 1;
                                }
                            }
                        }
                        var valoreOttimoAP = cplex.ObjValue;
                        var valoreOttimoPatch = 0.0;
                        // Applicare il patching
                        var sottocicli = TrovaSottocicli(soluzione);
                        List<Tuple<int, int>> ciclo = new List<Tuple<int, int>>();
                        if (sottocicli.Count() != 1)
                        {
                            ciclo = PatchingMerge(m, sottocicli);
                            valoreOttimoPatch = CalculateCost(ciclo, m);
                        }
                        else
                        {
                            valoreOttimoPatch = CalculateCost(sottocicli[0], m);
                        }


                        return Tuple.Create(valoreOttimoAP,valoreOttimoPatch);
                    }

                    return null;
                }
            }
            catch (ILOG.Concert.Exception exc)
            {
                Console.WriteLine("Concert exception caught: " + exc);
                return null;
            }
        }

        static List<List<Tuple<int, int>>> TrovaSottocicli(double[][] soluzione)
        {
            var sottocicli = new List<List<Tuple<int, int>>>();
            var visitati = new HashSet<int>();


            if (IsUniqueCycle(soluzione))
            {
                List<Tuple<int, int>> rigaLista = new List<Tuple<int, int>>();
                for (int i = 0; i < soluzione.Length; i++)
                {
                    for (int j = 0; j < soluzione[i].Length; j++)
                    {
                        if (soluzione[i][j] == 1)
                        {
                            rigaLista.Add(new Tuple<int, int>(i, j));
                        }
                    }
                    
                }
                sottocicli.Add(rigaLista);
            }
            else
            {
                for (int i = 0; i < soluzione.Length; i++)
                {
                    if (!visitati.Contains(i))
                    {
                        var sottociclo = new List<Tuple<int, int>>();
                        TrovaSottociclo(i, i, soluzione, visitati, sottociclo);
                        if (sottociclo.Count > 0)
                        {
                            sottocicli.Add(sottociclo);
                        }
                    }
                }
            }

            return sottocicli;
        }

        static bool TrovaSottociclo(int nodoCorrente, int nodoIniziale, double[][] soluzione, HashSet<int> visitati, List<Tuple<int, int>> sottociclo)
        {
            for (int j = 0; j < soluzione[nodoCorrente].Length; j++)
            {
                if (soluzione[nodoCorrente][j] != 0)
                {
                    if (j == nodoIniziale)
                    {
                        sottociclo.Add(Tuple.Create(nodoCorrente, j));
                        return true;
                    }

                    if (!visitati.Contains(j))
                    {
                        visitati.Add(j);
                        sottociclo.Add(Tuple.Create(nodoCorrente, j));
                        if (TrovaSottociclo(j, nodoIniziale, soluzione, visitati, sottociclo))
                        {
                            return true;
                        }
                        sottociclo.Remove(Tuple.Create(nodoCorrente, j));
                    }
                }
            }
            return false;
        }


        static List<Tuple<int, int>> PatchingMerge(double[][] costMatrix, List<List<Tuple<int, int>>> subcycles)
        {
            subcycles.Reverse();
            List<Tuple<int, int>> mergedSubcycle = new List<Tuple<int, int>>();
            if (subcycles.Count() == 1)
            {
                mergedSubcycle = subcycles[0];
            }

            while (subcycles.Count() !=1 )
            {

                List<Tuple<int, int>> largestSubcycle = new List<Tuple<int, int>>();
                List<Tuple<int, int>> secondLargestSubcycle = new List<Tuple<int, int>>();

                foreach (var cycle in subcycles)
                {
                    if (cycle.Count > largestSubcycle.Count)
                    {
                        secondLargestSubcycle = new List<Tuple<int, int>>(largestSubcycle);
                        largestSubcycle = new List<Tuple<int, int>>(cycle);
                    }
                    else if (cycle.Count > secondLargestSubcycle.Count)
                    {
                        secondLargestSubcycle = new List<Tuple<int, int>>(cycle);
                    }
                }
                mergedSubcycle = MergeSubcycles(largestSubcycle, secondLargestSubcycle, costMatrix);

                subcycles.RemoveAll(cycle => AreListsEqual(cycle, largestSubcycle) || AreListsEqual(cycle, secondLargestSubcycle));



                subcycles.Add(mergedSubcycle);

            }
            
            return mergedSubcycle;


        }



        static bool AreListsEqual<T>(List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                    return false;
            }

            return true;
        }


        static List<Tuple<int, int>> MergeSubcycles(List<Tuple<int, int>> cycle2, List<Tuple<int, int>> cycle1, double[][] costMatrix)
        {
            double cost = double.MaxValue;
            List<Tuple<int, int>> def = new List<Tuple<int, int>>();


            for (int i = 0; i<cycle1.Count; i++)
            {
                var a = cycle1[i].Item1;
                var b = cycle1[i].Item2;
                double costo_;
                
                for (int j = 0; j<cycle2.Count; j++)
                {
                    List<Tuple<int, int>> nuova = new List<Tuple<int, int>>();
                    var c = cycle2[j].Item1;
                    var d = cycle2[j].Item2;

                    nuova.Add(Tuple.Create(a, d));
                    nuova.Add(Tuple.Create(c, b));

                    for (int s1 = 0; s1 < cycle1.Count; s1++) 
                    {
                        if (cycle1[s1].Item1 != a || cycle1[s1].Item2!=b)
                        {
                            nuova.Add(cycle1[s1]);
                        }

                    }
                    for (int s2= 0; s2 < cycle2.Count; s2++)
                    {
                        if (cycle2[s2].Item1 != c || cycle2[s2].Item2 !=d)
                        {
                            nuova.Add(cycle2[s2]);
                        }

                    }

                    costo_ = CalculateCost(nuova, costMatrix);
                    if(costo_ < cost)
                    {
                        cost = costo_;
                        def = nuova;
                    }
                }
            }


            return def;
        }

        static double CalculateCost(List<Tuple<int, int>> tour, double[][] costMatrix)
        {
            double totalCost = 0;

            for (int i = 0; i < tour.Count; i++)
            {
                var edge = tour[i];
                totalCost += costMatrix[edge.Item1][edge.Item2];
            }


            return totalCost;
        }



        static bool IsUniqueCycle(double[][] soluzione)
        {
            int numRows = soluzione.Length;
            int numCols = soluzione[0].Length;
            bool[] visited = new bool[numRows];

            int startNode = -1;
            for (int i = 0; i < numRows; i++)
            {
                int count = 0;

                for (int j = 0; j < numCols; j++)
                {
                    if (soluzione[i][j] == 1)
                    {
                        count++;
                    }
                }

                if (count > 0)
                {
                    startNode = i;
                    break;
                }
            }

            if (startNode == -1)
            {
                return false;
            }

            VisitNode(soluzione, visited, startNode, startNode);

            foreach (bool visitato in visited)
            {
                if (!visitato)
                {
                    return false;
                }
            }

            return true;
        }

        static void VisitNode(double[][] soluzione, bool[] visited, int currentNode, int startNode)
        {
            visited[currentNode] = true;

            for (int i = 0; i < soluzione[currentNode].Length; i++)
            {
                if (soluzione[currentNode][i] == 1 && !visited[i])
                {
                    VisitNode(soluzione, visited, i, startNode);
                }
            }

            if (currentNode == startNode)
            {
                visited[startNode] = true;
            }
        }

    }
}
