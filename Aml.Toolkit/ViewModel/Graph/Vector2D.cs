// Copyright (c) 2017 AutomationML e.V.
using System;
using System.Windows;

namespace Aml.Toolkit.ViewModel.Graph;

internal class Vector2D
{
    #region Private Fields

    private const double Epsilon = 1e-10;

    #endregion Private Fields

    #region Internal Properties

    internal Point P => new(X, Y);

    #endregion Internal Properties

    #region Internal Fields

    internal double X;
    internal double Y;

    #endregion Internal Fields

    #region Internal Constructors

    // Constructors.
    internal Vector2D(Point p)
    {
        X = p.X;
        Y = p.Y;
    }

    internal Vector2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    internal Vector2D()
        : this(double.NaN, double.NaN)
    {
    }

    #endregion Internal Constructors

    #region Public Methods

    public static Vector2D operator -(Vector2D v, Vector2D w) => new(v.X - w.X, v.Y - w.Y);

    public static double operator *(Vector2D v, Vector2D w) => (v.X * w.X) + (v.Y * w.Y);

    public static Vector2D operator *(Vector2D v, double mult) => new(v.X * mult, v.Y * mult);

    public static Vector2D operator *(double mult, Vector2D v) => new(v.X * mult, v.Y * mult);

    public static Vector2D operator +(Vector2D v, Vector2D w) => new(v.X + w.X, v.Y + w.Y);

    public override bool Equals(object obj) => obj is Vector2D v && IsZero(X - v.X) && IsZero(Y - v.Y);

    public override int GetHashCode() => base.GetHashCode(); // X.GetHashCode() ^ Y.GetHashCode();

    #endregion Public Methods

    #region Internal Methods

    internal static bool IsZero(double d) => Math.Abs(d) < Epsilon;

    internal double Cross(Vector2D v) => (X * v.Y) - (Y * v.X);

    #endregion Internal Methods
}