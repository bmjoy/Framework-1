﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelComrades {
    public static class MonoBehaviourToEntity {

        private static List<EntityIdentifier> _tempIdentifiers = new List<EntityIdentifier>(25);
        private static List<Collider> _tempColliders = new List<Collider>(25);

        private static Dictionary<Collider, int> _colliderToDictionary = new Dictionary<Collider, int>();

        public static int GetEntityId(Collider collider) {
            return _colliderToDictionary.TryGetValue(collider, out var id) ? id : -1;
        }

        public static Entity GetEntity(Collider collider) {
            return _colliderToDictionary.TryGetValue(collider, out var id) ? EntityController.GetEntity(id) : null;
        }

        public static void RegisterToEntity(GameObject go, int entId) {
            go.GetComponentsInChildren(_tempIdentifiers);
            for (int i = 0; i < _tempIdentifiers.Count; i++) {
                if (_tempIdentifiers[i] == null) {
                    continue;
                }
                _tempIdentifiers[i].EntityID = entId;
            }
            go.GetComponentsInChildren(_tempColliders);
            for (int i = 0; i < _tempColliders.Count; i++) {
                if (_tempColliders[i] == null) {
                    continue;
                }
                if (_colliderToDictionary.ContainsKey(_tempColliders[i])) {
                    continue;
                }
                _colliderToDictionary.Add(_tempColliders[i], entId);
            }
        }

        public static void Unregister(Entity entity) {
            var tr = entity.Tr;
            if (tr == null) {
                return;
            }
            tr.gameObject.GetComponentsInChildren(_tempColliders);
            for (int i = 0; i < _tempColliders.Count; i++) {
                if (_tempColliders[i] == null) {
                    continue;
                }
                _colliderToDictionary.Remove(_tempColliders[i]);
            }
            ItemPool.Despawn(tr.gameObject);
        }
    }

    public static class UnityToEntityBridge {
        public static void LoadUnityData(Entity entity, DataEntry data) {

            if (data.TryGetValue<string>(DatabaseFields.Model, out var model)) {
                
            }
        }
    }
}
