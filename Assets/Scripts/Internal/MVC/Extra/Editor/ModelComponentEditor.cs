﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MVC
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ModelComponentEditorAttribute : Attribute
	{
		public Type ComponentType;
		public ModelComponentEditorAttribute(Type componentType)
		{
			ComponentType = componentType;
		}
	}

	public class ModelComponentEditor
	{
		public bool ShouldStayClosed
		{
			get
			{
				return GetType() == typeof(ModelComponentEditor) && (_editorFields.Length + _editorMethods.Length) == 0;
			}
		}

		private Dictionary<ParameterInfo, object> _parameterEditorValuesMap = new Dictionary<ParameterInfo, object>();
		private FieldInfo[] _editorFields = new FieldInfo[] { };
		private MethodInfo[] _editorMethods = new MethodInfo[] { };

		private bool _showMethods = true;
		private bool _showFields = true;

		public void CallOnOpen(BaseModelComponent component)
		{
			_editorFields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.GetCustomAttributes(typeof(ModelEditorFieldAttribute), true).Length > 0).ToArray();
			_editorMethods = component.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.GetCustomAttributes(typeof(ModelEditorMethodAttribute), true).Length > 0).ToArray();
			_showFields = true;
			_showMethods = false;
			OnOpen();
		}

		public void CallOnClose()
		{
			OnClose();
			_parameterEditorValuesMap.Clear();
		}

		public virtual void OnGUI(BaseModelComponent component)
		{
			if (_editorFields.Length > 0)
			{
				_showFields = EditorGUILayout.Foldout(_showFields, string.Format("Fields ({0}): ", _editorFields.Length));
				if (_showFields)
				{
					EditorGUILayout.BeginVertical(GUI.skin.box);
					foreach (FieldInfo editorField in _editorFields)
					{
						EditorGUILayout.BeginVertical(GUI.skin.box);
						DrawField(editorField, component);
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndVertical();

					GUILayout.Space(5f);
				}
			}

			if (_editorMethods.Length > 0)
			{
				_showMethods = EditorGUILayout.Foldout(_showMethods, string.Format("Methods ({0}): ", _editorMethods.Length));
				if (_showMethods)
				{
					EditorGUILayout.BeginVertical(GUI.skin.box);
					foreach (MethodInfo editorMethod in _editorMethods)
					{
						EditorGUILayout.BeginVertical(GUI.skin.box);
						DrawMethod(editorMethod, component);
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndVertical();
				}
			}
		}

		protected virtual void OnOpen()
		{

		}

		protected virtual void OnClose()
		{

		}

		private void DrawMethod(MethodInfo editorMethod, BaseModelComponent obj)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Method: " + editorMethod.Name);
			if (GUILayout.Button("Invoke", GUILayout.Width(70)))
			{
				ParameterInfo[] pms = editorMethod.GetParameters();
				List<object> parameters = new List<object>();
				for (int i = 0; i < pms.Length; i++)
				{
					foreach (var pair in _parameterEditorValuesMap)
					{
						if (pair.Key == pms[i])
						{
							parameters.Add(pair.Value);
						}
					}
				}

				editorMethod.Invoke(obj, parameters.ToArray());
			}
			EditorGUILayout.EndHorizontal();

			if (editorMethod.GetParameters().Length > 0)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(25f);
				GUILayout.Label("Parameters: ");
				EditorGUILayout.EndHorizontal();
				foreach (ParameterInfo parameterInfo in editorMethod.GetParameters())
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(45f);
					Type parameterType = parameterInfo.ParameterType;
					object preValue;
					object value;

					if (_parameterEditorValuesMap.ContainsKey(parameterInfo))
					{
						preValue = _parameterEditorValuesMap[parameterInfo];
					}
					else
					{
						preValue = string.IsNullOrEmpty(parameterInfo.RawDefaultValue.ToString()) ? (parameterType.IsValueType ? Activator.CreateInstance(parameterType) : null) : parameterInfo.DefaultValue;
					}

					try
					{
						value = DrawTypeField(parameterInfo.Name, parameterType, preValue);
						if (value != preValue)
						{
							if (!_parameterEditorValuesMap.ContainsKey(parameterInfo))
							{
								_parameterEditorValuesMap.Add(parameterInfo, value);
							}

							_parameterEditorValuesMap[parameterInfo] = value;
						}
					}
					catch (Exception e)
					{
						GUILayout.Label("Parameter `" + parameterInfo.Name + "` can't be used. Error: " + e.Message);
					}
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		private void DrawField(FieldInfo field, BaseModelComponent obj)
		{
			Type fieldType = field.FieldType;
			object newValue = DrawTypeField("Field: " + field.Name.Replace("_", " ").Trim(), fieldType, field.GetValue(obj));
			field.SetValue(obj, newValue);
		}

		private object DrawTypeField(string labelName, Type fieldType, object value)
		{
			if (fieldType == typeof(string))
			{
				return EditorGUILayout.TextField(labelName, (string)value);
			}
			else if (fieldType == typeof(int))
			{
				if (value == null)
				{
					value = default(int);
				}
				return EditorGUILayout.IntField(labelName, (int)value);
			}
			else if (fieldType == typeof(float))
			{
				if (value == null)
				{
					value = default(float);
				}
				return EditorGUILayout.FloatField(labelName, (float)value);
			}
			else if (fieldType == typeof(Vector2))
			{
				if (value == null)
				{
					value = default(Vector2);
				}
				return EditorGUILayout.Vector2Field(labelName, (Vector2)value);
			}
			else if (fieldType == typeof(Vector3))
			{
				if (value == null)
				{
					value = default(Vector3);
				}
				return EditorGUILayout.Vector3Field(labelName, (Vector3)value);
			}
			else if (fieldType == typeof(Vector2Int))
			{
				if (value == null)
				{
					value = default(Vector2Int);
				}
				return EditorGUILayout.Vector2IntField(labelName, (Vector2Int)value);
			}
			else if (fieldType == typeof(Vector3Int))
			{
				if (value == null)
				{
					value = default(Vector3Int);
				}
				return EditorGUILayout.Vector3IntField(labelName, (Vector3Int)value);
			}
			else if (typeof(BaseModel).IsAssignableFrom(fieldType))
			{
				BaseModel target = value as BaseModel;
				MonoBaseView view = null;

				if (target != null && !target.IsDestroyed)
				{
					view = MVCUtil.GetView<MonoBaseView>(target);
				}

				view = EditorGUILayout.ObjectField(labelName, view, typeof(MonoBaseView), true) as MonoBaseView;

				if (view != null)
				{
					BaseModel m = MVCUtil.GetModel<BaseModel>(view);
					if (fieldType.IsAssignableFrom(m.GetType()))
					{
						return m;
					}
					else
					{
						return null;
					}
				}
				else
				{
					return null;
				}
			}

			throw new Exception(string.Format("Type {0} is not supported", fieldType));
		}
	}
}