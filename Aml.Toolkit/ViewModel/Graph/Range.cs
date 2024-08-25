// Copyright (c) 2017 AutomationML e.V.
using System;

namespace Aml.Toolkit.ViewModel.Graph;

/// <summary>
///     generic class to define a value range
/// </summary>
/// <typeparam name="T"></typeparam>
public class Range<T> where T : IComparable
{
    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="Range{T}" /> class.
    /// </summary>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    public Range(T min, T max)
    {
        Min = min;
        Max = max;
        MinOffset = 0;
        MaxOffset = 0;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    ///     Determines whether the specified range overlapps with this instance.
    /// </summary>
    /// <param name="otherRange">The other range.</param>
    /// <returns>
    ///     <c>true</c> if the specified other range overlapps; otherwise, <c>false</c>.
    /// </returns>
    public bool IsOverlapped(Range<T> otherRange)
    {
        // <0 => Min < Max
        // >0 => Min > Max
        return Min.CompareTo(otherRange.Max) < 0
&& Max.CompareTo(otherRange.Min) > 0 && (Min.CompareTo(otherRange.Min) < 0 || Max.CompareTo(otherRange.Max) > 0);
    }

    #endregion Public Methods

    #region Private Fields

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    ///     The maximum value.
    /// </summary>
    /// <value>
    ///     The maximum.
    /// </value>
    public T Max { get; set; }

    /// <summary>
    ///     Gets or sets the maximum offset.
    /// </summary>
    /// <value>
    ///     The maximum offset.
    /// </value>
    public double MaxOffset { get; set; }

    /// <summary>
    ///     the minimum value.
    /// </summary>
    /// <value>
    ///     The minimum.
    /// </value>
    public T Min { get; set; }

    /// <summary>
    ///     Gets or sets the minimum offset.
    /// </summary>
    /// <value>
    ///     The minimum offset.
    /// </value>
    public double MinOffset { get; set; }

    #endregion Public Properties
}