﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelComrades {
    public class TweenAnimatorRotation : TweenAnimator {

        [SerializeField] private TweenQuaternion _tweener = new TweenQuaternion();
        [SerializeField] private Vector3[] _targets = new Vector3[0];
        [SerializeField] private bool _local = true;
        [SerializeField] private EasingTypes[] _easing = new EasingTypes[0];
        [SerializeField] private float[] _durations = new float[0];

        private int _index = -1;

        public override Tweener Tween { get { return _tweener; } }
        public override void StartTween() {
            _index++;
            if (_index >= _targets.Length) {
                _index = 0;
            }
            var source = _local ? Target.localRotation : Target.rotation;
            if (_easing.Length > _index) {
                _tweener.EasingConfig = _easing[_index];
            }
            _tweener.UnScaled = UnScaled;
            _tweener.Restart(source, Quaternion.Euler(_targets[_index]), _durations.Length > _index ? _durations[_index] : _tweener.Length);
        }

        public override void UpdateTween() {
            if (_local) {
                Target.localRotation = _tweener.Get();
            }
            else {
                Target.rotation = _tweener.Get();
            }
        }
    }
}
