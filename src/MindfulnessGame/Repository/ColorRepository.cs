using System;
using System.Collections.Generic;
using System.Windows.Media;
using MindfulnessGame.Repository.Interface;

namespace MindfulnessGame.Repository
{
    public class ColorRepository : IColorRepository
    {
        private readonly Random _random;

        public readonly List<Brush> ColorCollection = new List<Brush>
        {
            Brushes.Lime,
            Brushes.Green,
            Brushes.Yellow,
            Brushes.Orange,
            Brushes.Red,
            Brushes.Pink,
            Brushes.Purple,
            Brushes.Gray,
            Brushes.Blue,
            Brushes.Black
        };

        private int _lastIndex;

        public ColorRepository()
        {
            _random = new Random();
        }

        public Brush GetRandomColor()
        {
            var index = _random.Next(ColorCollection.Count);
            if (index == _lastIndex)
                index++;

            if (index == ColorCollection.Count)
                index = 0;

            _lastIndex = index;
            return ColorCollection[index];
        }
    }
}