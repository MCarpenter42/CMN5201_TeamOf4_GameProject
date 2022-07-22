using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

/*
public class UnityEvent_classType : UnityEvent<classType> { }
public class UnityEvent_classTypeArray : UnityEvent<classType[]> { }
public class UnityEvent_classTypeList : UnityEvent<List<classType>> { }
*/

#region [ C# DATA TYPES ]

public class UnityEvent_bool : UnityEvent<bool> { }
public class UnityEvent_boolArray : UnityEvent<bool[]> { }
public class UnityEvent_boolList : UnityEvent<List<bool>> { }

public class UnityEvent_byte : UnityEvent<byte> { }
public class UnityEvent_byteArray : UnityEvent<byte[]> { }
public class UnityEvent_byteList : UnityEvent<List<byte>> { }

public class UnityEvent_char : UnityEvent<char> { }
public class UnityEvent_charArray : UnityEvent<char[]> { }
public class UnityEvent_charList : UnityEvent<List<char>> { }

public class UnityEvent_decimal : UnityEvent<decimal> { }
public class UnityEvent_decimalArray : UnityEvent<decimal[]> { }
public class UnityEvent_decimalList : UnityEvent<List<decimal>> { }

public class UnityEvent_double : UnityEvent<double> { }
public class UnityEvent_doubleArray : UnityEvent<double[]> { }
public class UnityEvent_doubleList : UnityEvent<List<double>> { }

public class UnityEvent_dynamic : UnityEvent<dynamic> { }
public class UnityEvent_dynamicArray : UnityEvent<dynamic[]> { }
public class UnityEvent_dynamicList : UnityEvent<List<dynamic>> { }

public class UnityEvent_float : UnityEvent<float> { }
public class UnityEvent_floatArray : UnityEvent<float[]> { }
public class UnityEvent_floatList : UnityEvent<List<float>> { }

public class UnityEvent_int : UnityEvent<int> { }
public class UnityEvent_intArray : UnityEvent<int[]> { }
public class UnityEvent_intList : UnityEvent<List<int>> { }

public class UnityEvent_long : UnityEvent<long> { }
public class UnityEvent_longArray : UnityEvent<long[]> { }
public class UnityEvent_longList : UnityEvent<List<long>> { }

public class UnityEvent_object : UnityEvent<object> { }
public class UnityEvent_objectArray : UnityEvent<object[]> { }
public class UnityEvent_objectList : UnityEvent<List<object>> { }

public class UnityEvent_sbyte : UnityEvent<sbyte> { }
public class UnityEvent_sbyteArray : UnityEvent<sbyte[]> { }
public class UnityEvent_sbyteList : UnityEvent<List<sbyte>> { }

public class UnityEvent_short : UnityEvent<short> { }
public class UnityEvent_shortArray : UnityEvent<short[]> { }
public class UnityEvent_shortList : UnityEvent<List<short>> { }

public class UnityEvent_string : UnityEvent<string> { }
public class UnityEvent_stringArray : UnityEvent<string[]> { }
public class UnityEvent_stringList : UnityEvent<List<string>> { }

public class UnityEvent_uint : UnityEvent<uint> { }
public class UnityEvent_uintArray : UnityEvent<uint[]> { }
public class UnityEvent_uintList : UnityEvent<List<uint>> { }

public class UnityEvent_ulong : UnityEvent<ulong> { }
public class UnityEvent_ulongArray : UnityEvent<ulong[]> { }
public class UnityEvent_ulongList : UnityEvent<List<ulong>> { }

public class UnityEvent_ushort : UnityEvent<ushort> { }
public class UnityEvent_ushortArray : UnityEvent<ushort[]> { }
public class UnityEvent_ushortList : UnityEvent<List<ushort>> { }

#endregion

#region [ UNITY CLASSES ]

public class UnityEvent_UnityObject : UnityEvent<UnityEngine.Object> { }
public class UnityEvent_UnityObjectArray : UnityEvent<UnityEngine.Object[]> { }
public class UnityEvent_UnityObjectList : UnityEvent<List<UnityEngine.Object>> { }

public class UnityEvent_GameObject : UnityEvent<GameObject> { }
public class UnityEvent_GameObjectArray : UnityEvent<GameObject[]> { }
public class UnityEvent_GameObjectList : UnityEvent<List<GameObject>> { }

public class UnityEvent_Graphic : UnityEvent<Graphic> { }
public class UnityEvent_GraphicArray : UnityEvent<Graphic[]> { }
public class UnityEvent_GraphicList : UnityEvent<List<Graphic>> { }

#endregion
