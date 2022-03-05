using System;
using System.Windows;

namespace Aml.Toolkit.Tools
{
    /// <summary>
    /// Class implementing a weak event listener
    /// </summary>
    /// <seealso cref="IWeakEventListener" />
    public sealed class DelegatingWeakEventListener : IWeakEventListener
    {
        #region Private Fields

        private readonly Delegate _handler;

        #endregion Private Fields

        #region Public Methods

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            _ = _handler.DynamicInvoke(sender, e);
            return true;
        }

        #endregion Public Methods

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingWeakEventListener"/> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <exception cref="ArgumentNullException">handler</exception>
        public DelegatingWeakEventListener(Delegate handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingWeakEventListener"/> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <exception cref="ArgumentNullException">handler</exception>
        public DelegatingWeakEventListener(EventHandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        #endregion Public Constructors
    }
}