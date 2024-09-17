namespace HomelessAnimals.QueryParams
{
    public class GetVolunteersQueryParams
    {
        public int? Page { get; set; }
        public string[] Country { get; set; }
        public string Search { get; set; }
    }
}
