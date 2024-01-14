namespace MISBack.Services.Validation
{
    public class EntityConstants
    {
        public const string PasswordRegex = @"^(?=.*\d).+$";

        public const string ShortOrLongPasswordError = "Пароль должен быть более 6 и менее 100 символов в длину";

        public const string IncorrectPasswordError = "В пароле должна быть хотя бы одна цифра";

        public const string ShortOrLongEmailError = "Почта должна быть более 5 и менее 100 символов в длину";

        public const string IncorrectDateError = "Некорректно введена дата";

        public const string IncorrectEmailError = "Некорттектно введен почтовый адрес";

        public const string IncorrectGenderError = "Available values for Gender are Male and Female";
    }
}
