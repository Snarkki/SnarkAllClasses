// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using Kingmaker.ResourceLinks;
using Kingmaker.UI.Common;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Components;

using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;


namespace SnarkAllClasses.Utilities
{
    /// <summary>
    /// Collection of miscellaneous utilities
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Splits a string on capital letters.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>
        /// Array of split strings.
        /// </returns>
        public static IEnumerable<string> SplitCamelCase(this string text)
        {
            Regex regex = new Regex(@"[\p{Lu}][\p{Ll}]*");
            foreach (Match match in regex.Matches(text))
            {
                yield return match.Value;
            }
        }
        /// <summary>
        /// Executes an action on the called object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="run">
        /// Action that is run on the object.
        /// </param>
        public static void TemporaryContext<T>(this T obj, Action<T> run)
        {
            run?.Invoke(obj);
        }
        /// <summary>
        /// Creates a new object and initializes it with the supplied action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="init">
        /// Action to initialize the new object with.
        /// </param>
        /// <returns>
        /// New object after it has been initialized.
        /// </returns>
        public static T Create<T>(Action<T> init = null) where T : new()
        {
            var result = new T();
            init?.Invoke(result);
            return result;
        }


        public static T CreateCopy<T>(T original, Action<T> init = null)
        {
            var result = (T)ObjectDeepCopier.Clone(original);
            init?.Invoke(result);
            return result;
        }

        public static ActionList CreateActionList(params GameAction[] actions)
        {
            if (actions == null || actions.Length == 1 && actions[0] == null) actions = Array.Empty<GameAction>();
            return new ActionList() { Actions = actions };
        }
        public static void AddAction(this ActionList actionList, params GameAction[] actions)
        {
            if (actions == null || actions.Length == 1 && actions[0] == null) { return; }
            actionList.Actions = actionList.Actions.AppendToArray(actions);
        }
        public static void RemoveActions<T>(this ActionList actionList, Predicate<T> predicate) where T : GameAction
        {
            var actionsToRemove = actionList.Actions.OfType<T>().ToArray();
            foreach (var action in actionsToRemove)
            {
                if (predicate(action))
                {
                    actionList.Actions = actionList.Actions.RemoveFromArray(action);
                }
            }
        }


        internal class ObjectDeepCopier
        {
            internal class ArrayTraverse
            {
                public int[] Position;
                private int[] maxLengths;

                public ArrayTraverse(Array array)
                {
                    maxLengths = new int[array.Rank];
                    for (int i = 0; i < array.Rank; ++i)
                    {
                        maxLengths[i] = array.GetLength(i) - 1;
                    }
                    Position = new int[array.Rank];
                }

                internal bool Step()
                {
                    for (int i = 0; i < Position.Length; ++i)
                    {
                        if (Position[i] < maxLengths[i])
                        {
                            Position[i]++;
                            for (int j = 0; j < i; j++)
                            {
                                Position[j] = 0;
                            }
                            return true;
                        }
                    }
                    return false;
                }
            }
            internal class ReferenceEqualityComparer : EqualityComparer<Object>
            {
                public override bool Equals(object x, object y)
                {
                    return ReferenceEquals(x, y);
                }
                public override int GetHashCode(object obj)
                {
                    if (obj == null) return 0;
                    if (obj is WeakResourceLink wrl)
                    {
                        if (wrl.AssetId == null)
                        {
                            return "WeakResourceLink".GetHashCode();
                        }
                        else
                        {
                            return wrl.GetHashCode();
                        }
                    }
                    return obj.GetHashCode();
                }
            }
            private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

            internal static bool IsPrimitive(Type type)
            {
                if (type == typeof(string)) return true;
                return (type.IsValueType & type.IsPrimitive);
            }
            internal static object Clone(object originalObject)
            {
                return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
            }
            private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
            {
                if (originalObject == null) return null;
                var typeToReflect = originalObject.GetType();
                if (IsPrimitive(typeToReflect)) return originalObject;
                if (originalObject is BlueprintReferenceBase) return originalObject;
                if (visited.ContainsKey(originalObject)) return visited[originalObject];
                if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
                var cloneObject = CloneMethod.Invoke(originalObject, null);
                if (typeToReflect.IsArray)
                {
                    var arrayType = typeToReflect.GetElementType();
                    if (IsPrimitive(arrayType) == false)
                    {
                        Array clonedArray = (Array)cloneObject;
                        ForEach(clonedArray, (array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                    }

                }
                visited.Add(originalObject, cloneObject);
                CopyFields(originalObject, visited, cloneObject, typeToReflect);
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
                return cloneObject;

                void ForEach(Array array, Action<Array, int[]> action)
                {
                    if (array.LongLength == 0) return;
                    ArrayTraverse walker = new ArrayTraverse(array);
                    do action(array, walker.Position);
                    while (walker.Step());
                }
            }
            private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
            {
                if (typeToReflect.BaseType != null)
                {
                    RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                    CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
                }
            }
            private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
            {
                foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
                {
                    if (filter != null && filter(fieldInfo) == false) continue;
                    if (IsPrimitive(fieldInfo.FieldType)) continue;
                    var originalFieldValue = fieldInfo.GetValue(originalObject);
                    var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                    fieldInfo.SetValue(cloneObject, clonedFieldValue);
                }
            }
        }
    }
}