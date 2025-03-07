using System;
using System.Collections.Generic;

namespace FrostfallSaga.Utils
{
    public class ListOfTypes<T> : List<Type>
    {
        public void Add<U>() where U : T
        {
            Add(typeof(U));
        }
    }
}