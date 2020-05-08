using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ExcelManager : MonoBehaviour
{
    public string CSVName_Design;
    public string CSVName_Built;
    public Material mat;

    private List<string[][]> dataArrays = new List<string[][]>();

    private List<Vector3>[] Transversalsp = new List<Vector3>[2] { new List<Vector3>(), new List<Vector3>() };
    private List<Vector3>[] Transversalep = new List<Vector3>[2] { new List<Vector3>(), new List<Vector3>() };
    private List<Vector3>[] Longitudinalsp = new List<Vector3>[2] { new List<Vector3>(), new List<Vector3>() };
    private List<Vector3>[] Longitudinalep = new List<Vector3>[2] { new List<Vector3>(), new List<Vector3>() };

    void Start()
    {
        for (int i = 0; i < 9; i++)
        {
            Transversalsp[0].Add(new Vector3());
            Transversalsp[1].Add(new Vector3());
            Transversalep[0].Add(new Vector3());
            Transversalep[1].Add(new Vector3());
        }

        for (int i = 0; i < 6; i++)
        {
            Longitudinalsp[0].Add(new Vector3());
            Longitudinalsp[1].Add(new Vector3());
            Longitudinalep[0].Add(new Vector3());
            Longitudinalep[1].Add(new Vector3());
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

        for(int indexOfArray = 0; indexOfArray < dataArrays.Count; indexOfArray++)
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
                    if (item == "Transversal Start point") { nowReading = "Transversal Start point"; }
                    else if (item == "Transversal End Point") { nowReading = "Transversal End Point"; }
                    else if (item == "Longitudinal Start Point") { nowReading = "Longitudinal Start Point"; }
                    else if (item == "Longitudinal End Point") { nowReading = "Longitudinal End Point"; }

                    float num;
                    if (float.TryParse(item, out num))
                    {
                        if (item != "" || indexD != -1)
                        {
                            switch (nowReading)
                            {
                                case "Transversal Start point":
                                    switch (indexD)
                                    {
                                        case 0:
                                            Transversalsp[indexOfArray][index - 1] = new Vector3(num, Transversalsp[indexOfArray][index - 1].y, Transversalsp[indexOfArray][index - 1].z);
                                            break;
                                        case 1:
                                            Transversalsp[indexOfArray][index - 1] = new Vector3(Transversalsp[indexOfArray][index - 1].x, num, Transversalsp[indexOfArray][index - 1].z);
                                            break;
                                        case 2:
                                            Transversalsp[indexOfArray][index - 1] = new Vector3(Transversalsp[indexOfArray][index - 1].x, Transversalsp[indexOfArray][index - 1].y, num);
                                            break;
                                    }
                                    break;
                                case "Transversal End Point":
                                    switch (indexD)
                                    {
                                        case 0:
                                            Transversalep[indexOfArray][index - 1] = new Vector3(num, Transversalep[indexOfArray][index - 1].y, Transversalep[indexOfArray][index - 1].z);
                                            break;
                                        case 1:
                                            Transversalep[indexOfArray][index - 1] = new Vector3(Transversalep[indexOfArray][index - 1].x, num, Transversalep[indexOfArray][index - 1].z);
                                            break;
                                        case 2:
                                            Transversalep[indexOfArray][index - 1] = new Vector3(Transversalep[indexOfArray][index - 1].x, Transversalep[indexOfArray][index - 1].y, num);
                                            break;
                                    }
                                    break;
                                case "Longitudinal Start Point":
                                    switch (indexD)
                                    {
                                        case 0:
                                            Longitudinalsp[indexOfArray][index - 1] = new Vector3(num, Longitudinalsp[indexOfArray][index - 1].y, Longitudinalsp[indexOfArray][index - 1].z);
                                            break;
                                        case 1:
                                            Longitudinalsp[indexOfArray][index - 1] = new Vector3(Longitudinalsp[indexOfArray][index - 1].x, num, Longitudinalsp[indexOfArray][index - 1].z);
                                            break;
                                        case 2:
                                            Longitudinalsp[indexOfArray][index - 1] = new Vector3(Longitudinalsp[indexOfArray][index - 1].x, Longitudinalsp[indexOfArray][index - 1].y, num);
                                            break;
                                    }
                                    break;
                                case "Longitudinal End Point":
                                    switch (indexD)
                                    {
                                        case 0:
                                            Longitudinalep[indexOfArray][index - 1] = new Vector3(num, Longitudinalep[indexOfArray][index - 1].y, Longitudinalep[indexOfArray][index - 1].z);
                                            break;
                                        case 1:
                                            Longitudinalep[indexOfArray][index - 1] = new Vector3(Longitudinalep[indexOfArray][index - 1].x, num, Longitudinalep[indexOfArray][index - 1].z);
                                            break;
                                        case 2:
                                            Longitudinalep[indexOfArray][index - 1] = new Vector3(Longitudinalep[indexOfArray][index - 1].x, Longitudinalep[indexOfArray][index - 1].y, num);
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

        Debug.Log("Transversal");
        for(int i = 0; i < Transversalsp[0].Count; i++)
        {
            RebarAlgorithm ra = new RebarAlgorithm(Transversalsp[0][i], Transversalep[0][i], Transversalsp[1][i], Transversalep[1][i]);
            ra.mat = mat;
            ra.RunAlgorithm();
        }

        Debug.Log("Longitudinal");
        for (int i = 0; i < Longitudinalsp[0].Count; i++)
        {
            RebarAlgorithm ra = new RebarAlgorithm(Longitudinalsp[0][i], Longitudinalep[0][i], Longitudinalsp[1][i], Longitudinalep[1][i]);
            ra.mat = mat;
            ra.RunAlgorithm();
        }

        //List<Vector3> tran_asdesigned = new List<Vector3>();
        //List<Vector3> tran_asbuilt = new List<Vector3>();

        //Debug.Log("Transversal");
        //for (int j = 0; j < Transversalsp[0].Count; j++)
        //{
        //    tran_asdesigned.Add(Transversalsp[0][j]);
        //    tran_asdesigned.Add(Transversalep[0][j]);
        //    tran_asbuilt.Add(Transversalsp[1][j]);
        //    tran_asbuilt.Add(Transversalep[1][j]);

        //    //RebarAlgorithm rAl = new RebarAlgorithm(Transversalsp[0][j], Transversalep[0][j], Transversalsp[1][j], Transversalep[1][j]);
        //    //rAl.mat = mat;
        //    //rAl.RunAlgorithm();
        //}

        //List<Vector3> long_asdesigned = new List<Vector3>();
        //List<Vector3> long_asbuilt = new List<Vector3>();

        //Debug.Log("Longitudinal");
        //for (int j = 0; j < Longitudinalsp[0].Count; j++)
        //{
        //    tran_asdesigned.Add(Longitudinalsp[0][j]);
        //    tran_asdesigned.Add(Longitudinalep[0][j]);
        //    tran_asbuilt.Add(Longitudinalsp[1][j]);
        //    tran_asbuilt.Add(Longitudinalep[1][j]);

        //    //RebarAlgorithm rAl = new RebarAlgorithm(Longitudinalsp[0][j], Longitudinalep[0][j], Longitudinalsp[1][j], Longitudinalep[1][j]);
        //    //rAl.mat = mat;
        //    //rAl.RunAlgorithm();
        //}

        //AllRebarAlgorithm ara_l = new AllRebarAlgorithm(tran_asdesigned, tran_asbuilt);
        //ara_l.mat = mat;
        //ara_l.RunAlgorithm();
    }
}
