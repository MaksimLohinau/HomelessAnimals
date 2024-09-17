namespace HomelessAnimals.BusinessLogic.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRabiesVactinated { get; set; }
        public DateTime RabiesVacineDate { get; set; }
        public bool IsVactinated { get; set; }
        public string VacineName { get; set; }
        public DateTime VacineDate { get; set; }
        public string LivingPlace { get; set; }
        public byte[] Image { get; set; }
        public int VolunteerProfileId { get; set; }
        public int[] AnimalAdmins { get; set; }
    }
}
