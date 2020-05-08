using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RebarAlgorithm
{
    public Vector3 startPt_asdesigned;
    public Vector3 endPt_asdesigned;
    public Vector3 startPt_asbuilt;
    public Vector3 endPt_asbuilt;

    public float additionError = 0.009f;
    public float min = 1000000f; // Max error
    public float length = 10f;

    bool lessthanMin = false;

    public Material mat;

    public RebarAlgorithm(Vector3 startPt_asdesigned, Vector3 endPt_asdesigned, Vector3 startPt_asbuilt, Vector3 endPt_asbuilt)
    {
        this.startPt_asdesigned = startPt_asdesigned;
        this.endPt_asdesigned = endPt_asdesigned;
        this.startPt_asbuilt = startPt_asbuilt;
        this.endPt_asbuilt = endPt_asbuilt;
    }

    public void RunAlgorithm()
    {
        List<Vector3> insidePts1 = new List<Vector3>(); // as-designed
        List<Vector3> insidePts2 = new List<Vector3>(); // as-built
        Matrix4x4 T = new Matrix4x4(); // transformation matrix

        System.Random random = new System.Random();

        IList<int> indexes = new List<int>();
        int pointsNum = 10;
        // handle inside points
        for (int i = 0; i <= pointsNum; i++)
        {
            indexes.Add(i);
            insidePts1.Add(startPt_asdesigned + (endPt_asdesigned - startPt_asdesigned) * i / pointsNum);
            insidePts2.Add(startPt_asbuilt + (endPt_asbuilt - startPt_asbuilt) * i / pointsNum);
        }
        
        while (!lessthanMin)
        {
            foreach (IEnumerable<int> combination in Combinations(indexes, 4))
            {
                Matrix4x4 matrixP = new Matrix4x4(); // P as-built
                Matrix4x4 _matrixP = new Matrix4x4(); // P' as-designed
                Matrix4x4 T_temp; // transformation matrix temp
                Matrix4x4 after_T_temp; // transformation matrix temp


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

                //Matrix4x4 scaleMatrix = new Matrix4x4();
                //float sx = Convert.ToSingle(Math.Sqrt(Convert.ToDouble(Pow2(T_temp.GetColumn(0).x) + Pow2(T_temp.GetColumn(0).y) + Pow2(T_temp.GetColumn(0).z))));
                //float sy = Convert.ToSingle(Math.Sqrt(Convert.ToDouble(Pow2(T_temp.GetColumn(1).x) + Pow2(T_temp.GetColumn(1).y) + Pow2(T_temp.GetColumn(1).z))));
                //float sz = Convert.ToSingle(Math.Sqrt(Convert.ToDouble(Pow2(T_temp.GetColumn(2).x) + Pow2(T_temp.GetColumn(2).y) + Pow2(T_temp.GetColumn(2).z))));
                //scaleMatrix.SetColumn(0, new Vector4(sx, 0, 0, 0));
                //scaleMatrix.SetColumn(1, new Vector4(0, sy, 0, 0));
                //scaleMatrix.SetColumn(2, new Vector4(0, 0, sz, 0));
                //scaleMatrix.SetColumn(3, new Vector4(0, 0, 0, 1));
                //after_T_temp = T_temp * scaleMatrix.inverse;

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

        DrawLine(insidePts1[0], insidePts1[insidePts1.Count - 1], Color.black);
        DrawLine(T.MultiplyPoint(insidePts2[0]), T.MultiplyPoint(insidePts2[insidePts2.Count - 1]), Color.yellow);
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
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.SetWidth(1f, 1f);
    }
}
