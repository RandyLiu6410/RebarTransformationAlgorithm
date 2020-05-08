using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlgorithmTest : MonoBehaviour
{
    public GameObject cylinder1;
    public GameObject cylinder2;

    public float additionError = 0.009f;
    public float length = 1f;

    System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        while (length < 13f)
        {
            Vector3 cylinder1_sp;
            Vector3 cylinder1_ep;
            Vector3 cylinder2_sp;
            Vector3 cylinder2_ep;

            List<Vector3> insidePts1 = new List<Vector3>(); // as-designed
            List<Vector3> insidePts2 = new List<Vector3>(); // as-built

            Matrix4x4 T = new Matrix4x4(); // transformation matrix

            float min = 0.1f; // Max error
            bool lessthanMin = false;

            Debug.Log(String.Format("Length: {0}m", length.ToString()));

            Vector3 localscale1 = cylinder1.transform.localScale;
            cylinder1.transform.localScale = new Vector3(localscale1.x, length / 2, localscale1.z);
            Vector3 localscale2 = cylinder1.transform.localScale;
            cylinder2.transform.localScale = new Vector3(localscale2.x, length / 2, localscale2.z);

            cylinder1_sp = cylinder1.transform.position - cylinder1.transform.up * cylinder1.transform.localScale.y;
            cylinder1_ep = cylinder1.transform.position + cylinder1.transform.up * cylinder1.transform.localScale.y;

            cylinder2_sp = cylinder2.transform.position - cylinder2.transform.up * cylinder2.transform.localScale.y;
            cylinder2_ep = cylinder2.transform.position + cylinder2.transform.up * cylinder2.transform.localScale.y;

            IList<int> indexes = new List<int>();
            int pointsNum = 10;
            // handle inside points
            for (int i = 0; i < pointsNum+1; i++)
            {
                indexes.Add(i);
                insidePts1.Add(cylinder1_sp + cylinder1.transform.up * cylinder1.transform.localScale.y * 2 * i / pointsNum);
                insidePts2.Add(cylinder2_sp + cylinder2.transform.up * cylinder2.transform.localScale.y * 2 * i / pointsNum);
            }

            // algorithm

            while (!lessthanMin)
            {
                foreach (IEnumerable<int> combination in Combinations(indexes, 4))
                {
                    Matrix4x4 matrixP = new Matrix4x4(); // P as-built
                    Matrix4x4 _matrixP = new Matrix4x4(); // P' as-designed
                    Matrix4x4 T_temp; // transformation matrix temp


                    int column = 0;
                    foreach (int index in combination)
                    {
                        // Generate a random number to reset y and z of every as-built point
                        float randomDouble = (float)(random.NextDouble() * 0.001) + additionError; // 0~3 mm
                        matrixP.SetColumn(column, new Vector4(insidePts2[index].x + randomDouble, insidePts2[index].y + randomDouble, insidePts2[index].z + randomDouble, 1));
                        _matrixP.SetColumn(column, new Vector4(insidePts1[index].x, insidePts1[index].y, insidePts1[index].z, 1));

                        column++;
                    }

                    //Debug.Log(matrixP);
                    //Debug.Log(matrixP.inverse);

                    T_temp = _matrixP * matrixP.inverse;

                    float errorsum = 0;

                    for (int index = 0; index < insidePts2.Count; index++)
                    {
                        Vector3 p = T_temp.MultiplyPoint3x4(insidePts2[index]);
                        errorsum += Pow2(p.x - insidePts1[index].x) + Pow2(p.y - insidePts1[index].y) + Pow2(p.z - insidePts1[index].z);
                    }

                    float error = errorsum / insidePts2.Count;
                    //Debug.Log(String.Format("error: {0}", error));

                    if (error < min)
                    {
                        lessthanMin = true;
                        min = error;
                        T = T_temp;
                    }
                }
            }


            string asdesigned = "";
            string asbuilt = "";
            string aftercal = "";
            for (int index = 0; index < insidePts1.Count; index++)
            {
                asdesigned += String.Format("x: {0}, y: {1}, z: {2}", insidePts1[index].x, insidePts1[index].y, insidePts1[index].z) + "\n";
            }
            for (int index = 0; index < insidePts2.Count; index++)
            {
                asbuilt += String.Format("x: {0}, y: {1}, z: {2}", insidePts2[index].x, insidePts2[index].y, insidePts2[index].z) + "\n";
            }
            for (int index = 0; index < insidePts2.Count; index++)
            {
                Vector3 afterCal = T.MultiplyPoint3x4(insidePts2[index]);
                aftercal += String.Format("x: {0}, y: {1}, z: {2}", afterCal.x, afterCal.y, afterCal.z) + "\n";
            }
            Debug.Log(asdesigned);
            Debug.Log(asbuilt);
            Debug.Log(aftercal);
            Debug.Log(String.Format("final error: {0}", min));
            Debug.Log(T);

            length += 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static bool NextCombination(IList<int> num, int n, int k)
    {
        bool finished;

        var changed = finished = false;

        if (k <= 0) return false;

        for (var i = k - 1; !finished && !changed; i--)
        {
            if (num[i] < n - 1 - (k - 1) + i)
            {
                num[i]++;

                if (i < k - 1)
                    for (var j = i + 1; j < k; j++)
                        num[j] = num[j - 1] + 1;
                changed = true;
            }
            finished = i == 0;
        }

        return changed;
    }

    private static IEnumerable Combinations<T>(IEnumerable<T> elements, int k)
    {
        var elem = elements.ToArray();
        var size = elem.Length;

        if (k > size) yield break;

        var numbers = new int[k];

        for (var i = 0; i < k; i++)
            numbers[i] = i;

        do
        {
            yield return numbers.Select(n => elem[n]);
        } while (NextCombination(numbers, size, k));
    }

    private float Pow2(float x)
    {
        return x * x;
    }
}
