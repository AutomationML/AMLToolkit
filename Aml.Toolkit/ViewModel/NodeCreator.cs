using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Aml.Toolkit.ViewModel
{
    /// <summary>
    ///     Delegate NodeCreator used to create nodes for the tree view
    /// </summary>
    /// <param name="args">
    ///     The arguments.
    /// </param>
    /// <returns>
    ///     T.
    /// </returns>
    public delegate AMLNodeViewModel NodeCreatorDelegate(params object[] args);

    /// <summary>
    ///     Class NodeCreator is used to compile the constructor into a compiled lambda for
    ///     any constructor information of a node type
    /// </summary>
    public class NodeCreator
    {
        #region Public Methods

        /// <summary>
        ///     Get a NodeCreator Instance for the provided Constructor Information. The
        ///     NodeCreator contains a compiled delegate (lambda) for the creation of new
        ///     nodes with the constructor.
        /// </summary>
        /// <param name="ctor">
        ///     The ctor.
        /// </param>
        /// <returns>
        ///     NodeCreator.
        /// </returns>
        public static NodeCreator GetCreator(ConstructorInfo ctor)
        {
            var paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            var param =
                Expression.Parameter(typeof(object[]), "args");

            var argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array
            //and create a typed expression of them
            for (var i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            var newExp = Expression.New(ctor, argsExp);

            //create a lambda for the delegate with the New
            //Expression as body and our param object[] as arg
            var lambda =
                Expression.Lambda(typeof(NodeCreatorDelegate), newExp, param);

            return new NodeCreator
            {
                Creator = (NodeCreatorDelegate)lambda.Compile(),
                DeclaringType = ctor.DeclaringType,
                ParameterCount = argsExp.Length
            };
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Gets or sets the creator delegate.
        /// </summary>
        /// <value>
        /// The creator.
        /// </value>
        public NodeCreatorDelegate Creator { get; set; }

        /// <summary>
        /// Gets or sets the type to instantiate.
        /// </summary>
        public Type DeclaringType { get; set; }

        /// <summary>
        /// Gets or sets the parameter count, used by the constructor delegate.
        /// </summary>
        /// <value>
        /// The parameter count.
        /// </value>
        public int ParameterCount { get; set; }

        #endregion Public Properties
    }
}