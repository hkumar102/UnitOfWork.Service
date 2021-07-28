using System;

namespace UnitOfWork.Model.Query
{
   
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
