using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace LinearAlgebraGraphicsDemonstration
{
    /// <summary>
    /// Add functions to this class to add functionality to the console. It will be searched by reflection to make that possible.
    /// Must define documentation using the APIMethod attribute or APIConstant attribute so Help() can work properly
    /// </summary>
    class PublicAPI
    {
        Demonstration demonstration;

        [APIConstant("Represents PI")]
        public const float PI = MathHelper.Pi;

        public PublicAPI(Demonstration demonstration)
        {
            this.demonstration = demonstration;
        }

        [APIMethod("Allows you to define your own matrix and add it to the transform list.",
            "Row 1, Column 1", "Row 1, Column 2", "Row 1, Column 3", "Row 1, Column 4",
            "Row 2, Column 1", "Row 2, Column 2", "Row 2, Column 3", "Row 2, Column 4",
            "Row 3, Column 1", "Row 3, Column 2", "Row 3, Column 3", "Row 3, Column 4",
            "Row 4, Column 1", "Row 4, Column 2", "Row 4, Column 3", "Row 4, Column 4")]
        public void Define(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            demonstration.AddInputMatrix(m11, m21, m31, m41, m12, m22, m32, m42, m13, m23, m33, m43, m14, m24, m34, m44);
        }

        [APIMethod("Clears all of the transforms (excluding projection, view, and data).")]
        public void Clear()
        {
            demonstration.ClearTransform();
        }

        [APIMethod("Closes out of the program.")]
        public void Exit()
        {
            demonstration.Exit();
        }

        [APIMethod("Replaces the matrices on-screen (excluding projection, view, and data) with the result of their multiplication.")]
        public void Flatten()
        {
            demonstration.FlattenTransform();
        }

        [APIMethod("Prints out the documentation on the public API.")]
        public void Help()
        {
            Console.WriteLine("API Reference:");
            Console.WriteLine();
            Console.WriteLine("Constants:");
            Type t = this.GetType();

            FieldInfo[] fieldInfo = t.GetFields();
            foreach (FieldInfo i in fieldInfo)
            {
                object[] atts = i.GetCustomAttributes(false);
                if (atts.Length == 1 && atts[0] is APIConstantAttribute)
                {
                    APIConstantAttribute constant = atts[0] as APIConstantAttribute;

                    string tName = i.FieldType.Name;
                    if (tName == "Single")
                        tName = "float";
                    Console.WriteLine(tName + " " + i.Name);
                    Console.WriteLine("\t" + constant.Description);
                    Console.WriteLine("\t" + i.GetValue(this));
                }
            }

            Console.WriteLine();
            Console.WriteLine("Functions:");

            MethodInfo[] info = t.GetMethods();
            foreach (MethodInfo inf in info)
            {
                object[] attributes = inf.GetCustomAttributes(false);
                if (attributes.Length == 1 && attributes[0] is APIMethodAttribute)
                {
                    APIMethodAttribute att = attributes[0] as APIMethodAttribute;
                    string definitionString = inf.Name + "(";

                    ParameterInfo[] parameters = inf.GetParameters();
                    foreach (ParameterInfo par in parameters)
                    {
                        string typeName = par.ParameterType.Name;
                        if (typeName == "Single")
                            typeName = "float";
                        definitionString += typeName + " " + par.Name + ",";
                    }

                    if (parameters.Length > 0)
                        definitionString = definitionString.Substring(0, definitionString.Length - 1);
                    definitionString += ")";

                    Console.WriteLine(definitionString);

                    Console.WriteLine("\t" + att.Description);

                    for (int n = 0; n < parameters.Length && n < att.ParameterDescriptions.Length; n++)
                    {
                        Console.WriteLine("\t\t" + parameters[n].Name);
                        Console.WriteLine("\t\t\t" + att.ParameterDescriptions[n]);
                    }

                    Console.WriteLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        [APIMethod("Rotates the selected matrix about the x axis.", "The angle to rotate in radians.")]
        public void RotX(float radians)
        {
            demonstration.AddRotationMatrix(RotationMatrixMajorAxis.X, radians);
        }

        [APIMethod("Rotates the selected matrix about the y axis.", "The angle to rotate in radians.")]
        public void RotY(float radians)
        {
            demonstration.AddRotationMatrix(RotationMatrixMajorAxis.Y, radians);
        }

        [APIMethod("Rotates the selected matrix about the z axis.", "The angle to rotate in radians.")]
        public void RotZ(float radians)
        {
            demonstration.AddRotationMatrix(RotationMatrixMajorAxis.Z, radians);
        }

        [APIMethod("Scales the selected matrix on xyz.", "X scale", "Y scale", "Z scale")]
        public void Scale(float x, float y, float z)
        {
            demonstration.AddScaleMatrix(new Vector3(x, y, z));
        }

        [APIMethod("Translates the selected matrix on xyz.", "X translation", "Y translation", "Z translation")]
        public void Trans(float x, float y, float z)
        {
            demonstration.AddTranslationMatrix(new Vector3(x, y, z));
        }
    }
}
