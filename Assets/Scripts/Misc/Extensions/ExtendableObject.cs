using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Misc.Extensions
{
	public class ExtendableObject<T> 
		where T : ExtendableObject<T>
	{
		private readonly object[] _properties;
		
		private static readonly List<Property<T>> s_Properties = new List<Property<T>>();
		
		public ExtendableObject()
		{
			_properties = new object[s_Properties.Count];
		}

		internal TProperty GetProperty<TProperty>(Property<T, TProperty> property)
		{
			int index = property.Index;
			Debug.Assert(_properties.Length >= index, $"Property {property.Name} should be registered !");
			object value = _properties[index];
			return (TProperty)value;
		}

		internal void SetProperty<TProperty>(Property<T, TProperty> property, TProperty value)
		{
			int index = property.Index;
			Debug.Assert(_properties.Length >= index, $"Property {property.Name} should be registered !");
			_properties[index] = value;
		}

		
		public static void RegisterProperty(Property<T> property)
		{
			string name = property.Name;
			Debug.Assert(s_Properties.All(p => p.Name != name));
			property.Index = s_Properties.Count;
			s_Properties.Add(property);
		}
	}
	public abstract class Property<T>
	{
		public readonly string Name;
		public readonly Type   Type;

		public int Index { get; internal set; }

		protected Property(string name, Type type)
		{
			Name = name;
			Type = type;
		}
	}
	
	public class Property<TInstance, TProperty> : Property<TInstance> 
		where TInstance : ExtendableObject<TInstance>
	{
		public Property(string name) 
			: base(name, typeof(TProperty))
		{
			
		}

		public TProperty Get(TInstance instance) =>
			instance.GetProperty(this);

		public void Set(TInstance instance, TProperty value) =>
			instance.SetProperty(this, value);
	}
}