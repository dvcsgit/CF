﻿namespace Models.Maintenance.Solution
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
