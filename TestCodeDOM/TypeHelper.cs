using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace TestCodeDOM
{
    public class TypeHelper
    {

        private ConcurrentDictionary<string, Type> _dictType = new ConcurrentDictionary<string, Type>();

        private readonly ModuleBuilder _builder;
        public TypeHelper(Type type)
        {
            _builder = AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName(type.Assembly.GetName().Name), AssemblyBuilderAccess.Run)
                .DefineDynamicModule(type.Module.Name);
        }


        public Type AddType(string typeName, string[] PropertyNames, Type[] Types)
        {
            TypeBuilder typeBuilder = _builder.DefineType(typeName
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            for (int ind = 0; ind < PropertyNames.Count(); ind++)
            {
                CreateProperty(typeBuilder, PropertyNames[ind], Types[ind]);
            }

            Type type = typeBuilder.CreateType();

            _dictType.TryAdd(typeName, type);
            return type;

            static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
            {
                FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
                MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
                ILGenerator getIl = getPropMthdBldr.GetILGenerator();

                getIl.Emit(OpCodes.Ldarg_0);
                getIl.Emit(OpCodes.Ldfld, fieldBuilder);
                getIl.Emit(OpCodes.Ret);

                MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                      MethodAttributes.Public |
                      MethodAttributes.SpecialName |
                      MethodAttributes.HideBySig,
                      null, new[] { propertyType });

                ILGenerator setIl = setPropMthdBldr.GetILGenerator();
                Label modifyProperty = setIl.DefineLabel();
                Label exitSet = setIl.DefineLabel();

                setIl.MarkLabel(modifyProperty);
                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldarg_1);
                setIl.Emit(OpCodes.Stfld, fieldBuilder);

                setIl.Emit(OpCodes.Nop);
                setIl.MarkLabel(exitSet);
                setIl.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getPropMthdBldr);
                propertyBuilder.SetSetMethod(setPropMthdBldr);
            }
        }

        public Type GetType(string typeName)
        {
            _dictType.TryGetValue(typeName, out var type);

            return type;
        }

        public List<Type> GetTypes()
        {
            return _()?.ToList() ?? new List<Type>(); IEnumerable<Type> _()
            {
                foreach (var item in _dictType)
                    yield return item.Value;
            }
        }
    }
}
