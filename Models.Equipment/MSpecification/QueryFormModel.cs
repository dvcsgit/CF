namespace Models.Maintenance.MSpecification
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
