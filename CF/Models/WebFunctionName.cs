namespace CF.Models
{
    public class WebFunctionName
    {
        public string WebFunctionId { get; set; }
        public virtual WebFunction WebFunction { get; set; }

        public string Language { get; set; }
        public string Name { get; set; }        
    }
}
