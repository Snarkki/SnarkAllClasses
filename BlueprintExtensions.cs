using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;

using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SnarkAllClasses.Utilities;

namespace SnarkAllClasses.Helpers
{
    /// <summary>
    /// Collection of extentions for interacting with blueprints.
    /// </summary>
    public static class BlueprintExtentions
    {

        public static IEnumerable<GameAction> FlattenAllActions(this BlueprintScriptableObject blueprint)
        {
            List<GameAction> actions = new List<GameAction>();
            foreach (var component in blueprint.ComponentsArray.Where(c => c is not null))
            {
                Type type = component.GetType();
                var foundActions = AccessTools.GetDeclaredFields(type)
                    .Where(f => f.FieldType == typeof(ActionList))
                    .SelectMany(field => ((ActionList)field.GetValue(component)).Actions);
                actions.AddRange(FlattenAllActions(foundActions));
            }
            return actions;
        }

        private static IEnumerable<GameAction> FlattenAllActions(this IEnumerable<GameAction> actions)
        {
            List<GameAction> newActions = new List<GameAction>();
            foreach (var action in actions)
            {
                Type type = action?.GetType();
                var foundActions = AccessTools.GetDeclaredFields(type)?
                    .Where(f => f?.FieldType == typeof(ActionList))
                    .SelectMany(field => ((ActionList)field.GetValue(action)).Actions);
                if (foundActions != null) { newActions.AddRange(foundActions); }
            }
            if (newActions.Count > 0)
            {
                return actions.Concat(FlattenAllActions(newActions));
            }
            return actions;
        }

        public static IEnumerable<BlueprintAbility> AbilityAndVariants(this BlueprintAbility ability)
        {
            var List = new List<BlueprintAbility>() { ability };
            var varriants = ability.GetComponent<AbilityVariants>();
            if (varriants != null)
            {
                List.AddRange(varriants.Variants);
            }
            return List;
        }

  

        public static void InsertComponent(this BlueprintScriptableObject obj, int index, BlueprintComponent component)
        {
            var components = obj.ComponentsArray.ToList();
            components.Insert(index, component);
            obj.SetComponents(components.ToArray());
        }
        /// <summary>
        /// Adds new ContextRankConfig to the object and initalizes it with the supplied action.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="init">
        /// Action to initialize new ContextRankConfig.
        /// </param>

        /// <summary>
        /// Adds new component to the object's ComponentsArray and initalizes it with the supplied action.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="component">
        /// Components to add.
        /// </param>
        public static void AddComponent(this BlueprintScriptableObject obj, BlueprintComponent component)
        {
            obj.SetComponents(obj.ComponentsArray.AppendToArray(component));
        }
        /// <summary>
        /// Adds new component to the object's ComponentsArray and initalizes it with the supplied action.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="init">
        /// Action to initialize new Component.
        /// </param>
        public static void AddComponent<T>(this BlueprintScriptableObject obj, Action<T> init = null) where T : BlueprintComponent, new()
        {
            obj.SetComponents(obj.ComponentsArray.AppendToArray(SnarkAllClasses.Utilities.Helpers.Create(init)));
        }
        /// <summary>
        /// Adds new components to the object's ComponentsArray.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="components">
        /// Components to add.
        /// </param>
        public static void AddComponents(this BlueprintScriptableObject obj, IEnumerable<BlueprintComponent> components) => AddComponents(obj, components.ToArray());
        /// <summary>
        /// Adds new components to the object's ComponentsArray.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="components">
        /// Components to add.
        /// </param>
        public static void AddComponents(this BlueprintScriptableObject obj, params BlueprintComponent[] components)
        {
            var c = obj.ComponentsArray.ToList();
            c.AddRange(components);
            obj.SetComponents(c.ToArray());
        }
        /// <summary>
        /// Removes specified BlueprintComponent from the object's ComponentsArray.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="component">
        /// BlueprintComponent to remove.
        /// </param>
        public static void RemoveComponent(this BlueprintScriptableObject obj, BlueprintComponent component)
        {
            obj.SetComponents(obj.ComponentsArray.RemoveFromArray(component));
        }
        /// <summary>
        /// Removes BlueprintComponents of the specified type from the object's ComponentsArray.
        /// </summary>
        /// <typeparam name="T">
        /// Type of BlueprintComponent to remove.
        /// </typeparam>
        /// <param name="obj"></param>
        public static void RemoveComponents<T>(this BlueprintScriptableObject obj) where T : BlueprintComponent
        {
            var compnents_to_remove = obj.GetComponents<T>().ToArray();
            foreach (var c in compnents_to_remove)
            {
                obj.SetComponents(obj.ComponentsArray.RemoveFromArray(c));
            }
        }
        /// <summary>
        /// Removes BlueprintComponents that match the supplied Predicate from the object's ComponentsArray.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="predicate">
        /// Predicate to determine which components to remove.
        /// </param>
        public static void RemoveComponents<T>(this BlueprintScriptableObject obj, Predicate<T> predicate) where T : BlueprintComponent
        {
            var compnents_to_remove = obj.GetComponents<T>().ToArray();
            foreach (var c in compnents_to_remove)
            {
                if (predicate(c))
                {
                    obj.SetComponents(obj.ComponentsArray.RemoveFromArray(c));
                }
            }
        }
        /// <summary>
        /// Gets the first component that matches the supplied Predicate from the object's ComponentsArray.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="predicate">
        /// Predicate to determine which component to get.
        /// </param>
        /// <returns>
        /// First BlueprintComponent that matches the supplied predicate.
        /// </returns>
        [Obsolete("Replaced in Kingmaker.Blueprints.BlueprintExtenstions")]
        private static T GetComponent<T>(this BlueprintScriptableObject obj, Predicate<T> predicate) where T : BlueprintComponent
        {
            return obj.GetComponents<T>().Where(c => predicate(c)).FirstOrDefault();
        }
        /// <summary>
        /// Gets all components that match the supplied Predicate from the object's ComponentsArray.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="predicate">
        /// Predicate to determine which components to get.
        /// </param>
        /// <returns>
        /// IEnumerable that contains all components that match the supplied predicate.
        /// </returns>
        public static IEnumerable<T> GetComponents<T>(this BlueprintScriptableObject obj, Predicate<T> predicate) where T : BlueprintComponent
        {
            return obj.GetComponents<T>().Where(c => predicate(c)).ToArray();
        }
        /// <summary>
        /// Removes all components from the object's ComponentsArray that match the supplied predicate, 
        /// and if a component is removed in this way add a new component to the object's ComponentsArray.
        /// </summary>
        /// <param name="blueprint"></param>
        /// <param name="predicate">
        /// Predicate to determine which components to remove.
        /// </param>
        /// <param name="newComponent">
        /// Component to add if a component is removed.
        /// </param>
        public static void ReplaceComponents(this BlueprintScriptableObject blueprint, Predicate<BlueprintComponent> predicate, BlueprintComponent newComponent)
        {
            bool found = false;
            foreach (var component in blueprint.ComponentsArray)
            {
                if (predicate(component))
                {
                    blueprint.RemoveComponent(component);
                    found = true;
                }
            }
            if (found)
            {
                blueprint.AddComponent(newComponent);
            }
        }
        /// <summary>
        /// Removes all components from the object's ComponentsArray that match the supplied type, 
        /// and if a component is removed in this way add a new component to the object's ComponentsArray.
        /// </summary>
        /// <typeparam name="T">
        /// Type of BlueprintComponent to remove from the object's ComponentsArray.
        /// </typeparam>
        /// <param name="blueprint"></param>
        /// <param name="newComponent">
        /// Component to add if a component is removed.
        /// </param>
        public static void ReplaceComponents<T>(this BlueprintScriptableObject blueprint, BlueprintComponent newComponent) where T : BlueprintComponent
        {
            blueprint.ReplaceComponents<T>(c => true, newComponent);
        }
        /// <summary>
        /// Removes all components from the object's ComponentsArray that match the supplied type and Predicate, 
        /// and if a component is removed in this way add a new component to the object's ComponentsArray.
        /// </summary>
        /// <typeparam name="T">
        /// Type of BlueprintComponent to remove from the object's ComponentsArray.
        /// </typeparam>
        /// <param name="blueprint"></param>
        /// /// <param name="predicate">
        /// Predicate to determine which components to remove.
        /// </param>
        /// <param name="newComponent">
        /// Component to add if a component is removed.
        /// </param>
        public static void ReplaceComponents<T>(this BlueprintScriptableObject blueprint, Predicate<T> predicate, BlueprintComponent newComponent) where T : BlueprintComponent
        {
            var components = blueprint.GetComponents<T>().ToArray();
            bool found = false;
            foreach (var component in components)
            {
                if (predicate(component))
                {
                    blueprint.RemoveComponent(component);
                    found = true;
                }
            }
            if (found)
            {
                blueprint.AddComponent(newComponent);
            }
        }
        /// <summary>
        /// Overrides the object's existing ComponentsArray with a new ComponentsArray filled with the supplied components.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="components">
        /// Components to set as the new ComponentsArray.
        /// </param>
        public static void SetComponents(this BlueprintScriptableObject obj, params BlueprintComponent[] components)
        {
            // Fix names of components. Generally this doesn't matter, but if they have serialization state,
            // then their name needs to be unique.
            var names = new HashSet<string>();
            foreach (var c in components)
            {
                if (string.IsNullOrEmpty(c.name))
                {
                    c.name = $"${c.GetType().Name}";
                    //c.name = $"${c.GetType().Name}${Guid.NewGuid()}";
                }
                if (!names.Add(c.name))
                {
                    string name;
                    for (int i = 0; !names.Add(name = $"{c.name}${i}"); i++) ;
                    c.name = name;
                }
            }
            obj.ComponentsArray = components;
            obj.OnEnable(); // To make sure components are fully initialized
        }
        /// <summary>
        /// Overrides the object's existing ComponentsArray with a new ComponentsArray filled with the supplied components.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="components">
        /// Components to set as the new ComponentsArray.
        /// </param>
        public static void SetComponents(this BlueprintScriptableObject obj, IEnumerable<BlueprintComponent> components)
        {
            SetComponents(obj, components.ToArray());
        }
    }
}