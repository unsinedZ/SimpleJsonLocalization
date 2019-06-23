using System.Collections.Generic;

namespace System.Linq
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// The shortcut for the <c>foreach</c> loop that executes operation for each element.
        /// </summary>
        /// <param name="enumeration">The enumeration.</param>
        /// <param name="operation">The operation to apply to enumeration elements.</param>
        /// <typeparam name="T">The type of enumeration elements.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T> operation)
        {
            if (enumeration == null)
                throw new ArgumentNullException(nameof(enumeration));

            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            foreach (T element in enumeration)
                operation(element);

            return enumeration;
        }
    }
}