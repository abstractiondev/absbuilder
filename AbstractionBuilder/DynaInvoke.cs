using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Reflection;

namespace AbstractionBuilder
{
    public class DynaInvoke
    {
        // this way of invoking a function

        // is slower when making multiple calls

        // because the assembly is being instantiated each time.

        // But this code is clearer as to what is going on

        public static Object InvokeMethodSlow(string AssemblyName,
                                              string ClassName, string MethodName, Object[] args)
        {
            // load the assemly

            Assembly assembly = Assembly.LoadFrom(AssemblyName);

            // Walk through each type in the assembly looking for our class

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass == true)
                {
                    if (type.FullName.EndsWith("." + ClassName))
                    {
                        // create an instance of the object

                        object ClassObj = Activator.CreateInstance(type);

                        // Dynamically Invoke the method

                        object Result = type.InvokeMember(MethodName,
                                                          BindingFlags.Default | BindingFlags.InvokeMethod,
                                                          null,
                                                          ClassObj,
                                                          args);
                        return (Result);
                    }
                }
            }
            throw (new System.Exception("could not invoke method"));
        }

        // ---------------------------------------------

        // now do it the efficient way

        // by holding references to the assembly

        // and class


        // this is an inner class which holds the class instance info

        public class DynaClassInfo
        {
            public Type type;
            public Object ClassObject;

            public DynaClassInfo()
            {
            }

            public DynaClassInfo(Type t, Object c)
            {
                type = t;
                ClassObject = c;
            }
        }


        public static Hashtable AssemblyReferences = new Hashtable();
        public static Dictionary<string, DynaClassInfo> ClassReferences = new Dictionary<string, DynaClassInfo>();

        public static DynaClassInfo
            GetClassReference(string assemblyFullPath, string className, bool searchWithinCallingAssembly = true)
        {
            string assemblyClassName = assemblyFullPath + "." + className;
            DynaClassInfo classInfo = null;
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyFullPath);

            if (!ClassReferences.TryGetValue(assemblyClassName, out classInfo))
            {
                Assembly assembly = null;
                if (searchWithinCallingAssembly)
                {
                    var currAssembly = Assembly.GetExecutingAssembly();
                    var currAssemblyExistingClass = getClassInfo(currAssembly, className);
                    if (currAssemblyExistingClass != null)
                    {
                        assembly = currAssembly;
                        classInfo = currAssemblyExistingClass;
                    }
                    else
                    {
                        try
                        {
                            assembly = Assembly.Load(assemblyName);
                        }
                        catch (FileNotFoundException)
                        {
                            
                        }
                    }
                }
                if(assembly == null)
                {
                    if (AssemblyReferences.ContainsKey(assemblyFullPath) == false)
                    {
                        AssemblyReferences.Add(assemblyFullPath,
                            assembly = Assembly.LoadFrom(assemblyFullPath));
                    }
                    else
                        assembly = (Assembly) AssemblyReferences[assemblyFullPath];
                }

                classInfo = classInfo ?? getClassInfo(assembly, className);
                if(classInfo == null)
                    throw new System.Exception("Could not instantiate class: " + className);

                ClassReferences.Add(assemblyClassName, classInfo);
            }
            return classInfo;
        }

        private static DynaClassInfo getClassInfo(Assembly assembly, string className)
        {
            var classNameSuffix = "." + className;
            // Walk through each type in the assembly
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass == true && type.FullName.EndsWith(classNameSuffix))
                {
                    var classInfo = new DynaClassInfo(type, Activator.CreateInstance(type));
                    return classInfo;
                }
            }
            return null;
        }

        public static Object InvokeMethod(DynaClassInfo ci,
                                          string MethodName, Object[] args)
        {
            // Dynamically Invoke the method
            Object Result = ci.type.InvokeMember(MethodName,
                                                 BindingFlags.Default | BindingFlags.InvokeMethod,
                                                 null,
                                                 ci.ClassObject,
                                                 args);
            return (Result);
        }

        // --- this is the method that you invoke ------------
        public static Object InvokeMethod(string AssemblyName,
                                          string ClassName, string MethodName, Object[] args)
        {
            DynaClassInfo ci = GetClassReference(AssemblyName, ClassName);
            return (InvokeMethod(ci, MethodName, args));
        }

        // --- this is the method that you invoke ------------
        // Passes also AbstractionEnvironment as a first argument to caller
        public static Object InvokeMethod(string AssemblyName,
                                          string ClassName, string MethodName, AbstractionEnvironment env, Object[] args)
        {
            Object[] allArgs = new Object[] {env, args};
            return InvokeMethod(AssemblyName, ClassName, MethodName, allArgs);
        }
    }
}