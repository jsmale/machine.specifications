using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Explorers
{
  public class AssemblyExplorer
  {
    private readonly ContextFactory contextFactory;

    public AssemblyExplorer()
    {
      contextFactory = new ContextFactory();
    }

    public IEnumerable<Model.Context> FindDescriptionsIn(Assembly assembly)
    {
      return EnumerateDescriptionsIn(assembly).Select(x => CreateDescriptionFrom(x));
    }

    public IEnumerable<Model.Context> FindDescriptionsIn(Assembly assembly, string targetNamespace)
    {
      return EnumerateDescriptionsIn(assembly)
        .Where(x => x.Namespace == targetNamespace)
        .Select(x => CreateDescriptionFrom(x));
    }

    private Model.Context CreateDescriptionFrom(Type type)
    {
      object instance = Activator.CreateInstance(type);
      return contextFactory.CreateContextFrom(instance);
    }

    private Model.Context CreateDescriptionFrom(Type type, FieldInfo fieldInfo)
    {
      object instance = Activator.CreateInstance(type);
      return contextFactory.CreateContextFrom(instance, fieldInfo);
    }

    private static bool IsDescription(Type type)
    {
      return HasSpecificationMembers(type);
    }

    private static bool HasSpecificationMembers(Type type)
    {
      return type.GetPrivateFieldsWith(typeof(It)).Any();
    }

    /*
    private static bool HasDescriptionAttribute(Type type)
    {
      return type.IsDefined(typeof(DescriptionAttribute), false);
    }
    */

    private static IEnumerable<Type> EnumerateDescriptionsIn(Assembly assembly)
    {
      return assembly.GetExportedTypes().Where(IsDescription);
    }

    public Model.Context FindDescription(Type type)
    {
      if (IsDescription(type))
      {
        return CreateDescriptionFrom(type);
      }

      return null;
    }

    public Model.Context FindDescription(FieldInfo info)
    {
      Type type = info.ReflectedType;
      if (IsDescription(type))
      {
        return CreateDescriptionFrom(type, info);
      }

      return null;
    }
  }
}