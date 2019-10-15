namespace Models.Maintenance.CheckItem
{
    public class QueryFormModel
    {
        public QueryParameters QueryParameters { get; set; }

        public QueryFormModel()
        {
            QueryParameters = new QueryParameters();
        }
    }
}
