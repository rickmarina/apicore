using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace com.rorisoft.reflection
{
    public class ClassBuilder
    {

        private readonly AssemblyName asemblyName;

        public Type? tipo = null; 
        public ClassBuilder(string ClassName)
        {
            this.asemblyName = new AssemblyName(ClassName)
            {
                Name = ClassName,
                Version = new Version("1.0.0"),
                CultureName = CultureInfo.CurrentCulture.Name
            };
            //this.asemblyName.SetPublicKeyToken(new Guid().ToByteArray());

        }

        public object? CreateObject(string[] PropertyNames, Type[] Types)
        {
            if (PropertyNames.Length != Types.Length)
            {
                Console.WriteLine("The number of property names should match their corresopnding types number");
            }

            if (tipo == null) { 
                TypeBuilder DynamicClass = this.CreateClass();
                this.CreateConstructor(DynamicClass);
                for (int ind = 0; ind < PropertyNames.Count(); ind++)
                    CreateProperty(DynamicClass, PropertyNames[ind], Types[ind]);
                tipo = DynamicClass.CreateType();
            }

            return Activator.CreateInstance(tipo);
        }

        public IList? CreateListObject()
        {
            if (this.tipo is null) 
                throw new InvalidOperationException("Tipo cannot be null to create a list object");

            IList? lista = Activator.CreateInstance(typeof(List<>).MakeGenericType(tipo)) as IList;
            return lista;
        }

        private TypeBuilder CreateClass()
        {

            ArgumentNullException.ThrowIfNull(this.asemblyName, "AssemblyName is null");
            ArgumentNullException.ThrowIfNull(this.asemblyName.Name, "AssemblyName is null");

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(this.asemblyName, AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(this.asemblyName.Name);
            //ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(this.asemblyName.Name+"."+"Informe"
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);
            return typeBuilder;
        }
        private void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }
        private void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
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
}


