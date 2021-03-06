﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelComrades {
    public abstract class NodeFilter {

        private System.Type[] _requiredTypes;

        public Type[] RequiredTypes { get => _requiredTypes; }

        protected NodeFilter(System.Type[] types) {
            _requiredTypes = types;
        }
        
        public bool TryAdd(Entity entity, Dictionary<System.Type, ComponentReference> references) {
            if (ContainsEntity(entity)) {
                return true;
            }
            for (int i = 0; i < _requiredTypes.Length; i++) {
                if (!references.ContainsKey(_requiredTypes[i])) {
                    return false;
                }
            }
            AddEntity(entity, references);
            return true;
        }

        public void CheckRemove(Entity entity, Dictionary<System.Type, ComponentReference> references) {
            if (!ContainsEntity(entity)) {
                return;
            }
            for (int i = 0; i < _requiredTypes.Length; i++) {
                if (references.ContainsKey(_requiredTypes[i])) {
                    continue;
                }
                RemoveEntity(entity);
                return;
            }
        }

        protected abstract void AddEntity(Entity entity, Dictionary<System.Type, ComponentReference> references);
        public abstract void RemoveEntity(Entity entity);
        public abstract bool ContainsEntity(Entity entity);
    }

    public class NodeFilter<T> : NodeFilter where T : class, INode, new() {

        private Dictionary<int, T> _nodes = new Dictionary<int, T>();
        private List<T> _allNodes = new List<T>();
        private GenericPool<T> _pool = new GenericPool<T>(25, obj => obj.Dispose());

        public List<T> AllNodes { get => _allNodes; }

        public NodeFilter(System.Type[] types) : base(types) {
        }

        public static void New(System.Type[] types) {
            EntityController.RegisterNodeFilter(new NodeFilter<T>(types), typeof(T));
        }

        public override bool ContainsEntity(Entity entity) {
            return _nodes.ContainsKey(entity);
        }

        protected override void AddEntity(Entity entity, Dictionary<System.Type, ComponentReference> references) {
            if (_nodes.ContainsKey(entity)) {
                return;
            }
            T node = _pool.New();
            node.Register(entity, references);
            _allNodes.Add(node);
            _nodes.Add(entity, node);
        }

        public override void RemoveEntity(Entity entity) {
            if (!_nodes.TryGetValue(entity, out var node)) {
                return;
            }
            _allNodes.Remove(node);
            _nodes.Remove(entity);
            _pool.Store(node);
        }

        public T GetNode(Entity entity) {
            return _nodes.TryGetValue(entity, out var node) ? node : null;
        }
    }
}