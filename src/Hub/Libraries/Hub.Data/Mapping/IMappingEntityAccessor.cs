using System;

namespace Hub.Data.Mapping;

/// <summary>
/// Represents interface to implement an accessor to mapped entities
/// </summary>
public interface IMappingEntityAccessor
{
   /// <summary>
   /// Returns mapped entity descriptor
   /// </summary>
   /// <param name="entityType">Type of entity</param>
   /// <returns>Mapped entity descriptor</returns>
   AppEntityDescriptor GetEntityDescriptor(Type entityType);
}