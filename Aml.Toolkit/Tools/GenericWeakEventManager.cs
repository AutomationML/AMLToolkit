using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Aml.Toolkit.Tools;

/// <summary>
///     class implements a generic weak event manager
/// </summary>
/// <seealso cref="WeakEventManager" />
public sealed class GenericWeakEventManager : WeakEventManager
{
    #region Private Properties

    private static GenericWeakEventManager CurrentManager
    {
        get
        {
            var managerType = typeof(GenericWeakEventManager);
            var manager = (GenericWeakEventManager)GetCurrentManager(managerType);
            if (manager != null)
            {
                return manager;
            }

            manager = new GenericWeakEventManager();
            SetCurrentManager(managerType, manager);

            return manager;
        }
    }

    #endregion Private Properties

    #region Private Classes

    private class WeakEventListenerRecord
    {
        #region Public Constructors

        static WeakEventListenerRecord()
        {
            HandlerMethod = new Lazy<MethodInfo>(
                () => typeof(WeakEventListenerRecord).GetMethod(
                    "HandleEvent",
                    BindingFlags.Instance | BindingFlags.NonPublic));
        }

        #endregion Public Constructors

        #region Internal Constructors

        internal WeakEventListenerRecord(GenericWeakEventManager manager, object source,
            EventDescriptor eventDescriptor)
        {
            _listeners = new ListenerList();
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _source.Target = source ?? throw new ArgumentNullException(nameof(source));
            _eventDescriptor = eventDescriptor ?? throw new ArgumentNullException(nameof(eventDescriptor));

            var eventType = eventDescriptor.EventType;

            _handlerDelegate = Delegate.CreateDelegate(eventType, this, HandlerMethod.Value);

            if (_handlerDelegate == null)
            {
                throw new InvalidOperationException(
                    $"Could not bind weak event listener to event '{eventDescriptor.DisplayName}'.");
            }

            _eventDescriptor.AddEventHandler(source, _handlerDelegate);
        }

        #endregion Internal Constructors

        #region Internal Properties

        internal bool IsEmpty => _listeners.IsEmpty;

        #endregion Internal Properties

        #region Private Methods

        //[UsedImplicitly]
        private void HandleEvent(object sender, EventArgs e)
        {
            using (_manager.ReadLock)
            {
                _ = _listeners.BeginUse();
            }

            try
            {
                _manager.DeliverEventToList(sender, e, _listeners);
            }
            catch
            {
            }
            finally
            {
                _listeners.EndUse();
            }
        }

        #endregion Private Methods

        #region Private Fields

        private static readonly Lazy<MethodInfo> HandlerMethod;

        private readonly EventDescriptor _eventDescriptor;
        private readonly Delegate _handlerDelegate;
        private readonly GenericWeakEventManager _manager;
        private readonly WeakReference _source = new(null);
        private ListenerList _listeners;

        #endregion Private Fields

        #region Internal Methods

        internal void Add(IWeakEventListener listener)
        {
            _ = ListenerList.PrepareForWriting(ref _listeners);
            _listeners.Add(listener);
        }

        internal bool Purge()
        {
            _ = ListenerList.PrepareForWriting(ref _listeners);
            return _listeners.Purge();
        }

        internal void Remove(IWeakEventListener listener)
        {
            if (listener == null)
            {
                return;
            }

            _ = ListenerList.PrepareForWriting(ref _listeners);

            _listeners.Remove(listener);

            if (_listeners.IsEmpty)
            {
                StopListening();
            }
        }

        internal void StopListening()
        {
            var target = _source.Target;
            if (target == null)
            {
                return;
            }

            _source.Target = null;
            _eventDescriptor.RemoveEventHandler(target, _handlerDelegate);
        }

        #endregion Internal Methods
    }

    #endregion Private Classes

    #region Public Methods

    /// <summary>
    ///     Adds the event listener.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="listener">The listener.</param>
    /// <exception cref="ArgumentNullException">
    ///     source
    ///     or
    ///     eventName
    ///     or
    ///     listener
    /// </exception>
    public static void AddListener(object source, string eventName, IWeakEventListener listener)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (eventName == null)
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        CurrentManager.PrivateAddListener(
            source,
            listener,
            FindEvent(source, eventName));
    }

    /// <summary>
    ///     Adds the event listener.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="eventDescriptor">The event descriptor.</param>
    /// <param name="listener">The listener.</param>
    /// <exception cref="ArgumentNullException">
    ///     source
    ///     or
    ///     eventDescriptor
    ///     or
    ///     listener
    /// </exception>
    public static void AddListener(object source, EventDescriptor eventDescriptor, IWeakEventListener listener)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (eventDescriptor == null)
        {
            throw new ArgumentNullException(nameof(eventDescriptor));
        }

        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        CurrentManager.PrivateAddListener(
            source,
            listener,
            eventDescriptor);
    }

    /// <summary>
    ///     Removes the event listener.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="listener">The listener.</param>
    /// <exception cref="ArgumentNullException">
    ///     source
    ///     or
    ///     eventName
    ///     or
    ///     listener
    /// </exception>
    public static void RemoveListener(object source, string eventName, IWeakEventListener listener)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (eventName == null)
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        CurrentManager.PrivateRemoveListener(
            source,
            listener,
            FindEvent(source, eventName));
    }

    /// <summary>
    ///     Removes the event listener.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="eventDescriptor">The event descriptor.</param>
    /// <param name="listener">The listener.</param>
    /// <exception cref="ArgumentNullException">
    ///     source
    ///     or
    ///     eventDescriptor
    ///     or
    ///     listener
    /// </exception>
    public static void RemoveListener(object source, EventDescriptor eventDescriptor, IWeakEventListener listener)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (eventDescriptor == null)
        {
            throw new ArgumentNullException(nameof(eventDescriptor));
        }

        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        CurrentManager.PrivateRemoveListener(
            source,
            listener,
            eventDescriptor);
    }

    /// <summary>
    ///     Removes the event listener.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="listener">The listener.</param>
    /// <param name="eventDescriptor">The event descriptor.</param>
    public static void RemoveListener(object source, IWeakEventListener listener, EventDescriptor eventDescriptor)
    {
        CurrentManager.PrivateRemoveListener(source, listener, eventDescriptor);
    }

    #endregion Public Methods

    #region Protected Methods

    /// <inheritdoc />
    protected override bool Purge(object source, object data, bool purgeAll)
    {
        var removedAnyEntries = false;
        var dictionary = (Dictionary<EventDescriptor, WeakEventListenerRecord>)data;
        var keys = dictionary.Keys.ToList();

        foreach (var key in keys)
        {
            var isEmpty = purgeAll || source == null;

            if (!dictionary.TryGetValue(key, out var record))
            {
                continue;
            }

            if (!isEmpty)
            {
                if (record.Purge())
                {
                    removedAnyEntries = true;
                }

                isEmpty = record.IsEmpty;
            }

            if (!isEmpty)
            {
                continue;
            }

            record.StopListening();

            if (!purgeAll)
            {
                _ = dictionary.Remove(key);
            }
        }

        if (dictionary.Count != 0)
        {
            return removedAnyEntries;
        }

        if (source != null)
        {
            Remove(source);
        }

        return true;
    }

    /// <inheritdoc />
    protected override void StartListening(object source)
    {
    }

    /// <inheritdoc />
    protected override void StopListening(object source)
    {
    }

    #endregion Protected Methods

    #region Private Methods

    private static EventDescriptor FindEvent(object source, string eventName) =>
        TypeDescriptor.GetEvents(source)[eventName];

    private void PrivateAddListener(object source, IWeakEventListener listener, EventDescriptor eventDescriptor)
    {
        using (WriteLock)
        {
            if (base[source] is not Dictionary<EventDescriptor, WeakEventListenerRecord> dictionary)
            {
                dictionary = [];
                base[source] = dictionary;
            }

            if (!dictionary.TryGetValue(eventDescriptor, out var record))
            {
                record = new WeakEventListenerRecord(this, source, eventDescriptor);
                dictionary[eventDescriptor] = record;
            }

            record.Add(listener);

            ScheduleCleanup();
        }
    }

    private void PrivateRemoveListener(object source, IWeakEventListener listener, EventDescriptor eventDescriptor)
    {
        using (WriteLock)
        {
            if (base[source] is not Dictionary<EventDescriptor, WeakEventListenerRecord> dictionary)
            {
                return;
            }

            if (!dictionary.TryGetValue(eventDescriptor, out var record))
            {
                return;
            }

            record.Remove(listener);

            if (record.IsEmpty)
            {
                _ = dictionary.Remove(eventDescriptor);
            }

            if (dictionary.Count == 0)
            {
                Remove(source);
            }
        }
    }

    #endregion Private Methods
}