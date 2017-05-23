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
    /// The matrix which represents all of the vertex data for the model. Just sticks a giant 'D' for "Data" there instead of actual vertex data
    /// </summary>
    class DataMatrix : StackableMatrix
    {
        public DataMatrix(ContentManager content, GraphicsDevice device)
            : base(Matrix.Identity, content, "", device, 0)
        {

        }

        /// <summary>
        /// Draws the big ole "D" instead of an actual matrix
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(largeFont2, "D", new Vector2(0, MatrixArea.Height - titleMargin) / 2.0f + 
                new Vector2(MatrixArea.Width - 15, titleMargin + 5) -
                largeFont2.MeasureString("D") / 2.0f + Position, Color.Black * CurrentAlpha);
        }
    }
}
