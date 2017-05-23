using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using ExpressionParserEngine;

namespace LinearAlgebraGraphicsDemonstration
{
#if WINDOWS || XBOX
    static class Program
    {
        static Demonstration demonstration;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Thread consoleThread = new Thread(new ThreadStart(consoleThreadStart));

            demonstration = new Demonstration();

            // Run demonstration on one thread, console on other. Close out of either then both will shut down, whole program will shut down.
            consoleThread.Start();
            demonstration.Run();
            consoleThread.Abort();
            demonstration.Dispose();

            Environment.Exit(0);
        }

        // Does the not-so-fun task of parsing console user-input and invoking user-requested methods with evaluated arguments
        static void consoleThreadStart()
        {
            // Startup
            Console.WriteLine("Initializing Demonstration");

            // Perhaps a bit naive, but should only be a few milliseconds for this demo
            while (!demonstration.Loaded) { }

            Console.WriteLine("Demonstration loaded");
            Console.WriteLine("Command at will! Use 'Help()' as a reference.");

            PublicAPI apiObject = new PublicAPI(demonstration);
            Type api = apiObject.GetType();

            // Populate our api methods
            List<MethodInfo> info = new List<MethodInfo>();

            MethodInfo[] infoArr = api.GetMethods();

            foreach (MethodInfo inf in infoArr)
            {
                if (inf.DeclaringType != api)
                    continue;
                info.Add(inf);
            }

            // Main input loop
            do
            {
                string input = Console.ReadLine();

                // Parse user input, give feedback of what went wrong if anything, and loop again
                int leftParIndex = input.IndexOf('(');
                if (leftParIndex < 0)
                {
                    Console.WriteLine("Missing expected '('.");
                    continue;
                }
                string name = input.Substring(0, leftParIndex);

                string[] arguments = null;

                try
                {
                    int rightPar = input.IndexOf(')');
                    if (rightPar < 0)
                    {
                        Console.WriteLine("Missing expected ')'.");
                        continue;
                    }

                    arguments = input.Substring(leftParIndex + 1, rightPar - leftParIndex - 1).Split(',');
                    if (arguments.Length == 1 && arguments[0] == "")
                        arguments = null;
                }
                catch
                {
                    arguments = null;
                }

                bool found = false;
                MethodInfo method = null;
                foreach (MethodInfo inf in info)
                {
                    if (inf.Name == name)
                    {
                        found = true;
                        method = inf;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine("Function name not found");
                    continue;
                }

                ParameterInfo[] parameters = method.GetParameters();
                if (arguments != null && arguments.Length != parameters.Length)
                {
                    Console.WriteLine("Parameter count is not correct");
                    continue;
                }


                object[] evaluatedArguments = new object[0];
                if (arguments != null)
                    evaluatedArguments = new object[arguments.Length];

                bool fail = false;

                if (arguments != null)
                {
                    for (int n = 0; n < arguments.Length; n++) // for each argument...
                    {
                        if (parameters[n].ParameterType == typeof(float)) // float is only supported parameter type, currently
                        {
                            Expression expression = null;
                            try
                            {
                                expression = new Expression(arguments[n]);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                fail = true;
                                break;
                            }

                            FieldInfo[] fieldInfos = api.GetFields();

                            foreach (FieldInfo fieldInfo in fieldInfos)
                            {
                                object[] fieldAttributes = fieldInfo.GetCustomAttributes(false);
                                if (fieldAttributes.Length == 1 && fieldAttributes[0] is APIConstantAttribute && fieldInfo.FieldType == typeof(float))
                                {
                                    try
                                    {
                                        expression.SetVariable(fieldInfo.Name, (float)fieldInfo.GetValue(apiObject)); // Try to feed in any constants as 'variable' in expression parser engine
                                    }
                                    catch
                                    {
                                        // Swallow, not necessarily a bad thing in this case
                                    }
                                }
                            }

                            float prs = 0.0f;

                            try
                            {
                                // Evaluate the expression with populated variables using expression parser engine - a completely unrelated school project I did once
                                // That just happened to be useful here
                                prs = (float)expression.Evaluate();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                fail = true;
                                break;
                            }

                            evaluatedArguments[n] = prs;
                        }
                        else
                        {
                            throw new NotImplementedException("This type is not implemented.");
                        }
                    } // End of "for each argument"
                } // end of if arguments != null

                // The end! Invoke the method if success
                if (!fail)
                {
                    try
                    {
                        method.Invoke(apiObject, evaluatedArguments);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            } while (true);
        }
    }
#endif
}

