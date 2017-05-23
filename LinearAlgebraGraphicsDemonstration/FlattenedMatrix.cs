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
    /// Represents a single matrix that is the result of several other matrix multiplications
    /// </summary>
    class FlattenedMatrix : StackableMatrix
    {
        public FlattenedMatrix(Matrix value, ContentManager content, GraphicsDevice device, int matrixSlot)
            : base(value, content, "Flattened", device, matrixSlot)
        {

        }
    }
}
