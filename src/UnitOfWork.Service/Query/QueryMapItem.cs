using System;
using System.Collections.Generic;
using System.Text;

namespace UnitOfWork.Service.Query
{
    public enum DataRetriveTypeEnum
    {
        FirstOrDefault,
        List
    }

    /// <summary>
    /// This model is used to map data to model when using direct queries with dapper
    /// For more information use SqlScriptContext
    /// </summary>
    public class QueryMapItem
    {
        public Type Type { get; private set; }
        public DataRetriveTypeEnum DataRetriveType { get; private set; }
        public string PropertyName { get; private set; }

        public QueryMapItem(Type type, DataRetriveTypeEnum dataRetriveType, string propertyName)
        {
            Type = type;
            DataRetriveType = dataRetriveType;
            PropertyName = propertyName;
        }
    }
}
