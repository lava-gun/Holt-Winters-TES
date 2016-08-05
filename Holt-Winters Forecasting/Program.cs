using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holt_Winters_Forecasting
{
    class Program
    {
        static float[] DoubleExponentialSmoothing(float[] arr, float alpha, float beta, out float[] result)
        {
            float value, level=0, trend=0, lastLevel;
            int n = arr.Length + 1;
            result = new float[n];
            result[0] = arr[0];
            for (int i = 1; i < n; i++)
            {
                Console.WriteLine(i);
                if (i == 1)
                {
                    level = arr[0];
                    trend = arr[1] - arr[0];
                }
                if (i >= arr.Length)
                {
                    value = result[i-1];
                }
                else value = arr[i];

                lastLevel = level;
                level = alpha * value + (1 - alpha) * (level + trend);
                trend = beta * (level - lastLevel) + (1 - beta) * trend;
                result[i] = level + trend;
            }
            return result;
        }
        static float Initialtrend(float[] arr, int slen)
        {
            float sum = 0;
            for (int i = 0; i < slen; i++)
            {
                sum += (arr[i + slen] - arr[i]) / slen;
            }
            return sum / slen;
        }
        static float[] InitialSeasonalComponents(float[] arr, int slen, out float[] seasonals)
        {

            int nseasons = arr.Length / slen;
            Console.WriteLine(nseasons);
            seasonals = new float[slen];
            float[] seasonAverages = new float[nseasons];
            for (int j = 0; j < nseasons; j++)
            {
                float sum = 0.0F;
                for (int i = j * slen; i < j * slen + slen; i++)
                {
                    sum += arr[i];
                }
                seasonAverages[j] = sum / slen;
            }
            for (int i = 0; i < slen; i++)
            {
                float sum = 0.0F;
                for (int j = 0; j < nseasons; j++)
                {
                    sum += arr[slen * j + i] - seasonAverages[j];
                }
                Console.WriteLine(i);
                seasonals[i] = sum / nseasons;
            }
            return seasonals;
        }
        static float[] TripleExponentialSmoothing (float[] arr, int slen, float alpha, float beta, float gamma, int nPreds, out float[] result)
        {
            result = new float[arr.Length + nPreds];
            float[] seasonals = new float[slen];
            int m = 0;
            float smooth = 0.0F, trend =0.0F, val =0.0F, lastSmooth = 0.0F;
            seasonals = InitialSeasonalComponents(arr, slen, out seasonals);
            for (int i = 0; i < (arr.Length + nPreds); i++)
            {
                if (i == 0)
                {
                    smooth = arr[0];
                    trend = Initialtrend(arr, slen);
                    result[0] = arr[0];
                    continue;
                }
                if (i >= arr.Length)
                {
                    m = i - arr.Length + 1;
                    result[i] = (smooth + m * trend) + seasonals[i % slen];
                }
                else
                {
                    val = arr[i];
                    lastSmooth = smooth;
                    smooth = alpha * (val - seasonals[i % slen]) + (1 - alpha) * (smooth + trend);
                    trend = beta * (smooth - lastSmooth) + (1 - beta) * trend;
                    seasonals[i % slen] = gamma * (val - smooth) + (1 - gamma) * seasonals[i % slen];
                    result[i] = (smooth + trend + seasonals[i % slen]);
                }
            }
            return result;
        }
        static void Main(string[] args)
        {
            int sLength = 12, predLength = 24;
            float[] series = new float[7] { 3,10,12,13,12,10,12};
            float[] tripleSeries = new float[] {30,21,29,31,40,48,53,47,37,39,31,29,17,9,20,24,27,35,41,38,
          27,31,27,26,21,13,21,18,33,35,40,36,22,24,21,20,17,14,17,19,
          26,29,40,31,20,24,18,26,17,9,17,21,28,32,46,33,23,28,22,27,
          18,8,17,21,31,34,44,38,31,30,26,32};
            float[] result = new float[8];
            float alpha = 0.9F, beta = 0.9F;
            //Holt winters double exponential smoothing
            //smooths according to trend and level
            result = DoubleExponentialSmoothing(series, alpha, beta, out result);
            for (int i = 0; i < result.Length; i++)
            {
                Console.Write(" " + result[i] + " ");
            }
            float[] result2 = new float[predLength+tripleSeries.Length];
            //Holt winters triple exponential smoothing
            //now accounts for seasonal trends
            float[] seasonals = new float[sLength];
            result2 = TripleExponentialSmoothing(tripleSeries, sLength, 0.716F, 0.029F, 0.993F, predLength, out result2);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            for (int i = 0; i < result2.Length; i++)
            {
                Console.Write(" " + result2[i] + " ");
            }
            Console.ReadLine();
        }
    }
}
