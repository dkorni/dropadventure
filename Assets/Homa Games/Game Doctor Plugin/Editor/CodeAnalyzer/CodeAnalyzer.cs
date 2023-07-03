using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace HomaGames.CodeAnalyzer
{
    public static class CodeAnalyzer
    {
        /// <summary>
        /// Finds usage of a method inside an assembly.
        /// </summary>
        /// <param name="method">The method to find usages of</param>
        /// <param name="assembly">The assembly to look inside</param>
        /// <param name="bindingFlags">Allowed visibility for scanned methods</param>
        /// <returns>The list of methods using the method</returns>
        [PublicAPI]
        public static List<MethodBase> FindUsageOfMethod(MethodInfo method, Assembly assembly,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                        BindingFlags.Static)
        {
            List<MethodBase> methods = new List<MethodBase>();
            foreach (var type in assembly.GetTypes())
            {
                List<MethodBase> methodBases = new List<MethodBase>();
                methodBases.AddRange(type.GetMethods(bindingFlags));
                methodBases.AddRange(type.GetConstructors());
                foreach (var bodyMethod in methodBases)
                {
                    try
                    {
                        methods.AddRange(FindMethodUsageInsideMethodBody(method, bodyMethod));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            return methods;
        }

        /// <summary>
        /// Finds usage of a method inside another method.
        /// </summary>
        /// <param name="method">The method to find usages of</param>
        /// <param name="bodyMethod">The method to look inside</param>
        /// <returns>The list of methods using the method</returns>
        [PublicAPI]
        public static List<MethodBase> FindMethodUsageInsideMethodBody(MethodInfo method, MethodBase bodyMethod)
        {
            List<MethodBase> methodsUsingIt = new List<MethodBase>();
            FindMethodUsageInsideMethodBodyRecursive(method, bodyMethod, methodsUsingIt, 0);
            return methodsUsingIt;
        }

        private static void FindMethodUsageInsideMethodBodyRecursive(MethodInfo method, MethodBase bodyMethod,
            List<MethodBase> methodsUsingIt, int currentDepth)
        {
            const int maxSearchDepth = 4;
            var instructions = MethodBodyReader.GetInstructions(bodyMethod);

            foreach (Instruction instruction in instructions)
            {
                MethodInfo methodInfo = instruction.Operand as MethodInfo;

                if (methodInfo != null)
                {
                    if (methodInfo == method)
                    {
                        methodsUsingIt.Add(bodyMethod);
                    }
                    // This is a lambda declaration
                    else if (methodInfo.Name.Contains("<") && currentDepth < maxSearchDepth)
                    {
                        currentDepth++;
                        FindMethodUsageInsideMethodBodyRecursive(method, methodInfo, methodsUsingIt, currentDepth);
                    }
                }
            }
        }
    }
}