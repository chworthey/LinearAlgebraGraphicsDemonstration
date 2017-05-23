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
    /// Represents the X, Y, or Z axis
    /// </summary>
    public enum RotationMatrixMajorAxis
    {
        X, Y, Z
    }

    /// <summary>
    /// Represents a stackable matrix that is a rotation transformation
    /// </summary>
    class RotationMatrix : StackableMatrix
    {
        float targetRot;
        float currentRot;
        RotationMatrixMajorAxis axis;

        public RotationMatrix(RotationMatrixMajorAxis axis, float rotation, ContentManager content, GraphicsDevice device, int slot)
            : base(Matrix.Identity, content, axis.ToString() + " Rotation", device, slot)
        {
            targetRot = rotation;
            currentRot = 0.0f;
            this.axis = axis;
        }

        /// <summary>
        /// Modifies some of the element strings with cosines and sines instead of simply numbers
        /// </summary>
        /// <returns>The string array of all the elements</returns>
        protected override string[] GetMatrixArray()
        {
            string[] arr = base.GetMatrixArray();

            string rot = currentRot.ToString(numberFormat);

            switch (axis)
            {
                case RotationMatrixMajorAxis.X:
                    arr[getIndex(1, 1)] = "cos(" + rot + ")";
                    arr[getIndex(1, 2)] = "sin(" + rot + ")";
                    arr[getIndex(2, 1)] = "-sin(" + rot + ")";
                    arr[getIndex(2, 2)] = "cos(" + rot + ")";
                    break;
                case RotationMatrixMajorAxis.Y:
                    arr[getIndex(0, 0)] = "cos(" + rot + ")";
                    arr[getIndex(0, 2)] = "-sin(" + rot + ")";
                    arr[getIndex(2, 0)] = "sin(" + rot + ")";
                    arr[getIndex(2, 2)] = "cos(" + rot + ")";
                    break;
                case RotationMatrixMajorAxis.Z:
                    arr[getIndex(0, 0)] = "cos(" + rot + ")";
                    arr[getIndex(0, 1)] = "sin(" + rot + ")";
                    arr[getIndex(1, 0)] = "-sin(" + rot + ")";
                    arr[getIndex(1, 1)] = "cos(" + rot + ")";
                    break;
                default:
                    throw new InvalidOperationException("Axis does not exist.");
            }

            return arr;
        }

        int getIndex(int y, int x)
        {
            return x + y * 4;
        }

        /// <summary>
        /// Animates to the target rotations
        /// </summary>
        /// <param name="gameTime">The time</param>
        public override void Update(GameTime gameTime)
        {
            currentRot = MathHelper.Lerp(currentRot, targetRot, (float)gameTime.ElapsedGameTime.TotalSeconds);

            switch (axis)
            {
                case RotationMatrixMajorAxis.X:
                    Value = Matrix.CreateRotationX(currentRot);
                    break;
                case RotationMatrixMajorAxis.Y:
                    Value = Matrix.CreateRotationY(currentRot);
                    break;
                case RotationMatrixMajorAxis.Z:
                    Value = Matrix.CreateRotationZ(currentRot);
                    break;
                default:
                    throw new InvalidOperationException("Axis does not exist.");
            }

            base.Update(gameTime);
        }
    }
}
