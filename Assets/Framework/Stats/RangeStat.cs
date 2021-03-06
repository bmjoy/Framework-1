﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelComrades {
    public class RangeStat : BaseStat {

        private float _max;

        public RangeStat(string label,FloatRange range) : base(label, range.Min) {
            _max = (range.Max - range.Min);
        }

        public RangeStat(string label, string id, float rangeMin, float rangeMax) : base(label, id, rangeMin) {
            _max = rangeMax - rangeMin;
        }

        public float UpperRange { get { return base.Value + _max; } }
        public override float Value { get { return Game.Random.NextFloat(base.Value, base.Value + _max); } }
        public float Min { get { return base.Value; } }

        public void SetValue(float rangeMin, float rangeMax) {
            ChangeBase(rangeMin);
            _max = rangeMax < rangeMin ? rangeMax : (rangeMax - rangeMin);
        }

        public void SetValue(float increase) {
            ChangeBase(BaseValue + increase);
            _max += increase;
        }

        public override void AddToBase(float amountToAdd) {
            _baseValue += amountToAdd;
            _baseValue = Mathf.Clamp(_baseValue, -MaxBaseValue, MaxBaseValue);
            _max += amountToAdd;
            StatChanged();
        }

        public override string ToString() {
            return string.Format("{0}: {1:F0}-{2:F0}", Label, base.Value, base.Value + _max);
        }

        public override string ToLabelString() {
            return Label.ToBoldLabel(string.Format("{0:F0}-{1:F0}", base.Value, base.Value + _max));
        }
    }
}
