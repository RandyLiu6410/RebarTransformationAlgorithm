using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class T3Algorithm 
{
    public List<Vector3>[] AlltPts;
    public List<Vector3>[] StartPts;
    public List<Vector3>[] EndPts;

    public float additionError = 0f;
    public float min = 100000f; // Max error
    public float length = 10f;

    bool lessthanMin = false;

    public Material mat;

    public GameObject cylinderPrefab; //assumed to be 1m x 1m x 2m default unity cylinder to make calculations easy

    public T3Algorithm(List<Vector3>[] AlltPts, List<Vector3>[] StartPts, List<Vector3>[] EndPts)
    {
        this.AlltPts = AlltPts;
        this.StartPts = StartPts;
        this.EndPts = EndPts;
    }

    public void RunAlgorithm()
    {
        Matrix4x4 T = new Matrix4x4(); // transformation matrix

        System.Random random = new System.Random();

        IList<int> indexes = new List<int>();
        // handle inside points
        for (int i = 0; i < AlltPts[0].Count; i++)
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
                Matrix4x4 after_T_temp; // transformation matrix temp

                int column = 0;
                foreach (int index in combination)
                {
                    // Generate a random number to reset y and z of every as-built point
                    float randomDouble = 0f;// (float)(random.NextDouble() * 0.001) + additionError; // 0~3 mm
                    matrixP.SetColumn(column, new Vector4(AlltPts[1][index].x + randomDouble, AlltPts[1][index].y + randomDouble, AlltPts[1][index].z + randomDouble, 1));
                    _matrixP.SetColumn(column, new Vector4(AlltPts[0][index].x, AlltPts[0][index].y, AlltPts[0][index].z, 1));

                    column++;
                }

                T_temp = _matrixP * matrixP.inverse;


                // Get the scale matrix
                Matrix4x4 scaleMatrix = new Matrix4x4();
                float sx = Convert.ToSingle(Math.Sqrt(Convert.ToDouble(Pow2(T_temp.GetColumn(0).x) + Pow2(T_temp.GetColumn(0).y) + Pow2(T_temp.GetColumn(0).z))));
                float sy = Convert.ToSingle(Math.Sqrt(Convert.ToDouble(Pow2(T_temp.GetColumn(1).x) + Pow2(T_temp.GetColumn(1).y) + Pow2(T_temp.GetColumn(1).z))));
                float sz = Convert.ToSingle(Math.Sqrt(Convert.ToDouble(Pow2(T_temp.GetColumn(2).x) + Pow2(T_temp.GetColumn(2).y) + Pow2(T_temp.GetColumn(2).z))));
                scaleMatrix.SetColumn(0, new Vector4(sx, 0, 0, 0));
                scaleMatrix.SetColumn(1, new Vector4(0, sy, 0, 0));
                scaleMatrix.SetColumn(2, new Vector4(0, 0, sz, 0));
                scaleMatrix.SetColumn(3, new Vector4(0, 0, 0, 1));
                after_T_temp = T_temp * scaleMatrix.inverse;


                float errorsum = 0;

                for (int index = 0; index < AlltPts[0].Count; index++)
                {
                    Vector3 p = after_T_temp.MultiplyPoint3x4(AlltPts[1][index]);

                    errorsum += Pow2(p.x - AlltPts[0][index].x) + Pow2(p.y - AlltPts[0][index].y) + Pow2(p.z - AlltPts[0][index].z);
                }

                float error = errorsum / AlltPts[0].Count;

                if (error < min)
                {
                    lessthanMin = true;
                    min = error;
                    T = after_T_temp;
                }
            }
        }

        Debug.Log(String.Format("final error: {0}", min));
        Debug.Log(T);

        string asdesigned = "";
        string aftercal = "";
        for (int index = 0; index < StartPts[0].Count; index++)
        {
            asdesigned += String.Format("x: {0}, y: {1}, z: {2}", StartPts[0][index].x, StartPts[0][index].y, StartPts[0][index].z) + "\n";
        }
        for (int index = 0; index < EndPts[0].Count; index++)
        {
            asdesigned += String.Format("x: {0}, y: {1}, z: {2}", EndPts[0][index].x, EndPts[0][index].y, EndPts[0][index].z) + "\n";
        }

        string dis_o = "";
        string dis_a = "";
        for (int i = 0; i < StartPts[1].Count; i++)
        {
            dis_o += Vector3.Distance(StartPts[1][i], EndPts[1][i]) + "\n";
        }
        Debug.Log(dis_o);
        for (int index = 0; index < StartPts[1].Count; index++)
        {
            Vector3 afterCal = T.MultiplyPoint3x4(StartPts[1][index]);
            aftercal += String.Format("x: {0}, y: {1}, z: {2}", afterCal.x, afterCal.y, afterCal.z) + "\n";

            dis_a += Vector3.Distance(afterCal, T.MultiplyPoint3x4(EndPts[1][index])) + "\n";
        }
        Debug.Log(dis_a);
        for (int index = 0; index < EndPts[1].Count; index++)
        {
            Vector3 afterCal = T.MultiplyPoint3x4(EndPts[1][index]);
            aftercal += String.Format("x: {0}, y: {1}, z: {2}", afterCal.x, afterCal.y, afterCal.z) + "\n";
        }
        Debug.Log(asdesigned);
        Debug.Log(aftercal);

        float origin_error = 0;
        float modified_error = 0;
        float origin_d_error = 0;
        float modified_d_error = 0;

        for (int i = 0; i < StartPts[0].Count; i ++)
        {
            if((i + 1) == 5 || (i + 1) == 12)
            {
                CreateCylinderBetweenPoints(StartPts[0][i], EndPts[0][i], 14f, Color.green);
                CreateCylinderBetweenPoints(T.MultiplyPoint3x4(StartPts[1][i]), T.MultiplyPoint3x4(EndPts[1][i]), 14f, Color.cyan);

                float error_d_b = 0;
                error_d_b += Pow2(StartPts[0][i].x - StartPts[1][i].x) + Pow2(StartPts[0][i].y - StartPts[1][i].y) + Pow2(StartPts[0][i].z - StartPts[1][i].z);
                error_d_b += Pow2(EndPts[0][i].x - EndPts[1][i].x) + Pow2(EndPts[0][i].y - EndPts[1][i].y) + Pow2(EndPts[0][i].z - EndPts[1][i].z);
                Debug.Log("error_d_b");
                Debug.Log(error_d_b / 2);
                origin_d_error += error_d_b / 2;

                float error_d_mb = 0;
                error_d_mb += Pow2(StartPts[0][i].x - T.MultiplyPoint3x4(StartPts[1][i]).x) + Pow2(StartPts[0][i].y - T.MultiplyPoint3x4(StartPts[1][i]).y) + Pow2(StartPts[0][i].z - T.MultiplyPoint3x4(StartPts[1][i]).z);
                error_d_mb += Pow2(EndPts[0][i].x - T.MultiplyPoint3x4(EndPts[1][i]).x) + Pow2(EndPts[0][i].y - T.MultiplyPoint3x4(EndPts[1][i]).y) + Pow2(EndPts[0][i].z - T.MultiplyPoint3x4(EndPts[1][i]).z);
                Debug.Log("error_d_mb");
                Debug.Log(error_d_mb / 2);
                modified_d_error += error_d_mb / 2;

                RebarAlgorithm ra = new RebarAlgorithm(StartPts[0][i], EndPts[0][i], T.MultiplyPoint3x4(StartPts[1][i]), T.MultiplyPoint3x4(EndPts[1][i]));
                ra.RunAlgorithm();
            }
            else
            {
                CreateCylinderBetweenPoints(StartPts[0][i], EndPts[0][i], 14f, Color.black);
                CreateCylinderBetweenPoints(T.MultiplyPoint3x4(StartPts[1][i]), T.MultiplyPoint3x4(EndPts[1][i]), 14f, Color.yellow);

                float error_b = 0;
                error_b += Pow2(StartPts[0][i].x - StartPts[1][i].x) + Pow2(StartPts[0][i].y - StartPts[1][i].y) + Pow2(StartPts[0][i].z - StartPts[1][i].z);
                error_b += Pow2(EndPts[0][i].x - EndPts[1][i].x) + Pow2(EndPts[0][i].y - EndPts[1][i].y) + Pow2(EndPts[0][i].z - EndPts[1][i].z);
                origin_error += error_b / 2;

                float error_mb = 0;
                error_mb += Pow2(StartPts[0][i].x - T.MultiplyPoint3x4(StartPts[1][i]).x) + Pow2(StartPts[0][i].y - T.MultiplyPoint3x4(StartPts[1][i]).y) + Pow2(StartPts[0][i].z - T.MultiplyPoint3x4(StartPts[1][i]).z);
                error_mb += Pow2(EndPts[0][i].x - T.MultiplyPoint3x4(EndPts[1][i]).x) + Pow2(EndPts[0][i].y - T.MultiplyPoint3x4(EndPts[1][i]).y) + Pow2(EndPts[0][i].z - T.MultiplyPoint3x4(EndPts[1][i]).z);
                modified_error += error_mb / 2;
            }
        }

        Debug.Log(String.Format("origin_error: {0}", origin_error));
        Debug.Log(String.Format("modified_error: {0}", modified_error));
        Debug.Log(String.Format("origin_d_error: {0}", origin_d_error));
        Debug.Log(String.Format("modified_d_error: {0}", modified_d_error));
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

    private void CreateCylinderBetweenPoints(Vector3 start, Vector3 end, float width, Color color)
    {
        var offset = end - start;
        var scale = new Vector3(width, offset.magnitude / 2.0f, width);
        var position = start + (offset / 2.0f);

        var cylinder = MonoBehaviour.Instantiate(cylinderPrefab, position, Quaternion.identity);
        cylinder.transform.up = offset;
        cylinder.transform.localScale = scale;
        var cylinderRenderer = cylinder.GetComponent<Renderer>();
        cylinderRenderer.material.SetColor("_Color", color);
    }

    private Vector3 MultiplyT(Matrix4x4 T, Vector3 vector)
    {
        Vector4 vector4 = new Vector4(vector.x, vector.y, vector.z, 1);
        Vector4 _vector4 = T * vector4;
        Debug.Log(_vector4);
        return new Vector3(_vector4.x, _vector4.y, _vector4.z);
    }
}
