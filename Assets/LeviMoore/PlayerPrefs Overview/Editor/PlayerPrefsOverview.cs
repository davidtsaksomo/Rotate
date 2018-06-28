using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

public class PlayerPrefsOverview : EditorWindow
{
    [MenuItem("Window/PlayerPrefs Overview")]
    public static void Init()
    {
         GetWindow(typeof(PlayerPrefsOverview), false, "PlayerPrefs Overview");

    }
    void OnEnable()
    {
        sortType = (SortType)Enum.Parse(typeof(SortType), PlayerPrefs.GetString("PlayerPrefsOverview_SortType", "NameAZ"));
        searchType = (SearchType)Enum.Parse(typeof(SearchType), PlayerPrefs.GetString("PlayerPrefsOverview_SearchType", "Name"));
        searchWord = PlayerPrefs.GetString("PlayerPrefsOverview_SearchWord", "");
        keyTypeList = Enum.GetNames(typeof(KeyType));
        UpdateKeyList();
    }

    List<KeyData> keys = new List<KeyData>();
    List<KeyData> searchKeys = new List<KeyData>();
    List<string> ignoreKeys = new List<string>() { "UnityGraphicsQuality", "PlayerPrefsOverview_SortType", "PlayerPrefsOverview_SearchWord", "PlayerPrefsOverview_SearchType" };

    private Vector2 scrollPos;
    private bool selectedAllMixed;
    private bool selectedAll;
    private bool autoRefresh;
    private float autoRefreshRate = 1.5f;
    private float autoRefreshRateTmp;
    private SortType sortType;
    private SearchType searchType;
    private KeyType keyType;

    private Rect actionRect = new Rect(0, 20, 0, 0);
    private Rect sortRect = new Rect(45, 20, 0, 0);
    private Rect editRect;
    private Rect searchRect;

    private bool showNew;
    private string newName;
    private bool newNameError;
    private string newValueString;
    private int newValueInt;
    private float newValueFloat;
    private int newKeyTypeIndex;

    private string searchWord;

    void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor && autoRefresh)
        {
            float _autoRefreshRateTmp = Mathf.Repeat((float)EditorApplication.timeSinceStartup, autoRefreshRate);
            if (_autoRefreshRateTmp < autoRefreshRateTmp)
            {
                UpdateKeyList();
                Repaint();
            }
            autoRefreshRateTmp = _autoRefreshRateTmp;
        }
    }

    void OnGUI()
    {
        GUI.color = Color.white;
        GUIStyle keyBackground1 = new GUIStyle();
        keyBackground1.normal.background = MakeTex(600, 1, new Color(0f, 0f, 0f, 0.05f));
        GUIStyle keyBackground2 = new GUIStyle();
        keyBackground2.normal.background = MakeTex(600, 1, new Color(0f, 0f, 0f, 0.1f));
        GUIStyle keyInput = new GUIStyle();
        keyInput = EditorStyles.numberField;


        #region Toolbar
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Button(new GUIContent("Action"), EditorStyles.toolbarButton, GUILayout.Width(45)))
            {
                GUI.FocusControl("");
                GenericMenu _action = new GenericMenu();
                _action.AddItem(new GUIContent("Save", "Save all selected"), false, ActionSave);
                _action.AddItem(new GUIContent("Reset", "Reset all selected"), false, ActionReset);
                _action.AddItem(new GUIContent("Delete", "Delete all selected"), false, ActionDelete);
                _action.DropDown(actionRect);
            }
            if (GUILayout.Button(new GUIContent("Sort"), EditorStyles.toolbarButton, GUILayout.Width(35)))
            {
                GUI.FocusControl("");
                GenericMenu _sort = new GenericMenu();
                _sort.AddItem(new GUIContent("Name: A-Z", ""), (sortType == SortType.NameAZ) ? true : false, SortKeyList, SortType.NameAZ);
                _sort.AddItem(new GUIContent("Name: Z-A", ""), (sortType == SortType.NameZA) ? true : false, SortKeyList, SortType.NameZA);
                _sort.AddItem(new GUIContent("Value: A-Z", ""), (sortType == SortType.ValueAZ) ? true : false, SortKeyList, SortType.ValueAZ);
                _sort.AddItem(new GUIContent("Value: Z-A", ""), (sortType == SortType.ValueZA) ? true : false, SortKeyList, SortType.ValueZA);
                _sort.AddItem(new GUIContent("Type: A-Z", ""), (sortType == SortType.TypeAZ) ? true : false, SortKeyList, SortType.TypeAZ);
                _sort.AddItem(new GUIContent("Type: Z-A", ""), (sortType == SortType.TypeZA) ? true : false, SortKeyList, SortType.TypeZA);
                _sort.DropDown(sortRect);
            }

            GUILayout.Space(5);
            if (searchWord != (searchWord = EditorGUILayout.TextField(searchWord, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true))))
            {
                Search();
            }
            if (GUILayout.Button("▼  .", EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                searchRect = new Rect(this.position.width - 155, 20, 0, 0);
                GUI.FocusControl("");
                GenericMenu _search = new GenericMenu();
                _search.AddItem(new GUIContent("Name", ""), (searchType == SearchType.Name) ? true : false, SearchTypeList, SearchType.Name);
                _search.AddItem(new GUIContent("Value", ""), (searchType == SearchType.Value) ? true : false, SearchTypeList, SearchType.Value);
                _search.DropDown(searchRect);
            }


            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (autoRefresh != (autoRefresh = GUILayout.Toggle(autoRefresh, new GUIContent("Auto", "Automatically refreshes"), EditorStyles.toolbarButton, GUILayout.Width(35))))
                {
                    GUI.FocusControl("");
                }
            }
            if (GUILayout.Button(new GUIContent("Refresh", "Force a refresh"), EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                GUI.FocusControl("");
                UpdateKeyList();
            }
            if (GUILayout.Button(new GUIContent("Edit"), EditorStyles.toolbarButton, GUILayout.Width(35)))
            {
                editRect = new Rect(this.position.width - 45, 20, 0, 0);
                GUI.FocusControl("");
                GenericMenu _edit = new GenericMenu();
                _edit.AddItem(new GUIContent("New", ""), false, New);
                _edit.AddSeparator("");
                _edit.AddItem(new GUIContent("Import", ""), false, Import);
                _edit.AddItem(new GUIContent("Export", ""), false, Export, true);
                _edit.AddItem(new GUIContent("Export Selected", ""), false, Export, false);
                _edit.DropDown(editRect);
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion
        #region Top
        EditorGUILayout.BeginHorizontal();
        {
            if (selectedAllMixed)
            {
                EditorGUI.showMixedValue = true;
                if (EditorGUILayout.Toggle(false, "", GUILayout.Width(15)))
                {
                    selectedAll = false;
                    SelectedAll(selectedAll);
                }

                EditorGUI.showMixedValue = false;
            }
            else
            {
                if (selectedAll != (selectedAll = GUILayout.Toggle(selectedAll, new GUIContent("", "Toggle all"), GUILayout.Width(15))))
                {
                    SelectedAll(selectedAll);
                }
            }

            GUILayout.Space(3);
            GUILayout.Label("Name", GUILayout.MinWidth(70));
            GUILayout.Space(10);
            GUILayout.Label("Value", GUILayout.MinWidth(70));

            GUILayout.Label("Type", GUILayout.Width(40));
            GUILayout.Space(80);
        }
        EditorGUILayout.EndHorizontal();
        #endregion
        #region New
        if (showNew)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(28);
                if (newNameError)
                {
                    GUI.color = Color.red;
                }
                if (newName != (newName = EditorGUILayout.TextField(newName, keyInput, GUILayout.MinWidth(70))))
                {
                    newNameError = false;
                }
                GUI.color = Color.white;
                GUILayout.Space(5);
                if (newKeyTypeIndex == 0)
                {
                    newValueString = EditorGUILayout.TextField(newValueString, keyInput, GUILayout.MinWidth(70));
                }
                else if (newKeyTypeIndex == 1)
                {
                    newValueFloat = EditorGUILayout.FloatField(newValueFloat, keyInput, GUILayout.MinWidth(70));
                }
                else if (newKeyTypeIndex == 2)
                {
                    newValueInt = EditorGUILayout.IntField(newValueInt, keyInput, GUILayout.MinWidth(70));
                }
                newKeyTypeIndex = EditorGUILayout.Popup(newKeyTypeIndex, keyTypeList, GUILayout.Width(50));

                GUI.color = new Color(0.66f, 0.86f, 0.7f);
                if (GUILayout.Button(new GUIContent("Add", "Add new key"), GUILayout.Width(40)))
                {
                    GUI.FocusControl("");
                    AddKey();
                }
                GUI.color = new Color(0.89f, 0.54f, 0.54f);
                if (GUILayout.Button(new GUIContent("X", "Close"), GUILayout.Width(22)))
                {
                    GUI.FocusControl("");
                    showNew = false;
                    newNameError = false;
                }
                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
        #endregion
        #region Main
        if (searchKeys.Count != 0)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                for (int i = 0; i < searchKeys.Count; i++)
                {
                    if (searchKeys[i].hasChanged)
                    {
                        keyInput.font = EditorStyles.boldFont;
                    }
                    else
                    {
                        keyInput.font = EditorStyles.standardFont;
                    }

                    if (i % 2 == 0)
                    {
                        EditorGUILayout.BeginHorizontal(keyBackground1);
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal(keyBackground2);
                    }

                    if (searchKeys[i].selected != (searchKeys[i].selected = GUILayout.Toggle(searchKeys[i].selected, new GUIContent("", "Selected"), GUILayout.Width(15))))
                    {
                        Selected();
                    }
                    GUILayout.Space(5);

                    if (searchKeys[i].name != (searchKeys[i].name = EditorGUILayout.TextField(searchKeys[i].name, keyInput, GUILayout.MinWidth(70))))
                    {
                        searchKeys[i].Edit();
                    }
                    GUILayout.Space(5);

                    KeyType _type = searchKeys[i].type;
                    if (_type == KeyType.String)
                    {
                        if (searchKeys[i].value != (searchKeys[i].value = EditorGUILayout.TextField((string)searchKeys[i].value, keyInput, GUILayout.MinWidth(70))))
                        {
                            searchKeys[i].Edit();
                        }
                    }
                    else if (_type == KeyType.Int)
                    {
                        if (searchKeys[i].value != (searchKeys[i].value = EditorGUILayout.IntField((int)searchKeys[i].value, keyInput, GUILayout.MinWidth(70))))
                        {
                            searchKeys[i].Edit();
                        }
                    }
                    else if (_type == KeyType.Float)
                    {
                        if (searchKeys[i].value != (searchKeys[i].value = EditorGUILayout.FloatField((float)searchKeys[i].value, keyInput, GUILayout.MinWidth(70))))
                        {
                            searchKeys[i].Edit();
                        }
                    }

                    GUILayout.Label(searchKeys[i].type.ToString(), GUILayout.Width(40));

                    if (!searchKeys[i].hasChanged)
                    {
                        GUI.enabled = false;
                    }
                    GUI.color = new Color(0.66f, 0.86f, 0.7f);
                    if (GUILayout.Button(new GUIContent("S", "Save"), GUILayout.Width(22)))
                    {
                        GUI.FocusControl("");
                        searchKeys[i].Save();
                    }
                    GUI.color = new Color(0.9f, 0.9f, 0.15f);
                    if (GUILayout.Button(new GUIContent("R", "Reset"), GUILayout.Width(22)))
                    {
                        GUI.FocusControl("");
                        searchKeys[i].Reset();
                    }
                    GUI.color = new Color(0.89f, 0.54f, 0.54f);
                    GUI.enabled = true;
                    if (GUILayout.Button(new GUIContent("D", "Delete"), GUILayout.Width(22)))
                    {
                        GUI.FocusControl("");
                        searchKeys[i].Delete();
                        UpdateKeyList();
                        Search();
                    }
                    GUI.color = Color.white;

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        #endregion
        #region Info
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            GUILayout.Label("Version: 1.0");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Support: unity@levimoore.dk");
        }
        EditorGUILayout.EndHorizontal();
        #endregion
    }

    private void SearchTypeList(object _type)
    {
        searchType = (SearchType)_type;
        Search();
        SortKeyList(sortType);
        PlayerPrefs.SetString("PlayerPrefsOverview_SearchType", _type.ToString());
    }

    private void Search()
    {
        if (searchWord == "" || searchWord == " ")
        {
            searchKeys = keys;
        }
        else
        {
            if (searchType == SearchType.Name)
            {
                searchKeys = keys.Where(o => keys.Any(folder => o.name.ToLower().Contains(searchWord.ToLower()))).ToList();
            }
            else if (searchType == SearchType.Value)
            {
                searchKeys = keys.Where(o => keys.Any(folder => o.value.ToString().ToLower().Contains(searchWord.ToLower()))).ToList();
            }
        }
        SortKeyList(sortType);
        Selected();
        PlayerPrefs.SetString("PlayerPrefsOverview_SearchWord", searchWord);
    }

    private void Export(object _all)
    {
        Dictionary<string, object> _keys = new Dictionary<string, object>();
        for (int i = 0; i < searchKeys.Count; i++)
        {
            bool _add = true;
            if (!(bool)_all)
            {
                if (!searchKeys[i].selected)
                {
                    _add = false;
                }
            }
            if (_add)
            {
                _keys.Add(searchKeys[i].name, searchKeys[i].value);
            }
        }

        string _path = EditorUtility.SaveFilePanelInProject("Export PlayPrefs Keys", "PlayerPrefs", "ppo", "Export PlayPrefs Keys");
        if (!string.IsNullOrEmpty(_path))
        {
            string xml = PlistCS.Plist.writeXml(_keys);
            File.WriteAllText(_path, xml);
            AssetDatabase.Refresh();
        }
    }

    private void Import()
    {
        string _path = EditorUtility.OpenFilePanel("Import PlayerPrefs Keys", "Assets", "ppo");
        if (!string.IsNullOrEmpty(_path))
        {
            FileInfo _file = new FileInfo(_path);
            Dictionary<string, object> _keys = (Dictionary<string, object>)PlistCS.Plist.readPlist(_file.FullName);

            foreach (KeyValuePair<string, object> _key in _keys)
            {
                string _name = _key.Key;
                object _value = _key.Value;

                if (_value is string)
                {
                    PlayerPrefs.SetString(_name, (string)_value);
                }
                else if (_value is int)
                {
                    PlayerPrefs.SetInt(_name, (int)_value);
                }
                else if (_value is float)
                {
                    PlayerPrefs.SetFloat(_name, (float)_value);
                }

                UpdateKeyList();
                Repaint();
            }
        }
    }

    private void New()
    {
        newName = "";
        newNameError = false;
        newValueString = "";
        newValueInt = 0;
        newValueFloat = 0;
        newKeyTypeIndex = 0;
        showNew = true;
    }

    private void AddKey()
    {
        if (newName == "")
        {
            newNameError = true;
            return;
        }

        if (newKeyTypeIndex == 0)
        {
            PlayerPrefs.SetString(newName, newValueString);
        }
        else if (newKeyTypeIndex == 1)
        {
            PlayerPrefs.SetFloat(newName, newValueFloat);
        }
        else if (newKeyTypeIndex == 2)
        {
            PlayerPrefs.SetInt(newName, newValueInt);
        }

        keys.Add(new KeyData(newName));
        Search();
        SortKeyList(sortType);
        showNew = false;
        newNameError = false;
    }

    private void SortKeyList(object _sortType)
    {
        SortType _type = (SortType)_sortType;
        List<KeyData> _sortedList = null;
        if (_type == SortType.NameAZ)
        {
            _sortedList = searchKeys.OrderBy(o => o.name).ToList();
        }
        else if (_type == SortType.NameZA)
        {
            _sortedList = searchKeys.OrderBy(o => o.name).Reverse().ToList();
        }
        else if (_type == SortType.ValueAZ)
        {
            _sortedList = searchKeys.OrderBy(o => o.value.ToString()).ToList();
        }
        else if (_type == SortType.ValueZA)
        {
            _sortedList = searchKeys.OrderBy(o => o.value.ToString()).Reverse().ToList();
        }
        else if (_type == SortType.TypeAZ)
        {
            _sortedList = searchKeys.OrderBy(o => o.type.ToString()).ToList();
        }
        else if (_type == SortType.TypeZA)
        {
            _sortedList = searchKeys.OrderBy(o => o.type.ToString()).Reverse().ToList();
        }
        searchKeys = _sortedList;
        sortType = _type;
        PlayerPrefs.SetString("PlayerPrefsOverview_SortType", _type.ToString());
    }

    private void ActionDelete()
    {
        for (int i = 0; i < searchKeys.Count; i++)
        {
            if (searchKeys[i].selected)
            {
                searchKeys[i].Delete();
            }
        }
        UpdateKeyList();
        Selected();
    }

    private void ActionReset()
    {
        for (int i = 0; i < searchKeys.Count; i++)
        {
            if (searchKeys[i].selected)
            {
                searchKeys[i].Reset();
            }
        }
        Selected();
    }

    private void ActionSave()
    {
        for (int i = 0; i < searchKeys.Count; i++)
        {
            if (searchKeys[i].selected)
            {
                searchKeys[i].Save();
            }
        }
        UpdateKeyList();
        Selected();
    }
 
    private void Selected()
    {
        int _falseCount = 0;
        int _trueCount = 0;
        for (int i = 0; i < searchKeys.Count; i++)
        {
            if (searchKeys[i].selected)
            {
                _trueCount++;
            }
            else
            {
                _falseCount++;
            }
        }

        if (_falseCount == 0)
        {
            selectedAllMixed = false;
            selectedAll = true;
        }
        else if (_trueCount == 0)
        {
            selectedAllMixed = false;
            selectedAll = false;
        }
        else
        {
            selectedAllMixed = true;
        }
    }
    private void SelectedAll(bool _selectedAll)
    {
        selectedAllMixed = false;
        for (int i = 0; i < searchKeys.Count; i++)
        {
            searchKeys[i].selected = _selectedAll;
        }
    }

    void UpdateKeyList()
    {
        string[] _keys = GetAllKeys();
        keys = new List<KeyData>();
        for (int i = 0; i < _keys.Length; i++)
        {
            if (!ignoreKeys.Contains(_keys[i]))
            {
                keys.Add(new KeyData(_keys[i]));
            }
        }

        Search();
        SortKeyList(sortType);
    }

    string[] GetAllKeys()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            RegistryKey _currentUser = Registry.CurrentUser;
            RegistryKey _registryKey = _currentUser.CreateSubKey("Software\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);

            string[] _keys = _registryKey.GetValueNames();
            for (int i = 0; i < _keys.Length; i++)
            {
                _keys[i] = _keys[i].Substring(0, _keys[i].LastIndexOf("_"));
            }

            return _keys;
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            string _path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist";
            string[] _keys = new string[0];

            if (File.Exists(_path))
            {
                FileInfo _file = new FileInfo(_path);
                Dictionary<string, object> _list = (Dictionary<string, object>)PlistCS.Plist.readPlist(_file.FullName);

                _keys = new string[_list.Count];
                _list.Keys.CopyTo(_keys, 0);
            }

            return _keys;
        }
        else
        {
            string[] _keys = new string[0];
            return _keys;
        }
    }

    private class KeyData : IComparable<KeyData>
    {
        public string name, nameDefault;
        public object value, valueDefault;
        public KeyType type;
        public bool selected;
        public bool hasChanged;

        public KeyData(string _key)
        {
            name = _key;

            if (PlayerPrefs.GetString(_key, "PlayerPrefsOverviewString[][];") != "PlayerPrefsOverviewString[][];")
            {
                type = KeyType.String;
                value = PlayerPrefs.GetString(_key);
            }
            else if (PlayerPrefs.GetInt(_key, 0) != 0 || PlayerPrefs.GetInt(_key, 1) != 1)
            {
                type = KeyType.Int;
                value = PlayerPrefs.GetInt(_key);
            }
            else if (PlayerPrefs.GetFloat(_key, 0.5f) != 0.5f || PlayerPrefs.GetFloat(_key, 1.5f) != 1.5f)
            {
                type = KeyType.Float;
                value = PlayerPrefs.GetFloat(_key);
            }

            nameDefault = _key;
            valueDefault = value;
        }

        public void Edit()
        {
            if (name == nameDefault && value.ToString() == valueDefault.ToString())
            {
                hasChanged = false;
            }
            else
            {
                hasChanged = true;
            }
        }
        public void Save()
        {
            Delete();
            if (type == KeyType.String)
            {
                PlayerPrefs.SetString(name, (string)value);
            }
            else if (type == KeyType.Int)
            {
                PlayerPrefs.SetInt(name, (int)value);
            }
            else if (type == KeyType.Float)
            {
                PlayerPrefs.SetFloat(name, (float)value);
            }

            nameDefault = name;
            valueDefault = value;
            Edit();
        }
        public void Reset()
        {
            name = nameDefault;
            value = valueDefault;
            Edit();
        }
        public void Delete()
        {
            PlayerPrefs.DeleteKey(nameDefault);
        }

        public int CompareTo(KeyData _key)
        {
            return this.name.CompareTo(_key.name);
        }
    }
    private enum KeyType
    {
        String,
        Float,
        Int
    }
    private string[] keyTypeList;
    private enum SortType
    {
        NameAZ,
        NameZA,
        ValueAZ,
        ValueZA,
        TypeAZ,
        TypeZA
    }
    private enum SearchType
    {
        Name,
        Value
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}