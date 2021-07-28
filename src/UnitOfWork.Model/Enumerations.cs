namespace UnitOfWork.Model
{
    public enum DataRetriveTypeEnum
    {
        /// <summary>
        /// Select only top first records
        /// </summary>
        FirstOrDefault,

        /// <summary>
        /// Return object as list
        /// </summary>
        List,
        /// <summary>
        /// Script does not return anything, Probably SP returning INT
        /// </summary>
        Empty
    }
}
