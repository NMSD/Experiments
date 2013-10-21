﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Experiments
{
    public class TestObj
    {

        private readonly int age;
        private readonly int moreage;
        private readonly string name;
        private readonly DateTime timestamp;
        public TestObj()
        {

        }
        public TestObj(int age, string name)
        {
            this.name = name;
            this.age = age;
        }

        //public TestObj(int age, DateTime timestamp)
        //{
        //    this.timestamp = timestamp;
        //    this.age = age;
        //}
    }

    class Program
    {

        static void Main(string[] args)
        {
            string asd = "sadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdgsadfsdg.";
            var second = MeasureExecutionTime.Start(() =>
            {
                PrepareMetric2("R", "mycoolname", "mynkow", 10, "wtf");
            }, 10000000);
            var third = MeasureExecutionTime.Start(() =>
            {
                PrepareMetric3("R", "mycoolname", "mynkow", 10, "wtf");
            }, 10000000);

            // Console.WriteLine(first);
            Console.WriteLine(second);
            Console.WriteLine(third);
            //  Console.WriteLine(fourth);
            Console.ReadLine();
        }

        static string PrepareMetric2(string metricType, string name, string prefix, int value, string postFix = null)
        {
            StringBuilder metricBuilder = new StringBuilder();
            if (true)
            {
                metricBuilder.Append(prefix);
                metricBuilder.Append(".");
                metricBuilder.Append(name);
            }
            else
            {
                metricBuilder.Append(name);
            }
            metricBuilder.Append(":"); metricBuilder.Append(value);
            metricBuilder.Append("|"); metricBuilder.Append(metricType);
            if (postFix != null)
            {
                metricBuilder.Append("|"); metricBuilder.Append(postFix);
            }
            return metricBuilder.ToString();
        }

        static string PrepareMetric3(string metricType, string name, string prefix, int value, string postFix = null)
        {
            return (true ? (prefix + "." + name) : name)
              + ":" + value
              + "|" + metricType
              + (postFix == null ? String.Empty : "|" + postFix);
        }
    }

    public static class MeasureExecutionTime
    {
        public static string Start(Action action)
        {
            string result = string.Empty;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            action();
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            result = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            return result;
        }

        public static string Start(Action action, int repeat)
        {
            string result = string.Empty;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i <= repeat; i++)
            {
                action();
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            result = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            return result;
        }

        public static string Start(Action<int> action, int repeat)
        {
            string result = string.Empty;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i <= repeat; i++)
            {
                action(i);
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            result = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            return result;
        }
    }

    public static class FastActivator
    {
        static ConcurrentDictionary<Type, ObjectActivator> activators = new ConcurrentDictionary<Type, ObjectActivator>();

        public delegate object ObjectActivator(params object[] args);

        static ObjectActivator GetActivator(ConstructorInfo ctor)
        {
            ParameterInfo[] paramsInfo = ctor.GetParameters();
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");
            Expression[] argsExp = new Expression[paramsInfo.Length];
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                argsExp[i] = paramCastExp;
            }
            NewExpression newExp = Expression.New(ctor, argsExp);
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator), newExp, param);
            return (ObjectActivator)lambda.Compile();
        }

        public static void WarmInstanceConstructor(Type type)
        {
            if (!activators.ContainsKey(type))
            {
                var constructors = type.GetConstructors();
                if (constructors.Length == 1)
                {
                    ConstructorInfo ctor = type.GetConstructors().First();
                    activators.TryAdd(type, GetActivator(ctor));
                }
            }
        }

        public static object CreateInstance(Type type, params object[] args)
        {
            ObjectActivator activator;
            if (!activators.TryGetValue(type, out activator))
            {
                var constructors = type.GetConstructors();
                if (constructors.Length == 1)
                {
                    ConstructorInfo ctor = type.GetConstructors().First();
                    activator = GetActivator(ctor);
                    activators.TryAdd(type, activator);
                }
                else
                {
                    activator = (a) => Activator.CreateInstance(type, a);
                }

            }
            return activator(args);
        }
    }
}