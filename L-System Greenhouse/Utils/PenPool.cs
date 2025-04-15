using System;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace L_System_Greenhouse.Utils;

public class PenPool
{
    private readonly Dictionary<int, ImmutablePen> _pens;

    public PenPool(int maxThickness)
    {
        _pens = new();

        for (var thickness = 1; thickness <= maxThickness; thickness++)
        {
            var pen = new Pen
            {
                Brush = Brushes.White,
                Thickness = thickness
            }.ToImmutable();

            _pens.Add(thickness, pen.ToImmutable());
        }
    }

    public ImmutablePen GetPen(double thickness)
    {
        return _pens[Math.Max(1, (int)Math.Ceiling(thickness))];
    }
}