﻿#if WINDOWS

using System;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Delta.Tiled
{
    public class StyleSheet
    {
        static Dictionary<string, IEntity> _globalObjectStyles = new Dictionary<string, IEntity>();
        public static Dictionary<string, IEntity> GlobalObjectStyles {  get { return _globalObjectStyles; } }

        XmlDocument _document = new XmlDocument();
        [ContentSerializer(FlattenContent = true, CollectionItemName = "ObjectStyle")]
        public Dictionary<string, IEntity> ObjectStyles { get; private set; }

        public StyleSheet()
            : base()
        {
            ObjectStyles = new Dictionary<string, IEntity>();
        }

        public StyleSheet(string fileName)
            : this()
        {
            _document.Load(fileName);
            foreach (XmlNode node in _document.DocumentElement.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;
                string typeName = node.Attributes["Type"] == null ? string.Empty : node.Attributes["Type"].Value;
                if (string.IsNullOrEmpty(typeName))
                    continue;
                IEntity entity = null;
                if (ObjectStyles.ContainsKey(typeName))
                    entity = ObjectStyles[typeName].Copy() as IEntity;
                else
                    entity = CreateInstance(typeName);
                if (entity == null)
                    continue;
                entity.ID = node.Name;
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (!entity.ImportCustomValues(childNode.Name.ToLower(), childNode.InnerText))
                        throw new Exception(String.Format("Could not import XML property '{0}', no such property exists for '{1}'.", childNode.Name.ToLower(), entity.GetType().Name));
                }
                ObjectStyles.Add(node.Name, entity);
            }
        }

        static IEntity CreateInstance(string typeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeName, false, true);
                if (type != null)
                    return Activator.CreateInstance(type) as IEntity;
            }
            return null;
        }

        public static IEntity Load(string name)
        {
            if (_globalObjectStyles.ContainsKey(name))
                return _globalObjectStyles[name].Copy() as IEntity;
            return CreateInstance(name);
        }

    }
}

#endif