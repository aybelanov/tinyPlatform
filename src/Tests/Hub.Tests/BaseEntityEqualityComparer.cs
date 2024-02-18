using System;
using System.Collections.Generic;
using Hub.Core;
using Shared.Common;

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
