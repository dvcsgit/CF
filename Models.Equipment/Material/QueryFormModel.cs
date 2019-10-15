namespace Models.Maintenance.Material
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
