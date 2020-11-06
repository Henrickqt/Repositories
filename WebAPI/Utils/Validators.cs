using System.Linq;
using System.Net.Mail;

namespace Repositories.WebAPI.Utils
{
    public static class Validators
    {
        public static string ERROR_CHANGING_EMAIL_MSG = "Email não pode ser alterado\n";
        public static string ERROR_CHANGING_ID_MSG = "Id não pode ser alterado\n";
        public static string INVALID_NAME_MSG = "Nome é obrigatório\n";
        public static string INVALID_BIO_MSG = "Biografia é obrigatória\n";
        public static string INVALID_EMAIL_MSG = "Email inválido\n";
        public static string INVALID_PASSWORD_MSG = "Senha deve conter pelo menos 1 letra, 1 digito, 1 símbolo e ter 8 ou mais caracteres\n";
        public static string INVALID_PROJECT_NAME_MSG = "Nome é obrigatório\n";
        public static string INVALID_DESCRIPTION_MSG = "Descrição é obrigatória\n";
        public static string INVALID_LANGUAGES_MSG = "Linguagens é obrigatória\n";

        public static bool IsValidName(string name)
        {
            return (name != null && name.Trim().Length > 0);
        }

        public static bool IsValidBio(string bio)
        {
            return (bio != null && bio.Trim().Length > 0);
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var address = new MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        public static bool IsDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }

        public static bool IsSymbol(char c)
        {
            return (c > 32 && c < 127 && !IsDigit(c) && !IsLetter(c));
        }

        public static bool IsValidPassword(string password)
        {
            return
                password != null && password.Trim().Length > 7 &&
                password.Any(c => IsLetter(c)) &&
                password.Any(c => IsDigit(c)) &&
                password.Any(c => IsSymbol(c));
        }

        public static bool IsValidProjectName(string projectName)
        {
            return (projectName != null && projectName.Trim().Length > 0);
        }

        public static bool IsValidDescription(string description)
        {
            return (description != null && description.Trim().Length > 0);
        }

        public static bool IsValidLanguages(string languages)
        {
            return (languages != null && languages.Trim().Length > 0);
        }
    }
}
