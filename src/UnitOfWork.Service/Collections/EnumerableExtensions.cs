using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace UnitOfWork.Service.Collections
{
    public static class EnumerableExtensions
    {

        /// <summary>
        /// Convert object to Type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Convert<T>(this object obj)
        {
            var serializedString = JsonConvert.SerializeObject(obj);
            var result = JsonConvert.DeserializeObject<T>(serializedString);
            return result;
        }

    }
}
