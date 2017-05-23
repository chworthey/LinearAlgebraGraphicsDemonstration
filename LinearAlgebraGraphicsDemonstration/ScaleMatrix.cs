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
    /// Represents a stackable matrix that is a scale transformation
    /// </summary>
    class ScaleMatrix : StackableMatrix
    {
        Vector3 targetScale;
        Vector3 scale;

        public ScaleMatrix(Vector3 scale, ContentManager content, GraphicsDevice device, int matrixSlot)
            : base(Matrix.Identity, content, "Scale", device, matrixSlot)
        {
            this.scale = Vector3.One;
            targetScale = scale;
        }

        /// <summary>
        /// Updates the animation for the scale matrix
        /// </summary>
        /// <param name="gameTime">The time</param>
        public override void Update(GameTime gameTime)
        {
            scale = Vector3.Lerp(scale, targetScale, (float)gameTime.ElapsedGameTime.TotalSeconds);
            Value = Matrix.CreateScale(scale);

            base.Update(gameTime);
        }
    }
}
