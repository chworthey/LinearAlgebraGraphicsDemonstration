using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LinearAlgebraGraphicsDemonstration
{
    /// <summary>
    /// Represents a standard identity matrix
    /// </summary>
    class IdentityMatrix : StackableMatrix
    {
        public IdentityMatrix(ContentManager content, GraphicsDevice device, int slot)
            : base(Matrix.Identity, content, "Identity", device, slot)
        {

        }
    }
}
