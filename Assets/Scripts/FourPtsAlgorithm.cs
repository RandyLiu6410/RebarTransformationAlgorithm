using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FourPtsAlgorithm
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

    public FourPtsAlgorithm(List<Vector3>[] StartPts, List<Vector3>[] EndPts)
    {
        this.StartPts = StartPts;
        this.EndPts = EndPts;
        AlltPts = new List<Vector3>[2];
        AlltPts[0] = new List<Vector3>() { new Vector3(-857.74f, 341.94f, 319f), new Vector3(555.90f, 605.49f, 319f), new Vector3(729.64f, -326.46f, 319f), new Vector3(-507.29f, -557.06f, 319f) };
        AlltPts[1] = new List<Vector3>() { new Vector3(-853.87f, 309.58f, 310f), new Vector3(545.37f, 575.15f, 319f), new Vector3(719.62f, -311.73f, 318f), new Vector3(-667.87f, -573.5f, 305f) };
        
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


                int column = 0;
                foreach (int index in combination)
                {
                    // Generate a random number to reset y and z of every as-built point
                    float randomDouble = 0f;// (float)(random.NextDouble() * 0.001) + additionError; // 0~3 mm
                    matrixP.SetColumn(column, new Vector4(AlltPts[1][index].x + randomDouble, AlltPts[1][index].y + randomDouble, AlltPts[1][index].z + randomDouble, 1));
                    _matrixP.SetColumn(column, new Vector4(AlltPts[0][index].x, AlltPts[0][index].y, AlltPts[0][index].z, 1));

                    column++;
                }

                Debug.Log(matrixP);
                Debug.Log(_matrixP);

                T_temp = _matrixP * matrixP.inverse;

                float errorsum = 0;

                for (int index = 0; index < AlltPts[0].Count; index++)
                {
                    Vector3 p = T_temp.MultiplyPoint(AlltPts[1][index]);
                    errorsum += Pow2(p.x - AlltPts[0][index].x) + Pow2(p.y - AlltPts[0][index].y) + Pow2(p.z - AlltPts[0][index].z);
                }

                float error = errorsum / AlltPts[0].Count;
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
        for (int index = 0; index < StartPts[0].Count; index++)
        {
            asdesigned += String.Format("x: {0}, y: {1}, z: {2}", StartPts[0][index].x, StartPts[0][index].y, StartPts[0][index].z) + "\n";
        }
        for (int index = 0; index < EndPts[0].Count; index++)
        {
            asdesigned += String.Format("x: {0}, y: {1}, z: {2}", EndPts[0][index].x, EndPts[0][index].y, EndPts[0][index].z) + "\n";
        }
        for (int index = 0; index < StartPts[1].Count; index++)
        {
            Vector3 afterCal = T.MultiplyPoint(StartPts[1][index]);
            aftercal += String.Format("x: {0}, y: {1}, z: {2}", afterCal.x, afterCal.y, afterCal.z) + "\n";
        }
        for (int index = 0; index < EndPts[1].Count; index++)
        {
            Vector3 afterCal = T.MultiplyPoint(EndPts[1][index]);
            aftercal += String.Format("x: {0}, y: {1}, z: {2}", afterCal.x, afterCal.y, afterCal.z) + "\n";
        }
        Debug.Log(asdesigned);
        Debug.Log(aftercal);

        float origin_error = 0;
        float modified_error = 0;
        float origin_d_error = 0;
        float modified_d_error = 0;

        for (int i = 0; i < StartPts[0].Count; i++)
        {
            if ((i + 1) == 5 || (i + 1) == 12)
            {
                //DrawLine(StartPts[0][i], EndPts[0][i], Color.green);
                //DrawLine(StartPts[1][i], EndPts[1][i], Color.red);
                //DrawLine(T.MultiplyPoint(StartPts[1][i]), T.MultiplyPoint(EndPts[1][i]), Color.cyan);
                CreateCylinderBetweenPoints(StartPts[0][i], EndPts[0][i], 14f, Color.green);
                //CreateCylinderBetweenPoints(StartPts[1][i], EndPts[1][i], 14f, Color.red);
                CreateCylinderBetweenPoints(T.MultiplyPoint(StartPts[1][i]), T.MultiplyPoint(EndPts[1][i]), 14f, Color.cyan);

                float error_d_b = 0;
                error_d_b += Pow2(StartPts[0][i].x - StartPts[1][i].x) + Pow2(StartPts[0][i].y - StartPts[1][i].y) + Pow2(StartPts[0][i].z - StartPts[1][i].z);
                error_d_b += Pow2(EndPts[0][i].x - EndPts[1][i].x) + Pow2(EndPts[0][i].y - EndPts[1][i].y) + Pow2(EndPts[0][i].z - EndPts[1][i].z);
                Debug.Log("error_d_b");
                Debug.Log(error_d_b / 2);
                origin_d_error += error_d_b / 2;

                float error_d_mb = 0;
                error_d_mb += Pow2(StartPts[0][i].x - T.MultiplyPoint(StartPts[1][i]).x) + Pow2(StartPts[0][i].y - T.MultiplyPoint(StartPts[1][i]).y) + Pow2(StartPts[0][i].z - T.MultiplyPoint(StartPts[1][i]).z);
                error_d_mb += Pow2(EndPts[0][i].x - T.MultiplyPoint(EndPts[1][i]).x) + Pow2(EndPts[0][i].y - T.MultiplyPoint(EndPts[1][i]).y) + Pow2(EndPts[0][i].z - T.MultiplyPoint(EndPts[1][i]).z);
                Debug.Log("error_d_mb");
                Debug.Log(error_d_mb / 2);
                modified_d_error += error_d_mb / 2;
            }
            else
            {
                //DrawLine(StartPts[0][i], EndPts[0][i], Color.black);
                //DrawLine(StartPts[1][i], EndPts[1][i], Color.magenta);
                //DrawLine(T.MultiplyPoint(StartPts[1][i]), T.MultiplyPoint(EndPts[1][i]), Color.yellow);

                CreateCylinderBetweenPoints(StartPts[0][i], EndPts[0][i], 14f, Color.black);
                //CreateCylinderBetweenPoints(StartPts[1][i], EndPts[1][i], 14f, Color.grey);
                CreateCylinderBetweenPoints(T.MultiplyPoint(StartPts[1][i]), T.MultiplyPoint(EndPts[1][i]), 14f, Color.yellow);

                float error_b = 0;
                error_b += Pow2(StartPts[0][i].x - StartPts[1][i].x) + Pow2(StartPts[0][i].y - StartPts[1][i].y) + Pow2(StartPts[0][i].z - StartPts[1][i].z);
                error_b += Pow2(EndPts[0][i].x - EndPts[1][i].x) + Pow2(EndPts[0][i].y - EndPts[1][i].y) + Pow2(EndPts[0][i].z - EndPts[1][i].z);
                origin_error += error_b / 2;

                float error_mb = 0;
                error_mb += Pow2(StartPts[0][i].x - T.MultiplyPoint(StartPts[1][i]).x) + Pow2(StartPts[0][i].y - T.MultiplyPoint(StartPts[1][i]).y) + Pow2(StartPts[0][i].z - T.MultiplyPoint(StartPts[1][i]).z);
                error_mb += Pow2(EndPts[0][i].x - T.MultiplyPoint(EndPts[1][i]).x) + Pow2(EndPts[0][i].y - T.MultiplyPoint(EndPts[1][i]).y) + Pow2(EndPts[0][i].z - T.MultiplyPoint(EndPts[1][i]).z);
                modified_error += error_mb / 2;
            }
        }

        Debug.Log(String.Format("origin_error: {0}", origin_error));
        Debug.Log(String.Format("modified_error: {0}", modified_error));
        Debug.Log(String.Format("origin_d_error: {0}", origin_d_error));
        Debug.Log(String.Format("modified_d_error: {0}", modified_d_error));

        //Vector3 N4_s = new Vector3(-49.42f, 475.02f, 207f);
        //Vector3 N4_e = new Vector3(133.04f, -437.67f, 205f);
        //DrawLine(T.MultiplyPoint(N4_s), T.MultiplyPoint(N4_e), Color.red);

        //DrawLine(N4_s, N4_e, Color.cyan);

        //Vector3 N4o_s = new Vector3(-152.15f, 469.42f, 199f);
        //Vector3 N4o_e = new Vector3(20.86f, -458.59f, 199f);
        //DrawLine(N4o_s, N4o_e, Color.green);

        //float error_N4 = 0;
        //Vector3 p_N4 = T.MultiplyPoint(N4_s);
        //Vector3 p1_N4 = T.MultiplyPoint(N4_e);
        //error_N4 += Pow2(p_N4.x - N4_s.x) + Pow2(p_N4.y - N4_s.y) + Pow2(p_N4.z - N4_s.z);
        //error_N4 += Pow2(p1_N4.x - N4_e.x) + Pow2(p1_N4.y - N4_e.y) + Pow2(p1_N4.z - N4_e.z);
        //Debug.Log(error_N4 / 2);
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
}
