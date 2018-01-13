using UnityEngine;
using strange.extensions.command.impl;

namespace Approximator
{
    public class CalculateOLSApproximation : Command
    {
        [Inject(name = OLS.Degree)]
        public int K { get; private set; }

        [Inject]
        public MainModel Model { get; private set; }

        int pts;

        float[] a;
        float[] b;
        float[] x;
        float[] y;
        float[][] sums;
 
        void Calculate()
        {
            int i, j, k;
            float s, t, M;
            //упорядочиваем узловые точки по возрастанию абсцисс
            for (i = 0; i < pts; i++)
            {
                for (int ind = i; ind >= 1; ind--)
                    if (x[ind] < x[ind - 1])
                    {
                        t = x[ind - 1]; x[ind - 1] = x[ind]; x[ind] = t;
                        t = y[ind - 1]; y[ind - 1] = y[ind]; y[ind] = t;
                    }
            }
            //заполняем коэффициенты системы уравнений
            for (i = 0; i < K + 1; i++)
            {
                for (j = 0; j < K + 1; j++)
                {
                    sums[i][j] = 0;
                    for (k = 0; k < pts; k++)
                        sums[i][j] += Mathf.Pow(x[k], i + j);
                }
            }
            //заполняем столбец свободных членов
            for (i = 0; i < K + 1; i++)
            {
                b[i] = 0;
                for (k = 0; k < pts; k++)
                    b[i] += Mathf.Pow(x[k], i) * y[k];
            }
            //применяем метод Гаусса для приведения матрицы системы к треугольному виду
            for (k = 0; k < K + 1; k++)
            {
                for (i = k + 1; i < K + 1; i++)
                {
                    M = sums[i][k] / sums[k][k];// == 0 ? 1 : sums[k][k]);
                    for (j = k; j < K + 1; j++)
                        sums[i][j] -= M * sums[k][j];
                    b[i] -= M * b[k];
                    //Debug.Log("b[i]: " + b[i] + ", b[k]: " + b[k]);
                }
            }
            //вычисляем коэффициенты аппроксимирующего полинома
            for (i = K; i >= 0; i--)
            {
                s = 0;
                for (j = i; j < K + 1; j++)
                    s += sums[i][j] * a[j];
                a[i] = (b[i] - s) / sums[i][i];
                //Debug.Log(sums[i][i]);
            }
        }
        void Refresh()
        {
            for (int i = 0; i < K; i++)
            {
                a[i] = b[i] = 0;
                for (int j = 0; j < K; j++)
                    sums[i][j] = 0;
            }
        }
        void Init()
        {
            pts = Model.Table.Count;
            a = new float[pts];
            b = new float[pts];
            x = new float[pts];
            y = new float[pts];
            int ind = 0;
            foreach(int key in Model.Table.Keys)
            {
                x[ind] = key;
                y[ind] = Model.Table[key];
                ++ind;
            }
            sums = new float[pts][];
            for (int i = 0; i < pts; ++i)
                sums[i] = new float[pts];
            
        }
        public override void Execute()
        {
            Init();
            if (pts <= K)
                return;
            Refresh();
            Calculate();
            Model.OLS.Value = new MainModel.Calculator(
            (x) =>
                {
                    float y = 0f;
                    for (int i = 0; i <= K; ++i)
                        y += a[i] * Mathf.Pow(x, i);
                    return y;
                });
        }
    }

    public class CalculateLagrangeApproximation : Command
    {
        [Inject]
        public MainModel Model { get; private set; }
        [Inject]
        public ScheduleConfig Config { get; private set; }

        float[] x;
        float[] y;
        float[] r;

        void Init()
        {
            int len = Model.Table.Keys.Count;
            int i = 0;
            x = new float[len];
            y = new float[len];
            foreach(int key in Model.Table.Keys)
            {
                x[i] = key;
                y[i] = Model.Table[key];
                ++i;
            }
        }
        void Calculate()
        {
            int n, k, m, i, j;
            float ct, u;
            float[] pt;
            n = x.Length;
            r = new float[n];
            pt = new float[n];
            for (i = 0; i < n; ++i)
                r[i] = 0;
            for (i = 0; i < n; ++i)
            {
                ct = 1; pt[0] = 1; k = 0;
                for (j = 0; j < n; ++j)
                    if (i != j)
                    {
                        ct *= (x[i] - x[j]);
                        pt[k + 1] = 1;
                        if (k > 0)
                            for (m = k; m >= 1; --m)
                                pt[m] = pt[m - 1] - pt[m] * x[j];
                        pt[0] = -pt[0] * x[j];
                        ++k;
                    }
                u = y[i] / ct;
                for (m = 0; m < n; ++m)
                    r[m] += pt[m] * u;
            }
        }
        public override void Execute()
        {
            Init();
            Calculate();
            Model.Lagrange.Value = new MainModel.Calculator((param) =>
            {
                float y = 0f;
                for (int i = 0; i < x.Length; ++i)
                    y += r[i] * Mathf.Pow(param, i);
                return y;
            });
        }
    }
}
