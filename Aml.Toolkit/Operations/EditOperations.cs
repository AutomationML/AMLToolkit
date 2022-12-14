
using Aml.Toolkit.Properties;
using System.Windows.Input;

namespace Aml.Toolkit.Operations
{
    /// <summary>
    /// Class defining standard edit operations cut, paste, delete, copy, redo, undo
    /// </summary>
    /// <seealso cref="ItemOperations" />
    public class EditOperations : ItemOperations
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditOperations"/> class.
        /// </summary>
        public EditOperations()
        {
            StandardOperations = 6;
            AddStandardItems();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the standard edit operations.
        /// </summary>
        /// <value>
        /// The standard operations.
        /// </value>
        public sealed override int StandardOperations { get; set; }

        #endregion Public Properties

        #region Private Methods

        private void AddStandardItems()
        {
            AddOperation(new ItemOperationViewModel
            {
                Identifier = Cut,
                Command = ApplicationCommands.Cut,
                ToolTip = Resources.cutTip,
                Name = Resources.cutTip
            });

            AddOperation(CopyOperation);

            AddOperation(new ItemOperationViewModel
            {
                Identifier = Paste,
                Command = ApplicationCommands.Paste,
                ToolTip = Resources.pasteTip,
                Name = Resources.pasteTip
            });

            AddOperation(new ItemOperationViewModel
            {
                Identifier = Delete,
                Command = ApplicationCommands.Delete,
                ToolTip = Resources.deleteTip,
                Name = Resources.deleteTip
            });

            AddOperation(new ItemOperationViewModel
            {
                Identifier = Undo,
                Command = ApplicationCommands.Undo,
                ToolTip = Resources.undoTip,
                Name = Resources.undoTip
            });

            AddOperation(new ItemOperationViewModel
            {
                Identifier = Redo,
                Command = ApplicationCommands.Redo,
                ToolTip = Resources.redoTip,
                Name = Resources.redoTip
            });
        }

        #endregion Private Methods

        /// <summary>
        /// Gets the copy operation.
        /// </summary>
        public static ItemOperationViewModel CopyOperation { get; } =
            new()
            {
                Identifier = Copy,
                Command = ApplicationCommands.Copy,
                ToolTip = Resources.copyTip,
                Name = Resources.copyTip
            };

        #region Public Fields

        /// <summary>
        /// The copy operation id.
        /// </summary>
        public const int Copy = 101;

        /// <summary>
        /// The cut operation id.
        /// </summary>
        public const int Cut = 100;

        /// <summary>
        /// The delete  operation id.
        /// </summary>
        public const int Delete = 103;

        /// <summary>
        /// The paste operation id.
        /// </summary>
        public const int Paste = 102;

        /// <summary>
        /// The redo operation id.
        /// </summary>
        public const int Redo = 105;

        /// <summary>
        /// The undo operation id.
        /// </summary>
        public const int Undo = 104;

        #endregion Public Fields
    }
}