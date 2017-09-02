using System;
using System.Drawing;

namespace JaniPellikka.Windows.Forms.Content
{
    /// <summary>
    /// Data for a <see cref="Char"/> that dictates how to render
    /// </summary>
    internal class CharData : IEquatable<CharData>
    {
        /// <summary>
        /// Foreground color
        /// </summary>
        public Color? ForeColor;
        /// <summary>
        /// Background color
        /// </summary>
        public Color? BackColor;
        /// <summary>
        /// Font for the <see cref="Char"/>
        /// </summary>
        /// <remarks>Note that this is a font reference to the views font with different <see cref="FontStyle"/>s like bold, underline etc. You must NOT set this yourself</remarks>
        public Font Font { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="font">The views font</param>
        /// <param name="foreColor">Foreground color</param>
        /// <param name="backColor">Background color</param>
        public CharData(Font font, Color? foreColor = null, Color? backColor = null)
        {
            Font = font;
            ForeColor = foreColor;
            BackColor = backColor;
        }

        
        /// <summary>
        /// Checks if this is equal to another CharData
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if a match</returns>
        public bool Equals(CharData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ForeColor == other.ForeColor && BackColor == other.BackColor && Font.Equals(other.Font);
        }
        /// <summary>
        /// Checks if this is equal to another object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if a match</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CharData) obj);
        }
        /// <summary>
        /// Gets Hash Code
        /// </summary>
        /// <returns>Hash Code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                int hashCode = Font.GetHashCode();
                hashCode = (hashCode * 397) ^ ForeColor.GetHashCode();
                hashCode = (hashCode * 397) ^ BackColor.GetHashCode();
                // ReSharper enable NonReadonlyMemberInGetHashCode
                return hashCode;
            }
        }
    }
}