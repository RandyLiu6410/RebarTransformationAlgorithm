using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcelManagerFormwork : MonoBehaviour
{
    public string CSVName_Design;
    public string CSVName_Built;
    private List<string[][]> dataArrays = new List<string[][]>();
    private List<Vector3>[] Formworkpt = new List<Vector3>[2] { new List<Vector3>(), new List<Vector3>() };
    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            Formworkpt[0].Add(new Vector3());
            Formworkpt[1].Add(new Vector3());
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
            for(int j = 1; j < dataArrays[indexOfArray].Length; j++)
            { 
                float num;
                if (float.TryParse(dataArrays[indexOfArray][j][3], out num))
                {
                    Formworkpt[indexOfArray][j - 1] = new Vector3(float.Parse(dataArrays[indexOfArray][j][2]), float.Parse(dataArrays[indexOfArray][j][3]), float.Parse(dataArrays[indexOfArray][j][4]));
                }
            }
        }

        Debug.Log("Formwork");
        FormworkAlgorithm fAl = new FormworkAlgorithm(Formworkpt[0], Formworkpt[1]);
        fAl.mat = mat;
        fAl.RunAlgorithm();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
