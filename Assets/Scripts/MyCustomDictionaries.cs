using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Author: Daryl Keogh
/// Description: This class is an editor only class and is used to define a dictionary of Strings to a pair of Ints
/// </summary>

[Serializable]
public class IntIntPair
{
    public int first;
    public int second;
}

[Serializable]
public class IntIntDictionary : SerializableDictionary<string, IntIntPair> { }