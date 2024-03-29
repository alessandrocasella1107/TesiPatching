using ATSP_Patching;
using ILOG.Concert;
using ILOG.CPLEX;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.AccessControl;

namespace PatchingATSP
{
    public class SolveATSPPatchCplex
    {
        public static double solve(int n, double[][] m)
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
                    {//degree constraints.
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
                    {//degree constraints.
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
                    //eliminazione dei sottocicli
                    for (int i = 1; i < n; i++)
                    {
                        for (int j = 1; j < n; j++)
                        {
                            if (i != j)
                            {
                                ILinearNumExpr exp = cplex.LinearNumExpr();
                                exp.AddTerm(1, u[i]);
                                exp.AddTerm(-1, u[j]);
                                exp.AddTerm(n - 1, x[i][j]);
                                cplex.AddLe(exp, n - 2);
                            }
                        }
                    }

                    cplex.SetParam(Cplex.Param.MIP.Strategy.Search, (int)Cplex.MIPSearch.Traditional); //heuristic


                    if (cplex.Solve())
                    {


                        //Console.WriteLine();
                        //Console.WriteLine("Solution status = " + cplex.GetStatus());
                        //Console.WriteLine();
                        //Console.WriteLine("Optimal value = " + cplex.ObjValue);
                        //Console.WriteLine();

                        //for (int i = 0; i < n; i++)
                        //{
                        //    for (int j = 0; j < n; j++)
                        //    {
                        //        if (x[i][j] != null && cplex.GetValue(x[i][j]) != 0)
                        //        {
                        //            Console.WriteLine($"x[{i}][{j}] = {cplex.GetValue(x[i][j])}");
                        //        }
                        //    }
                        //}

                        return cplex.ObjValue;

                    }
                    return 0;
                }
            }
            catch (ILOG.Concert.Exception exc)
            {
                Console.WriteLine("Concert exception caught: " + exc);
                return 0;
            }
        }
    }
}
