using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlgorithmFormWork : MonoBehaviour
{
    public GameObject formwork1;
    public GameObject formwork2;

    Vector3[] combinations = new Vector3[] { new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, -1),
                                             new Vector3(-1, -1, 1), new Vector3(-1, 1, -1), new Vector3(1, -1, -1), new Vector3(-1, -1, -1) };

    List<Vector3> insidePts1 = new List<Vector3>(); // as-designed
    List<Vector3> insidePts2 = new List<Vector3>(); // as-built

    Matrix4x4 T = new Matrix4x4(); // transformation matrix

    float min = 100; // Max error

    System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        // handle inside points
        IList<int> indexes = new List<int>();
        int i = 0;
        foreach (Vector3 vec in combinations)
        {
            indexes.Add(i);
            i++;

            insidePts1.Add(formwork1.transform.TransformPoint(Vector3.Scale(formwork1.transform.localScale / 2, vec)));
            insidePts2.Add(formwork2.transform.TransformPoint(Vector3.Scale(formwork2.transform.localScale / 2, vec)));
        }

        for (int j = 0; j < insidePts1.Count; j++)
        {
            Debug.Log(insidePts1[j]);
            Debug.Log(insidePts2[j]);
        }

        foreach (IEnumerable<int> combination in Combinations(indexes, 4))
        {
            Matrix4x4 matrixP = new Matrix4x4(); // P as-built
            Matrix4x4 _matrixP = new Matrix4x4(); // P' as-designed
            Matrix4x4 T_temp; // transformation matrix temp


            int column = 0;
            foreach (int index in combination)
            {
                // Generate a random number to reset y and z of every as-built point
                float randomDouble = 0; //= (float)(random.NextDouble() * 0.001) * 3; // 0~3 mm

                matrixP.SetColumn(column, new Vector4(insidePts2[index].x, insidePts2[index].y + randomDouble, insidePts2[index].z + randomDouble, 1));
                _matrixP.SetColumn(column, new Vector4(insidePts1[index].x, insidePts1[index].y, insidePts1[index].z, 1));

                column++;
            }

            Debug.Log(matrixP);
            Debug.Log(matrixP.inverse);

            T_temp = _matrixP * matrixP.inverse;

            float errorsum = 0;

            for (int index = 0; index < insidePts2.Count; index++)
            {
                Vector3 p = T_temp.MultiplyPoint(insidePts2[index]);
                errorsum += Pow2(p.x - insidePts1[index].x) + Pow2(p.y - insidePts1[index].y) + Pow2(p.z - insidePts1[index].z);
            }

            float error = errorsum / insidePts2.Count;
            Debug.Log(String.Format("error: {0}", error));

            if (error < min)
            {
                min = error;
                T = T_temp;
            }
        }

        Debug.Log(String.Format("final error: {0}", min));
        Debug.Log(T);
        for (int index = 0; index < insidePts2.Count; index++)
        {
            Debug.Log(String.Format("after cal P{0}", index));
            Vector3 afterCal = T.MultiplyPoint(insidePts2[index]);
            Debug.Log(String.Format("x: {0}, y: {1}, z: {2}", afterCal.x, afterCal.y, afterCal.z));
            Debug.Log(String.Format("as-designed P{0}", index));
            Debug.Log(String.Format("x: {0}, y: {1}, z: {2}", insidePts1[index].x, insidePts1[index].y, insidePts1[index].z));
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
