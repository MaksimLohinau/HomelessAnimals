namespace HomelessAnimals.Shared.Constants
{
    public static class Permissions
    {
        public static class Animal
        {
            private const string Prefix = "animal";

            public const string Create = $"{Prefix}:create";
            public const string Edit = $"{Prefix}:edit";
            public const string Delete = $"{Prefix}:delete";
        }

        public static class Volunteer
        {
            private const string Prefix = "volunteer";

            public const string Edit = $"{Prefix}:edit";
            public const string Delete = $"{Prefix}:delete";
            public const string ViewEmail = $"{Prefix}:view_email";
            public const string ViewPhoneNumber = $"{Prefix}:view_phone_number";
            public const string ViewTelegram = $"{Prefix}:view_telegram";
        }
        public static class SignUp
        {
            private const string Prefix = "sign_up";

            public const string View = $"{Prefix}:view";
            public const string ChangeStatus = $"{Prefix}:change_status";
        }
    }
}
