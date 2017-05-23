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
    /// Represents a user-defined matrix where each element was entered in by the user
    /// </summary>
    class InputMatrix : StackableMatrix
    {
        public InputMatrix(ContentManager content, GraphicsDevice device, int slot, params float[] values)
            : base(Matrix.Identity, content, "User-Defined", device, slot)
        {
            if (values.Length != 16)
                throw new InvalidOperationException("Values array must be of length 16.");

            Matrix m = Matrix.Identity;
            m.M11 = values[0];
            m.M12 = values[1];
            m.M13 = values[2];
            m.M14 = values[3];
            m.M21 = values[4];
            m.M22 = values[5];
            m.M23 = values[6];
            m.M24 = values[7];
            m.M31 = values[8];
            m.M32 = values[9];
            m.M33 = values[10];
            m.M34 = values[11];
            m.M41 = values[12];
            m.M42 = values[13];
            m.M43 = values[14];
            m.M44 = values[15];

            Value = m;
        }
    }
}
