using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearAlgebraGraphicsDemonstration
{
    /// <summary>
    /// An attribute to put above any methods in the PublicAPI for documentation purposes
    /// </summary>
    class APIMethodAttribute : Attribute
    {
        /// <summary>
        /// The user-facing description of the method
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// An array of the user-facing descriptions for each parameter
        /// </summary>
        public string[] ParameterDescriptions { get; private set; }

        /// <summary>
        /// Constructs a new APIMethodAttribute
        /// </summary>
        /// <param name="description">The user-facing description of the method itself</param>
        /// <param name="parameterDescriptions">A list of descriptions for each parameter of the method.
        /// Must be included for each parameter of the method this attribute is attached to.</param>
        public APIMethodAttribute(string description, params string[] parameterDescriptions)
        {
            Description = description;
            ParameterDescriptions = parameterDescriptions;
        }
    }
}
