using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExcelManagerT3 : MonoBehaviour
{
    public string CSVName_Design;
    public string CSVName_Built;
    public Material mat;
    public GameObject cylinderPrefab; //assumed to be 1m x 1m x 2m default unity cylinder to make calculations easy

    private List<string[][]> dataArrays = new List<string[][]>();

    private List<Vector3>[] AlltPts = new List<Vector3>[2] { new List<Vector3>(), new List<Vector3>() };
    private List<Vector3>[] StartPts = new List<Vector3>[2] { new List<Vector3>(), new List<Vector3>() };
    private List<Vector3>[] EndPts = new List<Vector3>[2] { new List<Vector3>(), new List<Vector3>() };

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 22; i++)
        {
            AlltPts[0].Add(new Vector3());
            AlltPts[1].Add(new Vector3());
        }

        for (int i = 0; i < 15; i++)
        {
            StartPts[0].Add(new Vector3());
            StartPts[1].Add(new Vector3());
            EndPts[0].Add(new Vector3());
            EndPts[1].Add(new Vector3());
        }

        TextAsset binAsset1 = Resources.Load(CSVName_Design, typeof(TextAsset)) as TextAsset;
        //讀取每一行的內容
        string[] lineArray1 = binAsset1.text.Split("\r"[0]);
        //建立二維陣列
        string[][] text1 = new string[lineArray1.Length][];
        //把csv中的資料儲存在二位陣列中
        for (int i = 0; i < lineArray1.Length; i++)
        {
            text1[i] = lineArray1[i].Split(',');
        }
        dataArrays.Add(text1);
        TextAsset binAsset2 = Resources.Load(CSVName_Built, typeof(TextAsset)) as TextAsset;
        //讀取每一行的內容
        string[] lineArray2 = binAsset2.text.Split("\r"[0]);
        //建立二維陣列
        string[][] text2 = new string[lineArray2.Length][];
        //把csv中的資料儲存在二位陣列中
        for (int i = 0; i < lineArray2.Length; i++)
        {
            text2[i] = lineArray2[i].Split(',');
        }
        dataArrays.Add(text2);

        for (int indexOfArray = 0; indexOfArray < dataArrays.Count; indexOfArray++)
        {
            string nowReading = "";
            foreach (string[] row in dataArrays[indexOfArray])
            {
                int index = 0;
                int indexD = -1;
                if (row[0].Length > 0)
                    indexD = row[0].Last() == 'X' ? 0 : (row[0].Last() == 'Y' ? 1 : (row[0].Last() == 'Z' ? 2 : -1));

                foreach (string item in row)
                {
                    if (item == "All point") { nowReading = "All point"; }
                    else if (item == "Transversal Start point") { nowReading = "Transversal Start point"; }
                    else if (item == "Transversal End Point") { nowReading = "Transversal End Point"; }

                    float num;
                    if (float.TryParse(item, out num))
                    {
                        if (item != "" || indexD != -1)
                        {
                            switch (nowReading)
                            {
                                case "All point":
                                    switch (indexD)
                                    {
                                        case 0:
                                            AlltPts[indexOfArray][index - 1] = new Vector3(num, AlltPts[indexOfArray][index - 1].y, AlltPts[indexOfArray][index - 1].z);
                                            break;
                                        case 1:
                                            AlltPts[indexOfArray][index - 1] = new Vector3(AlltPts[indexOfArray][index - 1].x, num, AlltPts[indexOfArray][index - 1].z);
                                            break;
                                        case 2:
                                            AlltPts[indexOfArray][index - 1] = new Vector3(AlltPts[indexOfArray][index - 1].x, AlltPts[indexOfArray][index - 1].y, num);
                                            break;
                                    }
                                    break;
                                case "Transversal Start point":
                                    switch (indexD)
                                    {
                                        case 0:
                                            StartPts[indexOfArray][index - 1] = new Vector3(num, StartPts[indexOfArray][index - 1].y, StartPts[indexOfArray][index - 1].z);
                                            break;
                                        case 1:
                                            StartPts[indexOfArray][index - 1] = new Vector3(StartPts[indexOfArray][index - 1].x, num, StartPts[indexOfArray][index - 1].z);
                                            break;
                                        case 2:
                                            StartPts[indexOfArray][index - 1] = new Vector3(StartPts[indexOfArray][index - 1].x, StartPts[indexOfArray][index - 1].y, num);
                                            break;
                                    }
                                    break;
                                case "Transversal End Point":
                                    switch (indexD)
                                    {
                                        case 0:
                                            EndPts[indexOfArray][index - 1] = new Vector3(num, EndPts[indexOfArray][index - 1].y, EndPts[indexOfArray][index - 1].z);
                                            break;
                                        case 1:
                                            EndPts[indexOfArray][index - 1] = new Vector3(EndPts[indexOfArray][index - 1].x, num, EndPts[indexOfArray][index - 1].z);
                                            break;
                                        case 2:
                                            EndPts[indexOfArray][index - 1] = new Vector3(EndPts[indexOfArray][index - 1].x, EndPts[indexOfArray][index - 1].y, num);
                                            break;
                                    }
                                    break;
                            }
                        }
                    }

                    index++;
                }
            }
        }

        string allpts_d = "";
        foreach(Vector3 pt in AlltPts[0])
        {
            allpts_d += String.Format("x: {0}, y: {1}, z: {2}", pt.x, pt.y, pt.z) + "\n";
        }
        Debug.Log(allpts_d);
        string allpts_b = "";
        foreach (Vector3 pt in AlltPts[1])
        {
            allpts_b += String.Format("x: {0}, y: {1}, z: {2}", pt.x, pt.y, pt.z) + "\n";
        }
        Debug.Log(allpts_b);

        string start_d = "";
        foreach (Vector3 pt in StartPts[0])
        {
            start_d += String.Format("x: {0}, y: {1}, z: {2}", pt.x, pt.y, pt.z) + "\n";
        }
        Debug.Log(start_d);
        string start_b = "";
        foreach (Vector3 pt in StartPts[1])
        {
            start_b += String.Format("x: {0}, y: {1}, z: {2}", pt.x, pt.y, pt.z) + "\n";
        }
        Debug.Log(start_b);

        string end_d = "";
        foreach (Vector3 pt in EndPts[0])
        {
            end_d += String.Format("x: {0}, y: {1}, z: {2}", pt.x, pt.y, pt.z) + "\n";
        }
        Debug.Log(end_d);
        string end_b = "";
        foreach (Vector3 pt in EndPts[1])
        {
            end_b += String.Format("x: {0}, y: {1}, z: {2}", pt.x, pt.y, pt.z) + "\n";
        }
        Debug.Log(end_b);

        T3Algorithm t3a = new T3Algorithm(AlltPts, StartPts, EndPts);
        t3a.mat = mat;
        t3a.cylinderPrefab = cylinderPrefab;
        t3a.RunAlgorithm();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
