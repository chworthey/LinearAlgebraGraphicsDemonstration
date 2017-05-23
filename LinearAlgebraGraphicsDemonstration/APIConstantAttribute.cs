using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearAlgebraGraphicsDemonstration
{
    /// <summary>
    /// An attribute to put above any constants in the PublicAPI for documentation purposes
    /// </summary>
    class APIConstantAttribute : Attribute
    {
        /// <summary>
        /// The user-facing description of the constant
        /// </summary>
        public string Description { get; private set; }

        public APIConstantAttribute(string description)
        {
            Description = description;
        }
    }
}
