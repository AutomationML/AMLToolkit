﻿// Copyright (c) 2017 AutomationML e.V.

//----------------------------------------------------------------------------
//
// Description: Implementation of an Collection<T> implementing INotifyCollectionChanged
//              to notify listeners of dynamic changes of the list. In addition these
//              notifications can be postponed or disabled.
//
// See spec at  http://avalon/connecteddata/Specs/Collection%20Interfaces.mht
//
//  Author:     Eugene Sadovoi
//
// History:
//  09/1/2011 : [....] - created
//
//---------------------------------------------------------------------------

#if !SILVERLIGHT && !NETSTANDARD1_0 && !NETSTANDARD1_4 && !NETSTANDARD1_6 && !NETSTANDARD2_0
#define FRAMEWORK
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Aml.Toolkit.Tools;

/// <summary>
///     Observable collection with ability to delay or suspend CollectionChanged notifications
/// </summary>
/// <typeparam name="T"></typeparam>
#if FRAMEWORK
[Serializable]
#endif
public sealed class ObservableCollectionEx<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged,
    IDisposable
{
    //-----------------------------------------------------
    //  Constants
    //-----------------------------------------------------

    #region Constants

    private const string _countString = "Count";

    // This must agree with Binding.IndexerName.  It is declared separately
    // here so as to avoid a dependency on PresentationFramework.dll.
    private const string _indexerName = "Item[]";

    /// <summary>
    ///     Empty delegate used to initialize <see cref="CollectionChanged" /> event if it is empty
    /// </summary>
#if FRAMEWORK
    [field: NonSerialized]
#endif
    private static readonly NotifyCollectionChangedEventHandler EmptyDelegate = delegate { };

    #endregion Constants

    //-----------------------------------------------------
    //  Private Fields
    //-----------------------------------------------------

    #region Private Fields

    /// <summary>
    /// </summary>
#if FRAMEWORK
    [field: NonSerialized]
#endif
    private readonly ReentryMonitor _monitor = new();

    /// <summary>
    ///     Placeholder for all data related to delayed
    ///     notifications.
    /// </summary>
#if FRAMEWORK
    [field: NonSerialized]
#endif
    private readonly NotificationInfo _notifyInfo;

    /// <summary>
    ///     Indicates if modification of container allowed during change notification.
    /// </summary>
#if FRAMEWORK
    [field: NonSerialized]
#endif
    private bool _disableReentry;

#if FRAMEWORK

    [field: NonSerialized]
    private
#endif
        Action FireCountAndIndexerChanged = delegate { };

#if FRAMEWORK

    [field: NonSerialized]
    private
#endif
        Action FireIndexerChanged = delegate { };

    #endregion Private Fields

    //-----------------------------------------------------
    //  Protected Fields
    //-----------------------------------------------------

    #region Protected Fields

    /// <summary>
    ///     PropertyChanged event <see cref="INotifyPropertyChanged" />.
    /// </summary>
#if FRAMEWORK
    [field: NonSerialized]
#endif
    private event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    ///     Occurs when the collection changes, either by adding or removing an item.
    /// </summary>
    /// <remarks>See <seealso cref="INotifyCollectionChanged" /></remarks>
#if FRAMEWORK
    [field: NonSerialized]
#endif
    private event NotifyCollectionChangedEventHandler CollectionChanged = EmptyDelegate;

    #endregion Protected Fields

    //-----------------------------------------------------
    //  Constructors
    //-----------------------------------------------------

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of ObservableCollectionEx that is empty and has default initial capacity.
    /// </summary>
    public ObservableCollectionEx()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the ObservableCollectionEx class
    ///     that contains elements copied from the specified list
    /// </summary>
    /// <param name="list">The list whose elements are copied to the new list.</param>
    /// <remarks>
    ///     The elements are copied onto the ObservableCollectionEx in the
    ///     same order they are read by the enumerator of the list.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> list is a null reference </exception>
    public ObservableCollectionEx(List<T> list)
        : base((list != null ? new List<T>(list.Count) : null) ?? throw new InvalidOperationException())
    {
        foreach (var item in list)
        {
            Items.Add(item);
        }
    }

    /// <summary>
    ///     Initializes a new instance of the ObservableCollection class that contains
    ///     elements copied from the specified collection and has sufficient capacity
    ///     to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    /// <remarks>
    ///     The elements are copied onto the ObservableCollection in the
    ///     same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
    public ObservableCollectionEx(IEnumerable<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        using var enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Items.Add(enumerator.Current);
        }
    }

    /// <summary>
    ///     Constructor that configures the container to delay or disable notifications.
    /// </summary>
    /// <param name="parent">Reference to an original collection whos events are being postponed</param>
    /// <param name="notify">Specifies if notifications needs to be delayed or disabled</param>
    public ObservableCollectionEx(ObservableCollectionEx<T> parent, bool notify)
        : base(parent.Items)
    {
        _notifyInfo = new NotificationInfo
        {
            RootCollection = parent
        };

        if (notify)
        {
            CollectionChanged = _notifyInfo.Initialize();
        }
    }

    /// <summary>
    ///     Distructor
    /// </summary>
    ~ObservableCollectionEx()
    {
        Dispose(false);
    }

    #endregion Constructors

    //------------------------------------------------------
    //  Public Events
    //------------------------------------------------------

    #region Public Events

    #region INotifyPropertyChanged implementation

    /// <summary>
    ///     PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
    /// </summary>
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add
        {
            if (null == _notifyInfo)
            {
                if (null == PropertyChanged)
                {
                    FireCountAndIndexerChanged = delegate
                    {
                        OnPropertyChanged(new PropertyChangedEventArgs(_countString));
                        OnPropertyChanged(new PropertyChangedEventArgs(_indexerName));
                    };
                    FireIndexerChanged = delegate { OnPropertyChanged(new PropertyChangedEventArgs(_indexerName)); };
                }

                PropertyChanged += value;
            }
            else
            {
                _notifyInfo.RootCollection.PropertyChanged += value;
            }
        }

        remove
        {
            if (null == _notifyInfo)
            {
                PropertyChanged -= value;

                if (null != PropertyChanged)
                {
                    return;
                }

                FireCountAndIndexerChanged = delegate { };
                FireIndexerChanged = delegate { };
            }
            else
            {
                _notifyInfo.RootCollection.PropertyChanged -= value;
            }
        }
    }

    #endregion INotifyPropertyChanged implementation

    #region INotifyCollectionChanged implementation

    /// <summary>
    ///     Occurs when the collection changes, either by adding or removing an item.
    /// </summary>
    /// <remarks>
    ///     see <seealso cref="INotifyCollectionChanged" />
    /// </remarks>
    event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
    {
        add
        {
            if (null == _notifyInfo)
            {
                // Remove ballast delegate if necessary
                if (1 == CollectionChanged.GetInvocationList().Length)
                {
                    CollectionChanged -= EmptyDelegate;
                }

                CollectionChanged += value;
                _disableReentry = CollectionChanged.GetInvocationList().Length > 1;
            }
            else
            {
                _notifyInfo.RootCollection.CollectionChanged += value;
            }
        }

        remove
        {
            if (null == _notifyInfo)
            {
                CollectionChanged -= value;

                if (null == CollectionChanged || 0 == CollectionChanged.GetInvocationList().Length)
                {
                    CollectionChanged += EmptyDelegate;
                }

                _disableReentry = CollectionChanged.GetInvocationList().Length > 1;
            }
            else
            {
                _notifyInfo.RootCollection.CollectionChanged -= value;
            }
        }
    }

    #endregion INotifyCollectionChanged implementation

    #endregion Public Events

    //------------------------------------------------------
    //  Public Methods
    //-----------------------------------------------------

    #region Public Methods

#if !SILVERLIGHT

    /// <summary>
    ///     Move item at oldIndex to newIndex.
    /// </summary>
    public void Move(int oldIndex, int newIndex)
    {
        MoveItem(oldIndex, newIndex);
    }

#endif

    /// <summary>
    ///     Returns an instance of ObservableCollectionEx
    ///     class which manipulates original collection but suppresses notifications
    ///     untill this instance has been released and Dispose() method has been called.
    ///     To supress notifications it is recommended to use this instance inside
    ///     using() statement:
    ///     <code>
    ///         using (var iSuppressed = collection.DelayNotifications())
    ///         {
    ///             iSuppressed.Add(x);
    ///             iSuppressed.Add(y);
    ///             iSuppressed.Add(z);
    ///         }
    /// </code>
    ///     Each delayed interface is bound to only one type of operation such as Add, Remove, etc.
    ///     Different types of operation on the same delayed interface are not allowed. In order to
    ///     do other type of opertaion you can allocate another wrapper by calling .DelayNotifications() on
    ///     either original object or any delayed instances.
    /// </summary>
    /// <returns>ObservableCollectionEx</returns>
    public ObservableCollectionEx<T> DelayNotifications() =>
        new(null == _notifyInfo ? this : _notifyInfo.RootCollection, true);

    /// <summary>
    ///     Returns a wrapper instance of an ObservableCollectionEx class.
    ///     Calling methods of this instance will modify original collection
    ///     but will not generate any notifications.
    /// </summary>
    /// <returns>ObservableCollectionEx</returns>
    public ObservableCollectionEx<T> DisableNotifications() =>
        new(null == _notifyInfo ? this : _notifyInfo.RootCollection, false);

    #endregion Public Methods

    //-----------------------------------------------------
    //  Protected Methods
    //-----------------------------------------------------

    #region Protected Methods

    /// <summary>
    ///     Called by base class Collection&lt;T&gt; when the list is being cleared;
    ///     raises a CollectionChanged event to any listeners.
    /// </summary>
    protected override void ClearItems()
    {
        CheckReentrancy();

        base.ClearItems();

        FireCountAndIndexerChanged();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    ///     Called by base class Collection&lt;T&gt; when an item is removed from list;
    ///     raises a CollectionChanged event to any listeners.
    /// </summary>
    protected override void RemoveItem(int index)
    {
        CheckReentrancy();
        var removedItem = this[index];

        base.RemoveItem(index);

        FireCountAndIndexerChanged();
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
    }

    /// <summary>
    ///     Called by base class Collection&lt;T&gt; when an item is added to list;
    ///     raises a CollectionChanged event to any listeners.
    /// </summary>
    protected override void InsertItem(int index, T item)
    {
        CheckReentrancy();

        base.InsertItem(index, item);

        FireCountAndIndexerChanged();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
    }

    /// <summary>
    ///     Called by base class Collection&lt;T&gt; when an item is set in list;
    ///     raises a CollectionChanged event to any listeners.
    /// </summary>
    protected override void SetItem(int index, T item)
    {
        CheckReentrancy();

        var originalItem = this[index];
        base.SetItem(index, item);

        FireIndexerChanged();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
            originalItem, item, index));
    }

#if !SILVERLIGHT

    /// <summary>
    ///     Called by base class ObservableCollection&lt;T&gt; when an item is to be moved within the list;
    ///     raises a CollectionChanged event to any listeners.
    /// </summary>
    private void MoveItem(int oldIndex, int newIndex)
    {
        CheckReentrancy();

        var removedItem = this[oldIndex];
        base.RemoveItem(oldIndex);
        base.InsertItem(newIndex, removedItem);

        FireIndexerChanged();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, removedItem,
            newIndex, oldIndex));
    }

#endif

    /// <summary>
    ///     Raises a PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
    /// </summary>
    private void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }

    /// <summary>
    ///     Raise CollectionChanged event to any listeners.
    ///     Properties/methods modifying this ObservableCollection will raise
    ///     a collection changed event through this virtual method.
    /// </summary>
    /// <remarks>
    ///     When overriding this method, either call its base implementation
    ///     or call <see cref="BlockReentrancy" /> to guard against reentrant collection changes.
    /// </remarks>
    private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        using (BlockReentrancy())
        {
            CollectionChanged(this, e);
        }
    }

    /// <summary>
    ///     Disallow reentrant attempts to change this collection. E.g. a event handler
    ///     of the CollectionChanged event is not allowed to make changes to this collection.
    /// </summary>
    /// <remarks>
    ///     typical usage is to wrap e.g. a OnCollectionChanged call with a using() scope:
    ///     <code>
    ///         using (BlockReentrancy())
    ///         {
    ///             CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
    ///         }
    /// </code>
    /// </remarks>
    private IDisposable BlockReentrancy() => _monitor.Enter();

    /// <summary> Check and assert for reentrant attempts to change this collection. </summary>
    /// <exception cref="InvalidOperationException">
    ///     raised when changing the collection
    ///     while another collection change is still being notified to other listeners
    /// </exception>
    private void CheckReentrancy()
    {
        // we can allow changes if there's only one listener - the problem
        // only arises if reentrant changes make the original event args
        // invalid for later listeners.  This keeps existing code working
        // (e.g. Selector.SelectedItems).
        if (_disableReentry && _monitor.IsNotifying)
        {
            throw new InvalidOperationException("ObservableCollectionEx Reentrancy Not Allowed");
        }
    }

    #endregion Protected Methods

    //-----------------------------------------------------
    //  IDisposable
    //------------------------------------------------------

    #region IDisposable

    /// <summary>
    ///     Called by the application code to fire all delayed notifications.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Fires notification with all accumulated events
    /// </summary>
    /// <param name="reason">True is called by App code. False if called from GC.</param>
    private void Dispose(bool reason)
    {
        // Fire delayed notifications
        if (null == _notifyInfo)
        {
            return;
        }

        if (!_notifyInfo.HasEventArgs)
        {
            return;
        }

        if (null != _notifyInfo.RootCollection.PropertyChanged)
        {
            if (_notifyInfo.IsCountChanged)
            {
                _notifyInfo.RootCollection.OnPropertyChanged(new PropertyChangedEventArgs(_countString));
            }

            _notifyInfo.RootCollection.OnPropertyChanged(new PropertyChangedEventArgs(_indexerName));
        }

        using (_notifyInfo.RootCollection.BlockReentrancy())
        {
            var args = _notifyInfo.EventArgs;

            foreach (var delegateItem in _notifyInfo.RootCollection.CollectionChanged.GetInvocationList())
            {
#if FRAMEWORK
                try
                {
#endif
                    delegateItem.DynamicInvoke(_notifyInfo.RootCollection, args);
#if FRAMEWORK
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException is NotSupportedException &&
                        delegateItem.Target is ICollectionView view)
                    {
                        view.Refresh();
                    }
                    else
                    {
                        throw;
                    }
                }
#endif
            }
        }

        // Reset and reuse if necessary
        CollectionChanged = _notifyInfo.Initialize();
    }

    #endregion IDisposable

    //-----------------------------------------------------
    //  Private Types
    //------------------------------------------------------

    #region Private Types

#if FRAMEWORK

    [Serializable]
#endif
    private class ReentryMonitor : IDisposable
    {
        #region Fields

        private int _referenceCount;

        #endregion Fields

        #region Methods

        public IDisposable Enter()
        {
            ++_referenceCount;

            return this;
        }

        public void Dispose()
        {
            --_referenceCount;
        }

        public bool IsNotifying => _referenceCount != 0;

        #endregion Methods
    }

    private class NotificationInfo
    {
        #region Methods

        public NotifyCollectionChangedEventHandler Initialize()
        {
            _action = null;
            _newItems = null;
            _oldItems = null;

            return (sender, args) =>
            {
                var wrapper = sender as ObservableCollectionEx<T>;
                Debug.Assert(null != wrapper, "Calling object must be ObservableCollectionEx<T>");
                Debug.Assert(null != wrapper._notifyInfo, "Calling object must be Delayed wrapper.");

                // Setup
                _action = args.Action;

                if (wrapper == null)
                {
                    return;
                }

                switch (_action)
                {
                    case NotifyCollectionChangedAction.Add:
                        _newItems = new List<T>();
                        IsCountChanged = true;
                        wrapper.CollectionChanged = (s, e) =>
                        {
                            AssertActionType(e);
                            foreach (T item in e.NewItems)
                            {
                                _newItems.Add(item);
                            }
                        };
                        wrapper.CollectionChanged(sender, args);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        _oldItems = new List<T>();
                        IsCountChanged = true;
                        wrapper.CollectionChanged = (s, e) =>
                        {
                            AssertActionType(e);
                            foreach (T item in e.OldItems)
                            {
                                _oldItems.Add(item);
                            }
                        };
                        wrapper.CollectionChanged(sender, args);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        _newItems = new List<T>();
                        _oldItems = new List<T>();
                        wrapper.CollectionChanged = (s, e) =>
                        {
                            AssertActionType(e);
                            foreach (T item in e.NewItems)
                            {
                                _newItems.Add(item);
                            }

                            foreach (T item in e.OldItems)
                            {
                                _oldItems.Add(item);
                            }
                        };
                        wrapper.CollectionChanged(sender, args);
                        break;
#if !SILVERLIGHT
                    case NotifyCollectionChangedAction.Move:
                        _newIndex = args.NewStartingIndex;
                        _newItems = args.NewItems;
                        _oldIndex = args.OldStartingIndex;
                        _oldItems = args.OldItems;
                        wrapper.CollectionChanged = (s, e) => throw new InvalidOperationException(
                            "Due to design of NotifyCollectionChangedEventArgs combination of multiple Move operations is not possible");
                        break;
#endif
                    case NotifyCollectionChangedAction.Reset:
                        IsCountChanged = true;
                        wrapper.CollectionChanged = (s, e) => { AssertActionType(e); };
                        break;

                    default:
                        break;
                }
            };
        }

        #endregion Methods

        #region Private Helper Methods

        private void AssertActionType(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != _action)
            {
                throw new InvalidOperationException(
                    $"Attempting to perform {e.Action} during {_action}. Mixed actions on the same delayed interface are not allowed.");
            }
        }

        #endregion Private Helper Methods

        #region Fields

        private NotifyCollectionChangedAction? _action;

        private IList _newItems;

        private IList _oldItems;

        private int _newIndex;

        private int _oldIndex;

        #endregion Fields

        #region Properties

        public ObservableCollectionEx<T> RootCollection { get; set; }

        public bool IsCountChanged { get; private set; }

        public NotifyCollectionChangedEventArgs EventArgs
        {
            get
            {
                switch (_action)
                {
                    case NotifyCollectionChangedAction.Reset:
                        return new NotifyCollectionChangedEventArgs(_action.Value);

                    case NotifyCollectionChangedAction.Add:
#if !SILVERLIGHT
                        return new NotifyCollectionChangedEventArgs(_action.Value, _newItems);
#else
                            return new NotifyCollectionChangedEventArgs(_action.Value, _newItems, _newIndex);
#endif

                    case NotifyCollectionChangedAction.Remove:
#if SILVERLIGHT
                            return new NotifyCollectionChangedEventArgs(_action.Value, _oldItems, _oldIndex);
#else
                        return new NotifyCollectionChangedEventArgs(_action.Value, _oldItems);

                    case NotifyCollectionChangedAction.Move:
                        return new NotifyCollectionChangedEventArgs(_action.Value, _oldItems[0], _newIndex,
                            _oldIndex);
#endif
                    case NotifyCollectionChangedAction.Replace:
#if !SILVERLIGHT
                        return new NotifyCollectionChangedEventArgs(_action.Value, _newItems, _oldItems);

#else
                            return new NotifyCollectionChangedEventArgs(_action.Value, _newItems, _newIndex);
#endif

                    default:
                        return new NotifyCollectionChangedEventArgs(_action.Value, _newItems, _oldItems);
                }
            }
        }

        public bool HasEventArgs => _action.HasValue;

        #endregion Properties
    }

    #endregion Private Types
}