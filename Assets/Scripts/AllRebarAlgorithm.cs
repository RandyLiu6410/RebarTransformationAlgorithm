using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllRebarAlgorithm
{
    public List<Vector3> Pt_asdesigned;
    public List<Vector3> Pt_asbuilt;

    public float additionError = 0f;
    public float min = 100000f; // Max error
    public float length = 10f;

    bool lessthanMin = false;

    public Material mat;

    public AllRebarAlgorithm(List<Vector3> Pt_asdesigned, List<Vector3> Pt_asbuilt)
    {
        this.Pt_asdesigned = Pt_asdesigned;
        this.Pt_asbuilt = Pt_asbuilt;
    }

    public void RunAlgorithm()
    {
        List<Vector3> insidePts1 = this.Pt_asdesigned; // as-designed
        List<Vector3> insidePts2 = this.Pt_asbuilt; // as-built
        Matrix4x4 T = new Matrix4x4(); // transformation matrix

        System.Random random = new System.Random();

        IList<int> indexes = new List<int>();
        // handle inside points
        for (int i = 0; i < Pt_asdesigned.Count; i++)
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
                    float randomDouble = 0f;// (float)(random.NextDouble() * 0.001) + additionError; // 0~3 mm
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
                    Vector3 p = T_temp.MultiplyPoint(insidePts2[index]);
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

        Debug.Log(String.Format("final error: {0}", min));
        Debug.Log(T);

        string asdesigned = "";
        string aftercal = "";
        for (int index = 0; index < insidePts2.Count; index++)
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

        for(int i = 0; i < insidePts1.Count; i += 2)
        {
            DrawLine(insidePts1[i], insidePts1[i + 1], Color.black);
            DrawLine(T.MultiplyPoint(insidePts2[i]), T.MultiplyPoint(insidePts2[i + 1]), Color.yellow);
        }
        Vector3 N4_s = new Vector3(-49.42f, 475.02f, 207f);
        Vector3 N4_e = new Vector3(133.04f, -437.67f, 205f);
        DrawLine(T.MultiplyPoint(N4_s), T.MultiplyPoint(N4_e), Color.red);

        DrawLine(N4_s, N4_e, Color.cyan);

        Vector3 N4o_s = new Vector3(-152.15f, 469.42f, 199f);
        Vector3 N4o_e = new Vector3(20.86f, -458.59f, 199f);
        DrawLine(N4o_s, N4o_e, Color.green);

        float error_N4 = 0;
        Vector3 p_N4 = T.MultiplyPoint(N4_s);
        Vector3 p1_N4 = T.MultiplyPoint(N4_e);
        error_N4 += Pow2(p_N4.x - N4_s.x) + Pow2(p_N4.y - N4_s.y) + Pow2(p_N4.z - N4_s.z);
        error_N4 += Pow2(p1_N4.x - N4_e.x) + Pow2(p1_N4.y - N4_e.y) + Pow2(p1_N4.z - N4_e.z);
        Debug.Log(error_N4 / 2);
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
