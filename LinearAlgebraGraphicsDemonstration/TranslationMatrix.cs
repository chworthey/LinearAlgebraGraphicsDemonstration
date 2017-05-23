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
    /// Represents a stackable matrix that is a translation matrix
    /// </summary>
    class TranslationMatrix : StackableMatrix
    {
        Vector3 targetPos;
        Vector3 currentPos;

        public TranslationMatrix(Vector3 translation, ContentManager content, GraphicsDevice device, int matrixSlot)
            : base(Matrix.Identity, content, "Translation", device, matrixSlot)
        {
            targetPos = translation;
            currentPos = Vector3.Zero;
        }

        /// <summary>
        /// Updates the translation matrix animation
        /// </summary>
        /// <param name="gameTime">The time</param>
        public override void Update(GameTime gameTime)
        {
            currentPos = Vector3.Lerp(currentPos, targetPos, (float)gameTime.ElapsedGameTime.TotalSeconds);
            Value = Matrix.CreateTranslation(currentPos);

            base.Update(gameTime);
        }
    }
}
