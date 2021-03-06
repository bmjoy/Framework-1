﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PixelComrades {
    public class JsonDB {
        public const string Id = "id";
        public const string GenericEnumsSheet = "Enums";
        public const string GenericEnumColumn = "Enum";
        private const string EnumProp = "hasIndex";
        private const string EnumColumn = "index";

        private JsonParser _parser;
        private Dictionary<string, List<DataEntry>> _sheets = new Dictionary<string, List<DataEntry>>();

        public Dictionary<string, List<DataEntry>> Sheets { get => _sheets; }
        public JsonParser Parser { get => _parser; }

        public JsonDB(string input) {
            if (string.IsNullOrEmpty(input)) {
                return;
            }
            _parser = new JsonParser(input);
            SetupSheets();
        }

        public void SetupSheets() {
            for (var s = 0; s < _parser.Sheets.Count; s++) {
                var sheet = _parser.Sheets[s];
                var list = new List<DataEntry>();
                _sheets.Add(sheet.Name, list);
                for (int r = 0; r < sheet.Rows.Count; r++) {
                    JToken line = sheet.Rows[r];
                    var id = line[Id].ToString();
                    var entry = new DataEntry(id, string.Format("{0}.{1}", sheet.Name, id), sheet.Columns.Count);
                    entry.Index = r;
                    for (int c = 0; c < sheet.Columns.Count; c++) {
                        var columnName = sheet.Columns[c].Name;
                        if (columnName == id) {
                            entry.Cells.AddOrUpdate(Id, new DataCell<string>(Id, entry, id));
                            //entry.Cells[c] = new DataCell<string>(Id, entry, id);
                            continue;
                        }
                        if (columnName == EnumColumn) {
                            entry.Index = line[columnName].Value<int>();
                        }
                        if (line[columnName] == null) {
                            continue;
                        }
                        //entry.Cells[c] = GetColumnData(entry, line[columnName], sheet.Columns[c]);
                        var data = GetColumnData(entry, line[columnName], sheet.Columns[c]);
                        if (data != null) {
                            entry.Cells.AddOrUpdate(data.ID, data);
                        }
                    }
                    list.Add(entry);
                }
                if (sheet.HasProp(EnumProp)) {
                    if (sheet.Name == GenericEnumsSheet) {
                        AddGenericEnumsSheet(sheet);
                    }
                    else {
                        GameData.Enums.Add(sheet.Name, new FakeEnum(sheet));
                    }
                }
            }
            for (var s = 0; s < _parser.Nested.Count; s++) {
                var nestedSheet = _parser.Nested[s];
                var targetSheet = _parser.GetSheetWithName(nestedSheet.NestedParent);
                var column = targetSheet.GetColumn(nestedSheet.NestedColumn);
                if (column == null) {
                    Debug.Log(string.Format("No column for {0}", nestedSheet.Name));
                    continue;
                }
                var entryList = _sheets[targetSheet.Name];
                if (entryList == null) {
                    Debug.Log(string.Format("No list for {0}", column.Name));
                    continue;
                }
                for (int e = 0; e < entryList.Count; e++) {
                    var parentEntry = entryList[e];
                    if (parentEntry == null) {
                        Debug.Log(string.Format("No list for {0}", column.Name));
                        continue;
                    }
                    var list = parentEntry.Get(column.Name) as DataList;
                    if (list == null || nestedSheet.Columns.Count == 0) {
                        //Debug.Log(string.Format("No list for {0} at {1}", column.Name, parentEntry.ID));
                        continue;
                    }
                    for (int r = 0; r < list.TempData.Count; r++) {
                        JToken line = list.TempData[r];
                        string id = r.ToString();
                        if (line[Id] != null) {
                            id = line[Id].ToString();
                        }
                        var entry = new DataEntry(id, string.Format("{0}.{1}", list.FullID, id), nestedSheet.Columns.Count);
                        for (int c = 0; c < nestedSheet.Columns.Count; c++) {
                            var columnName = nestedSheet.Columns[c].Name;
                            if (columnName == id) {
                                //entry.Cells[c] = new DataCell<string>(Id, entry, id);
                                entry.Cells.AddOrUpdate(Id, new DataCell<string>(Id, entry, id));
                                continue;
                            }
                            if (line[columnName] == null) {
                                continue;
                            }
                            //entry.Cells[c] = GetColumnData(entry, line[columnName], nestedSheet.Columns[c]);
                            var data = GetColumnData(entry, line[columnName], nestedSheet.Columns[c]);
                            if (data != null) {
                                entry.Cells.AddOrUpdate(data.ID, data);
                            }
                        }
                        list.Value.Add(entry);
                    }
                    list.TempData = null;
                }
            }
        }

        private void AddGenericEnumsSheet(SheetNode sheet) {
            Dictionary<string, List<JToken>> enums = new Dictionary<string, List<JToken>>();
            for (int r = 0; r < sheet.Rows.Count; r++) {
                JToken line = sheet.Rows[r];
                var enumName = line[GenericEnumColumn].ToString();
                if (!enums.TryGetValue(enumName, out var list)) {
                    list = new List<JToken>();
                    enums.Add(enumName, list);
                }
                list.Add(line);
            }
            foreach (var enumType in enums) {
                var fakeEnum = new FakeEnum(enumType.Key, enumType.Value.Count);
                for (int i = 0; i < enumType.Value.Count; i++) {
                    fakeEnum.AddNode(enumType.Value[i], i);
                }
                GameData.Enums.Add(fakeEnum.TypeName, fakeEnum);
            }
        }

        private DataCell GetColumnData(DataEntry owner, JToken data, ColumnNode column) {
            if (owner == null) {
                Debug.LogErrorFormat("Column {0} at data {1} had null owner", column.Name, data.ToString());
                return null;
            }
            if (column == null) {
                Debug.LogErrorFormat("Owner {0} at data {1} had null column", owner.FullID, data.ToString());
                return null;
            }
            if (data == null) {
                Debug.LogErrorFormat("Owner {0} at column {1} had null data", owner.FullID, column.Name);
                return null;
            }
            DataCell cell = null;
            var typeNum = GetTypeNumFromCastleDBTypeString(column.TypeStr);
            switch (typeNum) {
                case "0":
                case "1":
                    //0 = UniqueIdentifier
                    //1 = Text
                    cell = new DataCell<string>(column.Name, owner, data.ToString());
                    break;
                case "2":
                    cell = new DataCell<bool>(column.Name, owner, data.Value<bool>());
                    break;
                case "3":
                    cell = new DataCell<int>(column.Name, owner, data.Value<int>());
                    break;
                case "4":
                    cell = new DataCell<float>(column.Name, owner, data.Value<float>());
                    break;
                case "6":
                    cell = new DataReference(column.Name, owner, GetTypeStrData(column.TypeStr), data.ToString(), this);
                    break;
                case "7":
                case "13":
                case "14":
                case "16":
                    //7 = Image
                    //13 = File
                    //14 = Tile
                    //16 = Dynamic
                    cell = new DataCell<string>(column.Name, owner, data.ToString());
                    break;
                case "9":
                    //Custom type
                    cell = new DataCell<string>(column.Name, owner, data.ToString());
                    break;
                case "11":
                    //Color
                    if (ColorUtility.TryParseHtmlString((data.Value<int>()).ToString("X"), out var color)) {
                        cell = new DataCell<Color>(column.Name, owner, color);
                    }
                    break;
                case "12":
                case "15":
                    //12 = Data Layer
                    //16 = Tile Layer
                    cell = new DataReference(column.Name, owner, GetTypeStrData(column.TypeStr), data.ToString(), this);
                    break;
                case "5":
                case "10":
                    //5 = Enum
                    //10 = Flags Enum
                    var enumMembers = GetEnumValuesFromTypeString(column.TypeStr);
                    var index = data.Value<int>();
                    cell = new DataEnum(column.Name, owner, enumMembers[index], index);
                    break;
                case "8": 
                    //List
                    var list = new DataList(column.Name, owner, new List<DataEntry>());
                    cell = list;
                    foreach (var item in data) {
                        list.TempData.Add(item);
                    }
                    break;
            }
            return cell;
        }

        private string GetTypeStrData(string entry) {
            var index = entry.LastIndexOf(':');
            if (index < 0) {
                for (int i = 0; i < entry.Length; i++) {
                    if (!System.Char.IsDigit(entry[i])) {
                        return entry.Substring(i);
                    }
                }
            }
            return entry.Substring(index + 1);
        }

        public static string[] GetEnumValuesFromTypeString(string inputString) {
            char delimiter1 = ':';
            char delimiter2 = ',';
            string[] init = inputString.Split(delimiter1);
            string[] enumValues = init[1].Split(delimiter2);
            return enumValues;
        }


        public static string GetTypeNumFromCastleDBTypeString(string inputString) {
            char delimiter = ':';
            string[] typeString = inputString.Split(delimiter);
            return typeString[0];
        }
    }

    public class JsonParser {

        public const string JsonSheets = "sheets";
        public const string JsonName = "name";
        public const string JsonDisplay = "display";
        public const string JsonType = "typeStr";
        public const string JsonColumns = "columns";
        public const string JsonLines = "lines";
        public const string JsonProps = "props";

        public List<SheetNode> Sheets { get; protected set; }
        public List<SheetNode> Nested { get; protected set; }

        public JsonParser(string text) {
            //var root = JSON.Parse(text);
            var root = JToken.Parse(text);
            Sheets = new List<SheetNode>();
            Nested = new List<SheetNode>();
            foreach (var item in root[JsonSheets]) {
                var sheet = new SheetNode(item);
                if (sheet.NestedType) {
                    Nested.Add(sheet);
                }
                else {
                    Sheets.Add(sheet);
                }
            }
        }

        public SheetNode GetSheetWithName(string name) {
            for (var i = 0; i < Sheets.Count; i++) {
                var item = Sheets[i];
                if (item.Name == name) {
                    return item;
                }
            }
            return null;
        }
    }
    public class ColumnNode {
        public ColumnNode(JToken sheetValue) {
            var value = sheetValue;
            Name = value[JsonParser.JsonName].ToString();
            TypeStr = value[JsonParser.JsonType].ToString();
        }

        public string TypeStr { get; protected set; }
        public string Name { get; protected set; }
    }

    public class SheetNode {
        public SheetNode(JToken sheetValue) {
            var value = sheetValue;
            Name = value[JsonParser.JsonName].ToString();
            char delimit = '@';
            var splitString = Name.Split(delimit);
            if (splitString.Length <= 1) {
                NestedType = false;
            }
            else {
                NestedParent = splitString[0];
                NestedColumn = splitString[1];
                NestedType = true;
            }
            Columns = new List<ColumnNode>();
            Rows = new List<JToken>();
            Props = value[JsonParser.JsonProps];
            foreach (var item in value[JsonParser.JsonColumns]) {
                Columns.Add(new ColumnNode(item));
            }
            foreach (var item in value[JsonParser.JsonLines]) {
                Rows.Add(item);
            }
        }

        public string NestedParent { get; }
        public string NestedColumn { get; }
        public bool NestedType { get; protected set; }
        public string Name { get; protected set; }
        public List<ColumnNode> Columns { get; protected set; }
        public List<JToken> Rows { get; protected set; }
        public JToken Props { get; protected set; }

        public ColumnNode GetColumn(string name) {
            for (var i = 0; i < Columns.Count; i++) {
                var item = Columns[i];
                if (item.Name == name) {
                    return item;
                }
            }
            return null;
        }

        public bool HasProp(string prop) {
            var targetProp = Props[prop];
            return targetProp != null && targetProp.Type == JTokenType.Boolean;
        }

        public int FindIdColumnIndex() {
            for (var i = 0; i < Columns.Count; i++) {
                if (Columns[i].Name == JsonDB.Id) {
                    return i;
                }
            }
            return 0;
        }
    }

    public abstract class DataCell {
        public abstract System.Object Get { get; }
        public abstract DataCell Clone();
        public string ID { get; }
        public string FullID { get; }

        protected DataCell(string id, DataEntry owner) {
            ID = id;
            FullID = string.Format("{0}.{1}", owner.ID, id);
        }

        protected DataCell(string fullID) {
            ID = fullID.Split('.')[0];
            FullID = fullID;
        }
    }

    public class DataCell<T> : DataCell {
        public T Value { get;}
        public System.Type Type { get; }
        public override System.Object Get { get { return Value; } }

        public DataCell(string id, DataEntry owner, T value) : base(id, owner) {
            Value = value;
            Type = typeof(T);
        }

        public DataCell(string fullID, T value) : base(fullID) {
            Value = value;
            Type = typeof(T);
        }

        public override DataCell Clone() {
            return new DataCell<T>(ID, Value);
        }
    }

    public class DataEnum : DataCell<string> {
        public int EnumIndex { get; }
        public override System.Object Get { get { return Value; } }

        public DataEnum(string id, DataEntry owner, string value, int index) : base(id, owner, value) {
            EnumIndex = index;
        }

        public DataEnum(string fullID, string value, int index) : base(fullID, value) {
            EnumIndex = index;
        }

        public override DataCell Clone() {
            return new DataEnum(ID, Value, EnumIndex);
        }
    }

    public class DataList : DataCell<List<DataEntry>> {
        public List<JToken> TempData = new List<JToken>();
        public DataList(string id, DataEntry owner, List<DataEntry> value) : base(id, owner, value) {}
        public DataList(string fullID, List<DataEntry> value) : base(fullID, value) {}

        public int Count { get { return Value.Count; } }
        public DataEntry this[int index] { get { return Value[index]; } }


        public T FindData<T>(string id, string targetField) {
            for (int i = 0; i < Value.Count; i++) {
                if (Value[i].ID == id) {
                    return Value[i].GetValue<T>(targetField);
                }
            }
            return default(T);
        }

        public bool HasID(string id) {
            for (int i = 0; i < Value.Count; i++) {
                if (Value[i].ID == id) {
                    return true;
                }
            }
            return false;
        }

        public override DataCell Clone() {
            return new DataList(ID, Value);
        }
    }

    public class DataReference : DataCell {

        public string TargetSheet { get; }
        public string TargetID { get; }
        private JsonDB _db;
        private DataEntry _entry;

        public override System.Object Get { get { return Value != null ? _entry.FullID : string.Format("Missing {0}.{1}", TargetSheet, TargetID); } }

        public DataEntry Value {
            get {
                if (_entry != null) {
                    return _entry;
                }
                var list = _db.Sheets[TargetSheet];
                for (int i = 0; i < list.Count; i++) {
                    if (list[i].ID == TargetID) {
                        _entry = list[i];
                        break;
                    }
                }
                return _entry;
            }
        }

        public DataReference(string id, DataEntry owner, string targetSheet, string targetId, JsonDB db) : base(id, owner) {
            TargetSheet = targetSheet;
            TargetID = targetId;
            _db = db;
        }

        public DataReference(string fullID, string targetSheet, string targetId, JsonDB db) : base(fullID) {
            TargetSheet = targetSheet;
            TargetID = targetId;
            _db = db;
        }

        public override DataCell Clone() {
            return new DataReference(ID, TargetSheet, TargetID, _db);
        }
    }

    public class DataEntry {
        public Dictionary<string, DataCell> Cells = new Dictionary<string, DataCell>();
        public string ID { get; private set; }
        public string FullID { get; private set; }
        public int Index = -1;

        public DataEntry Clone(string newId) {
            var clone = new DataEntry();
            var prefix = FullID.Split('.');
            clone.ID = newId;
            clone.FullID = string.Format("{0}.{1}", prefix.Length > 0 ? prefix[0] : "", newId);
            clone.Index = Index;
            //clone.Cells = new DataCell[Cells.Length];
            foreach (var dataEntry in Cells) {
                clone.Cells.Add(dataEntry.Key, dataEntry.Value.Clone());
            }
            //for (int i = 0; i < Cells.Length; i++) {
            //    clone.Cells[i] = Cells[i].Clone();
            //}
            return clone;
        }

        private DataEntry(){}

        public DataEntry(string id, string fullID, int cnt) {
            //Cells = new DataCell[cnt];
            ID = id;
            FullID = fullID;
        }

        public DataCell Get(string label) {
            return Cells.TryGetValue(label, out var entry) ? entry : null;
        }

        public T Get<T>(string field) where T : DataCell {
            var cell = Get(field);
            if (cell == null) {
                return null;
            }
            return (T) cell;
        }

        public void Replace<T>(string field, T value) where T : DataCell {
            if (Cells.ContainsKey(field)) {
                Cells[field] = value;
            }
            else {
                Cells.Add(field, value);
            }
        }

        public bool TryGet<T>(string field, out T value) where T : DataCell {
            var cell = Get(field);
            if (cell == null) {
                value = default(T);
                return false;
            }
            value = (T) cell;
            return true;
        }

        public T GetValue<T>(string field) {
            var cell = Get(field);
            if (cell == null) {
                return FindChildValue<T>(field);
            }
            if (cell is DataCell<T> typeCell) {
                return typeCell.Value;
            }
            return (T) cell.Get;
        }

        public int GetEnum(string field, int defaultValue) {
            return TryGetEnum(field, out var index) ? index : defaultValue;
        }

        public bool TryGetEnum(string field, out int enumIndex) {
            var cell = Get(field);
            if (cell == null) {
                enumIndex = 0;
                return false;
            }
            if (cell is DataEnum dataEnum) {
                enumIndex = dataEnum.EnumIndex;
                return true;
            }
            if (cell is DataReference refData) {
                enumIndex = refData.Value.Index;
                return true;
            }
            if (cell is DataCell<int> intData) {
                enumIndex = intData.Value;
                return true;
            }
            return GameData.Enums.TryGetEnumIndex(cell.Get.ToString(), out enumIndex);
        }

        public T TryGetValue<T>(string field, T defaultValue) {
            var cell = Get(field);
            if (cell == null) {
                return FindChildValue<T>(field, defaultValue);
            }
            if (cell is DataCell<T> typeCell) {
                return typeCell.Value;
            }
            if (typeof(T).IsEnum && cell is DataCell<string> stringCell) {
                return ParseUtilities.TryParseEnum(stringCell.Value, defaultValue);
            }
            if (cell.Get is T variable) {
                return variable;
            }
            return FindChildValue<T>(field, defaultValue);
        }

        public T FindChildValue<T>(string field, T defaultValue) {
            var child = FindChild(field);
            return child != null ? child.FindType<T>() : defaultValue;
        }

        public T FindChildValue<T>(string field) {
            var child = FindChild(field);
            return child != null ? child.FindType<T>() : default(T);
        }

        public DataEntry FindChild(string field) {
            foreach (var dataCell in Cells) {
            //for (int i = 0; i < Cells.Length; i++) {
                if (dataCell.Value is DataList cell) {
                    for (int c = 0; c < cell.Count; c++) {
                        if (cell[c].ID == field) {
                            return cell[c];
                        }
                    }
                }
            }
            return null;
        }

        public T FindType<T>() {
            foreach (var dataCell in Cells) {
            //for (int i = 0; i < Cells.Length; i++) {
                //var cell = Cells[i];
                var cell = dataCell.Value;
                if (cell is DataCell<T> typeCell) {
                    return typeCell.Value;
                }
                if (cell.Get is T variable) {
                    return variable;
                }
            }
            return default(T);
        }

        public bool HasType<T>() {
            foreach (var dataCell in Cells) {
                //for (int i = 0; i < Cells.Length; i++) {
                //var cell = Cells[i];
                var cell = dataCell.Value;
                if (cell is DataCell<T>) {
                    return true;
                }
                if (cell.Get is T) {
                    return true;
                }
            }
            return false;
        }

        public bool TryGetValue<T>(string field, out T value) {
            var cell = Get(field);
            if (cell != null) {
                if (cell is DataCell<T> typeCell) {
                    value = typeCell.Value;
                    return value != null;
                }
                if (cell.Get is T variable) {
                    value = variable;
                    return true;
                }
            }
            var child = FindChild(field);
            if (child != null && child.HasType<T>()) {
                value = child.FindType<T>();
                return true;
            }
            value = default(T);
            return false;
        }
    }

    public class FakeEnum {
        private const string JsonName = "Name";
        private const string JsonDescription = "Description";
        private const string JsonValue = "Value";

        private string[] _shortIDs;
        private string[] _ids;
        private string[] _names;
        private string[] _descriptions;
        private int[] _associatedValues;
        private Dictionary<string, int> _stringToIndex = new Dictionary<string, int>();

        public string TypeName { get; }
        public string[] ShortIDs { get => _shortIDs; }
        public string[] IDs { get => _ids; }
        public string[] Names { get => _names; }
        public string[] Descriptions { get => _descriptions; }
        public int[] AssociatedValues { get => _associatedValues; }
        public int Length { get { return _shortIDs.Length; } }
        public int Count { get { return _shortIDs.Length; } }
        public string this[int index] { get { return _ids[index]; } }

        public string GetNameAt(int index) {
            return _names[index];
        }

        public string GetNameAt(string text) {
            if (_stringToIndex.TryGetValue(text.ToLower(), out var index)) {
                return _names[index];
            }
            return text;
        }

        public string GetDescriptionAt(int index) {
            return _descriptions[index];
        }

        public string GetDescriptionAt(string text) {
            if (_stringToIndex.TryGetValue(text.ToLower(), out var index)) {
                return _descriptions[index];
            }
            return text;
        }

        public string GetID(int index) {
            return _ids[index];
        }

        public string GetShortID(int index) {
            return _shortIDs[index];
        }

        public string GetShortID(string key) {
            if (TryParse(key, out var index)) {
                return _shortIDs[index];
            }
            return "";
        }

        public int GetIndex(string key) {
            return TryParse(key, out var index) ? index : 0;
        }

        public string GetID(string key) {
            if (TryParse(key, out var index)) {
                return _ids[index];
            }
            return "";
        }

        public int GetAssociatedValue(int index) {
            return _associatedValues[index];
        }

        public int GetAssociatedValue(string key) {
            if (TryParse(key, out var index)) {
                return _associatedValues[index];
            }
            return 1;
        }

        public FakeEnum(SheetNode sheet) {
            TypeName = sheet.Name;
            SetupArrays(sheet.Rows.Count);
            for (int r = 0; r < sheet.Rows.Count; r++) {
                JToken line = sheet.Rows[r];
                AddNode(line, r);
            }
        }

        public FakeEnum(string name, int count) {
            TypeName = name;
            SetupArrays(count);
        }

        public void AddNode(JToken line, int index) {
            _shortIDs[index] = line[JsonDB.Id].ToString();
            _ids[index] = string.Format("{0}.{1}", TypeName, _shortIDs[index]);
            var name = line[JsonName];
            _names[index] = name != null ? name.ToString() : _shortIDs[index];
            var description = line[JsonDescription];
            _descriptions[index] = description != null && !string.IsNullOrEmpty(description.ToString()) ? description.ToString(): _shortIDs[index];
            var value = line[JsonValue];
            _associatedValues[index] = value != null ? value.Value<int>() : index;
            _stringToIndex.AddOrUpdate(_shortIDs[index].ToLower(), index);
            _stringToIndex.AddOrUpdate(_names[index].ToLower(), index);
            _stringToIndex.AddOrUpdate(_ids[index].ToLower(), index);
        }

        public void AddNode(int index, string id, string name, string description, int value) {
            _shortIDs[index] = id;
            _ids[index] = string.Format("{0}.{1}", TypeName, _shortIDs[index]);
            _names[index] = name != null ? name.ToString() : _shortIDs[index];
            _descriptions[index] = description != null && !string.IsNullOrEmpty(description) ? description.ToString() : _shortIDs[index];
            _associatedValues[index] = value;
            _stringToIndex.AddOrUpdate(_shortIDs[index].ToLower(), index);
            _stringToIndex.AddOrUpdate(_names[index].ToLower(), index);
            _stringToIndex.AddOrUpdate(_ids[index].ToLower(), index);
        }

        public void AddNode(int index, string id) {
            _shortIDs[index] = id;
            _ids[index] = string.Format("{0}.{1}", TypeName, _shortIDs[index]);
            _names[index] = id;
            _descriptions[index] = id;
            _associatedValues[index] = 0;
            _stringToIndex.AddOrUpdate(_shortIDs[index].ToLower(), index);
            _stringToIndex.AddOrUpdate(_names[index].ToLower(), index);
            _stringToIndex.AddOrUpdate(_ids[index].ToLower(), index);
        }

        private void SetupArrays(int cnt) {
            _shortIDs = new string[cnt];
            _ids = new string[cnt];
            _names = new string[cnt];
            _descriptions = new string[cnt];
            _associatedValues = new int[cnt];
        }

        public bool TryParse(string text, out int index) {
            return _stringToIndex.TryGetValue(text.ToLower(), out index);
        }
    }
}
