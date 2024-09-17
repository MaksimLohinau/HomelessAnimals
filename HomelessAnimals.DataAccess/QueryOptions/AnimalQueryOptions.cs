namespace HomelessAnimals.DataAccess.QueryOptions
{
    public class AnimalQueryOptions
    {
        public bool IncludeVolunteerProfile { get; set; }
        public bool IncludeCity {  get; set; }
        public bool AsNoTracking { get; set; }
    }
}
