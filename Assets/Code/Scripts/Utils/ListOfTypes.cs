using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
