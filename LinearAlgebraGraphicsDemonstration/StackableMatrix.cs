using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LinearAlgebraGraphicsDemonstration
{
    delegate void FunctionCall();

    /// <summary>
    /// Represents a matrix that can be stacked with other matrices
    /// </summary>
    abstract class StackableMatrix
    {
        /// <summary>
        /// The actual value of this matrix
        /// </summary>
        public Matrix Value { get; set; }

        /// <summary>
        /// Gets a rectangle defining the matrix area that we have to work with
        /// </summary>
        public Rectangle MatrixArea
        {
            get
            {
                Rectangle bracket = getBracketRect();
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(bracket.Width * 2 + spacing.X * 4),
                    bracket.Height + 10);
            }
        }

        static Vector2 spacing = new Vector2(75, 20);
        static Vector2 targetSpacing = new Vector2(75, 20);

        /// <summary>
        /// Spacing between the matrix elements
        /// </summary>
        public static Vector2 DefaultSpacing
        {
            get
            {
                return new Vector2(75, 20);
            }
        }
        Vector2 offset = new Vector2(5, 0);

        /// <summary>
        /// Gets invoked when the spacing is changed
        /// </summary>
        public static FunctionCall OnSpacingChanged;

        /// <summary>
        /// Top Left anchor position of the slot
        /// </summary>
        protected Vector2 Position { get; set; } = new Vector2(0, 0);
        
        string typeString;
        int bracketHeight
        {
            get
            {
                return (int)(86.0f / 20.0f * spacing.Y);
            }
        }
        protected const int titleMargin = 15;
        float targetAlpha = 0.4f;
        protected float CurrentAlpha { get; set; } = 0.0f;

        /// <summary>
        /// The slot of space this matrix will occupy (index on the stack array, basically)
        /// </summary>
        public int MatrixSlot
        {
            get
            {
                return matrixSlot;
            }
        }

        /// <summary>
        /// Whether or not this matrix is actually visible
        /// </summary>
        public bool IsHidden
        {
            get
            {
                return Math.Abs(CurrentAlpha) < 0.01f;
            }
        }

        int matrixSlot;

        Vector2 targetPosition;

        SpriteFont smallFont;
        SpriteFont smallFont2;
        SpriteFont largeFont;
        protected SpriteFont largeFont2 { get; set; }

        protected const string numberFormat = "0.00";

        Texture2D squareBracket;
        GraphicsDevice device;

        public StackableMatrix(Matrix value, ContentManager content, string typeString, GraphicsDevice device, int matrixSlot)
        {
            Value = value;

            smallFont = content.Load<SpriteFont>("SmallFont");
            smallFont2 = content.Load<SpriteFont>("SmallFont2");
            largeFont = content.Load<SpriteFont>("LargeFont");
            largeFont2 = content.Load<SpriteFont>("LargeFont2");
            squareBracket = content.Load<Texture2D>("LeftSquareBracket");

            this.typeString = typeString;
            this.device = device;
            MoveToSlot(matrixSlot, true);
        }

        /// <summary>
        /// Begins the fade out animation
        /// </summary>
        public void Hide()
        {
            targetAlpha = 0.0f;
        }

        /// <summary>
        /// Begins an animation to move to the specified slot
        /// </summary>
        /// <param name="slot">Slot index to move to</param>
        /// <returns>Whether or not this matrix is off the screen</returns>
        public bool MoveToSlot(int slot)
        {
            return MoveToSlot(slot, false);
        }

        /// <summary>
        /// Begins an animation to move to the specified slot
        /// </summary>
        /// <param name="slot">Slot index to move to</param>
        /// <param name="initialMove">Do we need to play the construction animation?</param>
        /// <returns>Whether or not this matrix is off the screen</returns>
        protected bool MoveToSlot(int slot, bool initialMove)
        {
            matrixSlot = slot;

            int rowCapacity = device.Viewport.Width / MatrixArea.Width;

            // slots will occupy the screen as words would occupy a page in a book
            targetPosition = offset + new Vector2((matrixSlot % rowCapacity) * (MatrixArea.Width + 3.0f), matrixSlot / rowCapacity * MatrixArea.Height);
            
            // start off the bottom of the screen if this is the initial move
            if (initialMove)
                Position = targetPosition + new Vector2(0.0f, device.Viewport.Height);

            return matrixSlot / rowCapacity * MatrixArea.Height + MatrixArea.Height > device.Viewport.Height;
        }

        /// <summary>
        /// Get matrix elements as a flat string array
        /// </summary>
        /// <returns>The string array</returns>
        protected virtual string[] GetMatrixArray()
        {
            string[] rv = new string[16];

            rv[0] = Value.M11.ToString(numberFormat);
            rv[1] = Value.M12.ToString(numberFormat);
            rv[2] = Value.M13.ToString(numberFormat);
            rv[3] = Value.M14.ToString(numberFormat);
            rv[4] = Value.M21.ToString(numberFormat);
            rv[5] = Value.M22.ToString(numberFormat);
            rv[6] = Value.M23.ToString(numberFormat);
            rv[7] = Value.M24.ToString(numberFormat);
            rv[8] = Value.M31.ToString(numberFormat);
            rv[9] = Value.M32.ToString(numberFormat);
            rv[10] = Value.M33.ToString(numberFormat);
            rv[11] = Value.M34.ToString(numberFormat);
            rv[12] = Value.M41.ToString(numberFormat);
            rv[13] = Value.M42.ToString(numberFormat);
            rv[14] = Value.M43.ToString(numberFormat);
            rv[15] = Value.M44.ToString(numberFormat);

            return rv;
        }

        Rectangle getBracketRect()
        {
            return new Rectangle((int)Position.X, (int)Position.Y + titleMargin,
                (int)((float)squareBracket.Width / squareBracket.Height * (float)bracketHeight), bracketHeight);
        }

        /// <summary>
        /// Updates the animations
        /// </summary>
        /// <param name="gameTime">The time</param>
        public virtual void Update(GameTime gameTime)
        {
            CurrentAlpha = MathHelper.Lerp(CurrentAlpha, targetAlpha, (float)gameTime.ElapsedGameTime.TotalSeconds);
            Position = Vector2.Lerp(Position, targetPosition, (float)gameTime.ElapsedGameTime.TotalSeconds);

            spacing = Vector2.Lerp(spacing, targetSpacing, (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// Draws the * symbol in front of the slot
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        protected void DrawMultiplySymbol(SpriteBatch spriteBatch)
        {
            if (matrixSlot > 0)
                spriteBatch.DrawString(largeFont, ".", new Vector2(Position.X - 2.0f, MatrixArea.Height / 2.0f + Position.Y + 4.0f) -
                    largeFont.MeasureString(".") / 2.0f, Color.Black * CurrentAlpha);
        }

        /// <summary>
        /// Draws the matrix on screen
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            string[] arr = GetMatrixArray();

            Rectangle bracketRect = getBracketRect();

            Vector2 txtPos = new Vector2(bracketRect.Width, titleMargin - 2.0f) + Position;

            // Draw the type of matrix as string above the slot
            spriteBatch.DrawString(smallFont2, typeString, new Vector2(Position.X + MatrixArea.Width / 2.0f, Position.Y) - new Vector2(
                smallFont.MeasureString(typeString).X / 2.0f, 0.0f), Color.Black * CurrentAlpha);

            // Draw square brackets
            spriteBatch.Draw(squareBracket, bracketRect, Color.Black * CurrentAlpha);
            spriteBatch.Draw(squareBracket, new Rectangle((int)(spacing.X * 4) + (int)txtPos.X, (int)Position.Y + titleMargin, bracketRect.Width, bracketRect.Height),
                null, Color.Black * CurrentAlpha, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);

            DrawMultiplySymbol(spriteBatch);

            // Draw matrix elements
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    string str = arr[x + 4 * y];
                    Vector2 measure = smallFont.MeasureString(str);
                    spriteBatch.DrawString(smallFont, str, txtPos + new Vector2(x * spacing.X + spacing.X / 2.0f, y * spacing.Y + spacing.Y / 2.0f) -
                        new Vector2(measure.X / 2.0f, 5.0f), Color.Black * CurrentAlpha);

                    // Adjust horizontal spacing if we have to (if our element text is just too long)
                    if (targetAlpha != 0 && smallFont.MeasureString(str).X > spacing.X)
                    {
                        targetSpacing.X = measure.X;

                        if (OnSpacingChanged != null)
                            OnSpacingChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the target spacing between the elements
        /// </summary>
        /// <param name="spacing">The X and Y spacing</param>
        public static void SetSpacing(Vector2 spacing)
        {
            targetSpacing = spacing;

            if (OnSpacingChanged != null)
                OnSpacingChanged();
        }
    }
}
