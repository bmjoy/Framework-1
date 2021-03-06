﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PixelComrades {
    public class StatusContainer : IComponent  {

        public int Owner { get; set; }

        public event System.Action OnResourceChanged;

        private Dictionary<string, CharacterStatus> _keys = new Dictionary<string, CharacterStatus>();
        private bool _preventsMove = false;
        private bool _preventsAction = false;

        public bool PreventsAction { get { return _preventsAction; } }
        public bool PreventsMove { get { return _preventsMove; } }

        public string AddValue(CharacterStatus value) {
            return AddValue(value, System.Guid.NewGuid().ToString());
        }

        public string AddValue(CharacterStatus value, string id) {
            if (_keys.ContainsKey(id)) {
                _keys[id] = value;
                return id;
            }
            _keys.Add(id, value);
            return id;
        }

        public void RemoveAll() {
            _keys.Clear();
            ValuesChanged();
        }

        public bool HasId(string key) {
            return _keys.ContainsKey(key);
        }

        public void RemoveValue(string id) {
            _keys.Remove(id);
            ValuesChanged();
        }

        private void ValuesChanged() {
            _preventsMove = false;
            _preventsAction = false;
            var enumerator = _keys.GetEnumerator();
            try {
                while (enumerator.MoveNext()) {
                    if (enumerator.Current.Value.PreventsMove) {
                        _preventsMove = true;
                    }
                    if (enumerator.Current.Value.PreventsAction) {
                        _preventsAction = true;
                    }
                    if (_preventsAction && _preventsMove) {
                        break;
                    }
                }
            }
            finally {
                enumerator.Dispose();
            }
            OnResourceChanged.SafeInvoke();
        }

        public string DebugString() {
            var sb = new StringBuilder();
            foreach (var vals in _keys) {
                sb.Append(System.Environment.NewLine);
                sb.Append(vals.Key);
                sb.Append(vals.Value.PreventsAction);
                sb.Append(vals.Value.PreventsMove);
            }
            return sb.ToString();
        }

        public void Dispose() {
            OnResourceChanged = null;
            _keys = null;
        }
    }

    public struct CharacterStatus {
        public static readonly CharacterStatus Stunned = new CharacterStatus(true, true);
        public static readonly CharacterStatus Sleep = new CharacterStatus(true, true);
        public static readonly CharacterStatus Paralyzed = new CharacterStatus(true, false);
        public static readonly CharacterStatus Lost = new CharacterStatus(true, true);

        public bool PreventsMove {get;}
        public bool PreventsAction { get; }

        public CharacterStatus(bool preventsMove, bool preventsAction) {
            PreventsMove = preventsMove;
            PreventsAction = preventsAction;
        }
    }
}
