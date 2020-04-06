using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 输入框
    /// </summary>
    public GameObject inputBox;
    public GameObject panel;
    /// <summary>
    /// 数独
    /// </summary>
    ///
    private GameObject[,] sudoku = new GameObject[9, 9];
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                GameObject node = GameObject.Instantiate(inputBox, panel.transform);
                node.GetComponent<Node>().line = i;
                node.GetComponent<Node>().row = j;
            }
        }
    }
    /// <summary>
    /// 更新数字（Node中number和text）
    /// </summary>
    private void UpdateNumber(GameObject node, int num)
    {
        node.GetComponent<Node>().fix = true;
        node.GetComponent<Node>().number = num;
        node.GetComponentInChildren<Text>().text = "" + num;
    }

    public void GenerateAutomatic()
    {
        //HACK：让生成的更加随机
        GameObject[] inputFields = GameObject.FindGameObjectsWithTag("InputField");
        for (int i = 0; i < inputFields.Length; i++)
        {
            sudoku[i / 9, i % 9] = inputFields[i];
        }
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                sudoku[i, j].GetComponent<Node>().number = 0;
                sudoku[i, j].GetComponent<Node>().fix = false;
            }
        }
        GenerateASquare();
        if (Solve(0, 0) == true)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    UpdateNumber(sudoku[i, j], sudoku[i, j].GetComponent<Node>().number);
                }
            }
        }
    }
    private void GenerateASquare()
    {
        int temp_1 = Random.Range(0, 3);
        int temp_2 = Random.Range(0, 3);
        List<int> From0To9 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int j = 3 * temp_1; j < 3 * temp_1 + 3; j++)
        {
            for (int k = 3 * temp_2; k < 3 * temp_2 + 3; k++)
            {
                int temp = From0To9[Random.Range(0, From0To9.Count)];
                UpdateNumber(sudoku[j, k], temp);
                From0To9.Remove(temp);
            }
        }
    }
    public bool Solve(int line, int row)
    {
        Debug.Log(line + "," + row);
        // 完成
        if (line == 9) return true;
        // 这个格子不需要填
        if (sudoku[line, row].GetComponent<Node>().fix == true)
        {
            if (row == 8) return Solve(line + 1, 0);
            else return Solve(line, row + 1);
        }
        bool success = false;
        DeleteImpossibleNum(line, row);
        if (sudoku[line, row].GetComponent<Node>().possibleNumber.Count != 0)
        {
            for (int i = 0; i < sudoku[line, row].GetComponent<Node>().possibleNumber.Count; i++)
            {
                sudoku[line, row].GetComponent<Node>().number = sudoku[line, row].GetComponent<Node>().possibleNumber[i];
                if (row == 8) success = Solve(line + 1, 0);
                else success = Solve(line, row + 1);
                if (success) break;
            }
            if (success==false) sudoku[line, row].GetComponent<Node>().number = 0;
            return success;
        }
        else return false;
    }
    private void DeleteImpossibleNum(int line, int row)
    {
        sudoku[line, row].GetComponent<Node>().possibleNumber = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        if (sudoku[line, row].GetComponent<Node>().fix == true) return;
        for (int i = 0; i < 9; i++)
        {
            if (i != row && sudoku[line, i].GetComponent<Node>().number != 0)
            {
                sudoku[line, row].GetComponent<Node>().possibleNumber.Remove(sudoku[line, i].GetComponent<Node>().number);
            }
            if (i != line && sudoku[i, row].GetComponent<Node>().number != 0)
            {
                sudoku[line, row].GetComponent<Node>().possibleNumber.Remove(sudoku[i, row].GetComponent<Node>().number);
            }
        }
        for (int i = (line / 3) * 3; i < (line / 3) * 3 + 3; i++)
        {
            for (int j = (row / 3) * 3; j < (row / 3) * 3 + 3; j++)
            {
                if (i != line && j != row && sudoku[i, j].GetComponent<Node>().number != 0)
                {
                    sudoku[line, row].GetComponent<Node>().possibleNumber.Remove(sudoku[i, j].GetComponent<Node>().number);
                }
            }
        }
        //Debug.Log(line + "," + row);
        //for (int i = 0; i < sudoku[line, row].GetComponent<Node>().possibleNumber.Count; i++)
        //{
        //    Debug.Log(sudoku[line, row].GetComponent<Node>().possibleNumber[i]);
        //}
    }
    public void SolveTheSudoku()
    {
        //TODO: 如果没有生成数独，先生成数独再求解
        if (Solve(0, 0) == true)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    UpdateNumber(sudoku[i, j], sudoku[i, j].GetComponent<Node>().number);
                }
            }
        }
    }
    public void GenerateByPlayer()
    {
        GameObject[] inputFields = GameObject.FindGameObjectsWithTag("InputField");
        GameObject[] inputFieldsText = GameObject.FindGameObjectsWithTag("Text");
        Debug.Log("inputFields:" + inputFields.Length);
        Debug.Log("inputFieldText:" + inputFieldsText.Length);
        for (int i = 0; i < inputFields.Length; i++)
        {
            sudoku[i / 9, i % 9] = inputFields[i];
            if (inputFieldsText[i].GetComponent<Text>().text!="")
            {
                UpdateNumber(sudoku[i / 9, i % 9], inputFieldsText[i].GetComponent<Text>().text[0] - '0');
                inputFieldsText[i].SetActive(false);
                inputFields[i].GetComponentInChildren<Text>().enabled = true;
                //inputFields[i].GetComponentsInChildren<Transform>()[1]
                //sudoku[i / 9, i % 9].GetComponent<Node>().possibleNumber = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            }
        }
    }
}
