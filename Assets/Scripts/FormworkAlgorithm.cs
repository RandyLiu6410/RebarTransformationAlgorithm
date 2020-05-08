using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormworkAlgorithm
{
    List<Vector3> insidePts1 = new List<Vector3>(); // as-designed
    List<Vector3> insidePts2 = new List<Vector3>(); // as-built

    public float additionError = 0.009f;
    float min = 50f; // Max error
    System.Random random = new System.Random();

    bool lessthanMin = false;

    public Material mat;

    public FormworkAlgorithm(List<Vector3> insidePts1, List<Vector3> insidePts2)
    {
        this.insidePts1 = insidePts1;
        this.insidePts2 = insidePts2;
    }

    public void RunAlgorithm()
    {
        Matrix4x4 T = new Matrix4x4(); // transformation matrix
        IList<int> indexes = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            indexes.Add(i);
        }

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

                T_temp = _matrixP * matrixP.inverse;

                float errorsum = 0;

                for (int index = 0; index < insidePts2.Count; index++)
                {
                    Vector3 p = T_temp.MultiplyPoint(insidePts2[index]);
                    errorsum += Pow2(p.x - insidePts1[index].x) + Pow2(p.y - insidePts1[index].y) + Pow2(p.z - insidePts1[index].z);
                }

                float error = errorsum / insidePts2.Count;

                if (error < min)
                {
                    lessthanMin = true;
                    min = error;
                    T = T_temp;
                }
            }
        }
        Debug.Log(String.Format("final error: {0}", min));
        Debug.Log(T);

        string asdesigned = "";
        string aftercal = "";
        for (int index = 0; index < insidePts1.Count; index++)
        {
            asdesigned += String.Format("x: {0}, y: {1}, z: {2}", insidePts1[index].x, insidePts1[index].y, insidePts1[index].z) + "\n";
        }
        for (int index = 0; index < insidePts2.Count; index++)
        {
            Vector3 afterCal = T.MultiplyPoint(insidePts2[index]);
            aftercal += String.Format("x: {0}, y: {1}, z: {2}", afterCal.x, afterCal.y, afterCal.z) + "\n";
        }
        Debug.Log(asdesigned);
        Debug.Log(aftercal);

        for (int i = 0; i < 4; i ++)
        {
            if (i != 3)
            {
                DrawLine(insidePts1[i], insidePts1[i + 1], Color.black);
                DrawLine(T.MultiplyPoint(insidePts2[i]), T.MultiplyPoint(insidePts2[i + 1]), Color.yellow);
            }
            else
            {
                DrawLine(insidePts1[3], insidePts1[0], Color.black);
                DrawLine(T.MultiplyPoint(insidePts2[3]), T.MultiplyPoint(insidePts2[0]), Color.yellow);
            }
        }
        for (int i = 4; i < 8; i++)
        {
            if (i != 7)
            {
                DrawLine(insidePts1[i], insidePts1[i + 1], Color.black);
                DrawLine(T.MultiplyPoint(insidePts2[i]), T.MultiplyPoint(insidePts2[i + 1]), Color.yellow);
            }
            else
            {
                DrawLine(insidePts1[7], insidePts1[4], Color.black);
                DrawLine(T.MultiplyPoint(insidePts2[7]), T.MultiplyPoint(insidePts2[4]), Color.yellow);
            }
        }
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

    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = mat;
        lr.material.color = color;
        lr.SetColors(color, color);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.SetWidth(1f, 1f);
    }
}
