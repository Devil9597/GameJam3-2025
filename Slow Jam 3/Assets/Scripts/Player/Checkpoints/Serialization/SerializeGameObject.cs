using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Player.Checkpoints.Serialization
{
    public class SerializeGameObject : MonoBehaviour
    {
        public List<Component> ComponentsToSerialize = new();


        // for now we are storing this in memory with hard references to the components, if time allows
        // come back and make this work from disk
        private List<(Component component, FieldInfo fieldInfo, object obj)> _fields = new();
        private List<(Component component, PropertyInfo propertyInfo, object obj)> _properties = new();
        private bool _isObjectEnabled;

        private void Start()
        {
            SaveState();
        }

        public void SaveState()
        {
            _fields.Clear();
            _properties.Clear();
            _isObjectEnabled = gameObject.activeInHierarchy;
            foreach (var component in ComponentsToSerialize)
            {
                var props = component.GetType().GetProperties()
                    .Where(p => p.GetCustomAttribute<SerializeDataAttribute>() != null).ToList();

                foreach (var property in props)
                {
                    _properties.Add(new(component, property, property.GetValue(component)));
                }

                var fields = component.GetType().GetRuntimeFields()
                    .Where(f => f.GetCustomAttribute<SerializeDataAttribute>() != null).ToList();

                foreach (var field in fields)
                {
                    _fields.Add(new(component, field, field.GetValue(component)));
                }
            }
        }

        public void LoadState()
        {
            Debug.Log($"{gameObject.name}, is being enabled:{_isObjectEnabled}");
            gameObject.SetActive(_isObjectEnabled);
            foreach (var prop in _properties)
            {
                prop.propertyInfo.SetValue(prop.component, prop.obj);
            }

            foreach (var field in _fields)
            {
                field.fieldInfo.SetValue(field.component, field.obj);
            }
        }

        private void Update()
        {
        }
    }
}