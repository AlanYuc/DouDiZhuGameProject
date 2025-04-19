using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteArray
{
    /// <summary>
    /// 默认的字节数组大小
    /// </summary>
    const int DEFAULT_SIZE = 1024;
    /// <summary>
    /// 初始大小
    /// </summary>
    private int initSize;
    /// <summary>
    /// 字节数组
    /// </summary>
    public byte[] bytes;
    /// <summary>
    /// 读的位置
    /// </summary>
    public int readIndex;
    /// <summary>
    /// 写的位置
    /// </summary>
    public int writeIndex;
    /// <summary>
    /// 字节数组的容量
    /// </summary>
    public int capacity;
    /// <summary>
    /// 当前读取的数据长度
    /// </summary>
    public int Length { get { return writeIndex - readIndex; } }
    /// <summary>
    /// 剩余数据的长度
    /// </summary>
    public int Remain { get { return capacity - writeIndex; } }

    /// <summary>
    /// 提供字节数组长度的构造函数
    /// </summary>
    /// <param name="size">数组长度</param>
    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        initSize = size;
        readIndex = 0;
        writeIndex = 0;
        capacity = size;
    }
    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        initSize = defaultBytes.Length;
        readIndex = 0;
        writeIndex = defaultBytes.Length;
        capacity = defaultBytes.Length;
    }

    /// <summary>
    /// 移动数据
    /// </summary>
    public void MoveBytes()
    {
        if(Length > 0)
        {
            Array.Copy(bytes, readIndex, bytes, 0, Length);
        }
        writeIndex = Length;
        readIndex = 0;
    }

    /// <summary>
    /// 设置数组大小
    /// </summary>
    /// <param name="size">新的大小</param>
    public void Resize(int size)
    {
        //如果请求的大小小于当前数据长度(Length)，直接返回不做任何操作
        //避免数据截断的风险
        if (size < Length)
        {
            return;
        }
        //如果请求的大小小于初始大小(initSize)，也直接返回
        //避免频繁分配过小的数组
        if (size < initSize)
        {
            return;
        }
        capacity = size;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIndex, newBytes, 0, Length);
        bytes = newBytes;
        writeIndex = Length;
        readIndex = 0;
    }
}
