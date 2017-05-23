using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace LinearAlgebraGraphicsDemonstration
{
    /// <summary>
    /// Represents the camera's projection in stackable form
    /// </summary>
    class ProjectionMatrix : StackableMatrix
    {
        public ProjectionMatrix(Matrix initialValue, ContentManager content, GraphicsDevice device, int matrixSlot)
            : base(initialValue, content, "Projection", device, matrixSlot)
        {

        }

        /// <summary>
        /// Sets the matrix value
        /// </summary>
        /// <param name="value">The value</param>
        public void UpdateProj(Matrix value)
        {
            Value = value;
        }
    }
}
