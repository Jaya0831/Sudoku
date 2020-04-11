using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 输入框
    /// </summary>
    public GameObject inputBox;
    public GameObject panel;
    public Button[] buttons;
    public GameObject wrongText;
    public GameObject canvas;
    /// <summary>
    /// 红色
    /// </summary>
    public Material wrong;
    /// <summary>
    /// 对应每个按钮
    /// </summary>
    enum Buttonable { GenerateByPlayer, GenerateAutomatic, SolveTheSudoku, Judge, Renew };
    /// <summary>
    /// 控制按钮交互的规则
    /// </summary>
    int[,] rule = { { 0, 0, 1, 1, 1 }, { 0, 1, 0, 0, 1 }, { 0, 0, 0, 0, 1 }, { 0, 0, 0, 1, 1 }, { 1, 1, 0, 0, 0 } };
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
                sudoku[i, j] = node;
            }
        }
    }
    /// <summary>
    /// 控制按钮是否可交互
    /// </summary>
    private void ButtonController(Buttonable button)
    {
        for (int i = 0; i < 5; i++)
        {
            buttons[i].interactable = rule[(int)button, i] == 1 ? true : false;
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
    /// <summary>
    /// 随机生成完整的数独
    /// </summary>
    public void GenerateAutomatic()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                sudoku[i, j].GetComponent<Node>().number = 0;
                sudoku[i, j].GetComponent<Node>().fix = false;
            }
        }
        GenerateASquare();
        if (Solve(true, 0, 0) == true)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    UpdateNumber(sudoku[i, j], sudoku[i, j].GetComponent<Node>().number);
                    sudoku[i, j].GetComponentsInChildren<Text>()[1].enabled = false;
                    sudoku[i, j].GetComponentsInChildren<Text>()[0].enabled = true;
                }
            }
        }
        ButtonController(Buttonable.GenerateAutomatic);
    }
    /// <summary>
    /// 随机生成一个九宫格
    /// </summary>
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
    /// <summary>
    /// 数独求解 
    /// </summary>
    /// <param name="random"></param>
    /// <param name="line"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool Solve(bool random, int line, int row)
    {
        // 完成
        if (line == 9) return true;
        // 这个格子不需要填
        if (sudoku[line, row].GetComponent<Node>().fix == true)
        {
            if (row == 8) return Solve(random, line + 1, 0);
            else return Solve(random, line, row + 1);
        }
        bool success = false;
        DeleteImpossibleNum(line, row);
        if (sudoku[line, row].GetComponent<Node>().possibleNumber.Count != 0)
        {
            if (random)
            {
                List<int> temp = new List<int>();
                for (int i = 0; i < sudoku[line, row].GetComponent<Node>().possibleNumber.Count; i++)
                {
                    temp.Add(sudoku[line, row].GetComponent<Node>().possibleNumber[i]);
                    sudoku[line, row].GetComponent<Node>().possibleNumber.RemoveAt(i);
                }
                while (temp.Count != 0)
                {
                    int index = Random.Range(0, temp.Count);
                    sudoku[line, row].GetComponent<Node>().possibleNumber.Add(temp[index]);
                    temp.RemoveAt(index);
                }
            }
            for (int i = 0; i < sudoku[line, row].GetComponent<Node>().possibleNumber.Count; i++)
            {
                sudoku[line, row].GetComponent<Node>().number = sudoku[line, row].GetComponent<Node>().possibleNumber[i];
                if (row == 8) success = Solve(random, line + 1, 0);
                else success = Solve(random, line, row + 1);
                if (success) break;
            }
            if (success == false) sudoku[line, row].GetComponent<Node>().number = 0;
            return success;
        }
        else return false;
    }
    /// <summary>
    /// 删除不可能的数字 
    /// </summary>
    /// <param name="line"></param>
    /// <param name="row"></param>
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
    }
    /// <summary>
    /// 求解数独（part）
    /// </summary>
    public void SolveTheSudoku()
    {
        if (buttons[(int)Buttonable.GenerateByPlayer] == true)
        {
            GenerateByPlayer();
        }
        if (Solve(false, 0, 0) == true)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    UpdateNumber(sudoku[i, j], sudoku[i, j].GetComponent<Node>().number);
                }
            }
        }
        ButtonController(Buttonable.SolveTheSudoku);
    }
    /// <summary>
    /// 生成玩家输入的数独 
    /// </summary>
    public void GenerateByPlayer()
    {
        bool generate = true;
        for (int i = 0; i < 9; i++)
        {
            List<int> temp_1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int j = 0;
            while (j < 9 && generate)
            {
                if (sudoku[i, j].GetComponentsInChildren<Text>()[1].text != "")
                {
                    if (sudoku[i, j].GetComponentsInChildren<Text>()[1].text.Length != 1 || sudoku[i, j].GetComponentsInChildren<Text>()[1].text[0] == 0)
                    {
                        generate = false;
                        break;
                    }
                    generate = temp_1.Remove(sudoku[i, j].GetComponentsInChildren<Text>()[1].text[0] - '0');
                }
                j++;
            }
            List<int> temp_2 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            j = 0;
            while (j < 9 && generate)
            {
                if (sudoku[j, i].GetComponentsInChildren<Text>()[1].text != "")
                {
                    if (sudoku[j, i].GetComponentsInChildren<Text>()[1].text.Length != 1 || sudoku[j, i].GetComponentsInChildren<Text>()[1].text[0] == 0)
                    {
                        generate = false;
                        break;
                    }
                    generate = temp_2.Remove(sudoku[j, i].GetComponentsInChildren<Text>()[1].text[0] - '0');
                }
                j++;
            }
            List<int> temp_3 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            j = 0;
            while (j < 9 && generate)
            {
                int k = 3 * (i / 3) + j / 3, m = 3 * (i % 3) + j % 3;
                if (sudoku[k, m].GetComponentsInChildren<Text>()[1].text != "")
                {
                    if (sudoku[k, m].GetComponentsInChildren<Text>()[1].text.Length != 1 || sudoku[k, m].GetComponentsInChildren<Text>()[1].text[0] == 0)
                    {
                        generate = false;
                        break;
                    }
                    generate = temp_3.Remove(sudoku[k, m].GetComponentsInChildren<Text>()[1].text[0] - '0');
                }
                j++;
            }
            if (!generate)
            {
                StartCoroutine(WrongInput());
                break;
            }
        }
        if (!generate) return;
        GameObject[] inputFields = GameObject.FindGameObjectsWithTag("InputField");
        GameObject[] inputFieldsText = GameObject.FindGameObjectsWithTag("Text");
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFieldsText[i].GetComponent<Text>().text != "")
            {
                UpdateNumber(sudoku[i / 9, i % 9], inputFieldsText[i].GetComponent<Text>().text[0] - '0');
                inputFieldsText[i].SetActive(false);
                inputFields[i].GetComponent<InputField>().enabled = false;
                inputFields[i].GetComponentInChildren<Text>().enabled = true;
            }
        }
        ButtonController(Buttonable.GenerateByPlayer);
    }
    IEnumerator WrongInput()
    {
        GameObject text = GameObject.Instantiate(wrongText, new Vector3(500, 25, 1), Quaternion.identity, canvas.transform);
        yield return new WaitForSeconds(3);
        Destroy(text);
    }
    /// <summary>
    /// 检验数独
    /// </summary>
    public void Judge()
    {
        bool[,] judge_false = new bool[9, 9];
        for (int i = 0; i < 9; i++)
        {
            List<int> temp_1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int j = 0; j < 9; j++)
            {
                if (sudoku[i, j].GetComponent<Node>().fix == true)
                {
                    temp_1.Remove(sudoku[i, j].GetComponent<Node>().number);
                }
                else
                {
                    if (sudoku[i, j].GetComponentsInChildren<Text>()[1].text != "") temp_1.Remove(sudoku[i, j].GetComponentsInChildren<Text>()[1].text[0] - '0');
                }
            }
            if (temp_1.Count != 0)
            {
                for (int j = 0; j < 9; j++)
                {
                    judge_false[i, j] = true;
                }
            }
            List<int> temp_2 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int j = 0; j < 9; j++)
            {
                if (sudoku[j, i].GetComponent<Node>().fix == true)
                {
                    temp_2.Remove(sudoku[j, i].GetComponent<Node>().number);
                }
                else
                {
                    if (sudoku[j, i].GetComponentsInChildren<Text>()[1].text != "") temp_2.Remove(sudoku[j, i].GetComponentsInChildren<Text>()[1].text[0] - '0');
                }
            }
            if (temp_2.Count != 0)
            {
                for (int j = 0; j < 9; j++)
                {
                    judge_false[j, i] = true;
                }
            }
        }
        for (int i = 0; i < 9; i++)
        {
            List<int> temp_1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int k = 3 * (i / 3); k < 3 * (i / 3) + 3; k++)
            {
                for (int m = 3 * (i % 3); m < 3 * (i % 3) + 3; m++)
                {
                    if (sudoku[k, m].GetComponent<Node>().fix == true)
                    {
                        temp_1.Remove(sudoku[k, m].GetComponent<Node>().number);
                    }
                    else
                    {
                        if (sudoku[k, m].GetComponentsInChildren<Text>()[1].text != "") temp_1.Remove(sudoku[k, m].GetComponentsInChildren<Text>()[1].text[0] - '0');
                    }
                }
            }
            if (temp_1.Count != 0)
            {
                for (int k = 3 * (i / 3); k < 3 * (i / 3) + 3; k++)
                {
                    for (int m = 3 * (i % 3); m < 3 * (i % 3) + 3; m++)
                    {
                        judge_false[k, m] = true;
                    }
                }
            }
        }
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (judge_false[i, j]) sudoku[i, j].GetComponent<Image>().color = new Color(1, 0, 0, 0.2f);
                else sudoku[i, j].GetComponent<Image>().color = Color.white;
            }
        }
        ButtonController(Buttonable.Judge);
    }
    /// <summary>
    /// 重载场景
    /// </summary>
    public void Renew()
    {
        SceneManager.LoadScene(0);
        ButtonController(Buttonable.Renew);
    }
}
