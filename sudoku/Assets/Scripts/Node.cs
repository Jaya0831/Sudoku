using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    /// <summary>
    /// 格子上的数字,number=0时，说明该位置还未填入
    /// </summary>
    public int number;
    /// <summary>
    /// 行
    /// </summary>
    public int row;
    /// <summary>
    /// 列
    /// </summary>
    public int line;
    /// <summary>
    /// 是否是已经给定数字的格子
    /// </summary>
    public bool fix = false;
    /// <summary>
    /// 可填写的数字
    /// </summary>
    public List<int> possibleNumber;
}
