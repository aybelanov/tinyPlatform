using Shared.Common;
using System;
using System.Collections.Generic;

namespace Hub.Tests;

public class BaseEntityEqualityComparer<T> : IEqualityComparer<T> where T : BaseEntity
{
   public bool Equals(T x, T y)
   {
      return x?.Id == y?.Id;
   }

   public int GetHashCode(T obj)
   {
      throw new NotImplementedException();
   }
}
