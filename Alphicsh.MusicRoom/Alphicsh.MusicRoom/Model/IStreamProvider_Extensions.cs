using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.Model
{
    public static class IStreamProvider_Extensions
    {
        public static IStreamProvider Copy(this IStreamProvider provider)
        {
            var type = provider.GetType();
            ConstructorInfo constructor = type.GetConstructor(new Type[] { type });

            if (constructor == null)
                throw new NotSupportedException("The given stream provider has no copying constructor.");

            return constructor.Invoke(new object[] { provider }) as IStreamProvider;
        }
    }
}
