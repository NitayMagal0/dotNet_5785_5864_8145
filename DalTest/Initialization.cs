namespace DalTest;

using System.Text;
using DalApi;
using DO;

public static class Initialization
{
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static IAssignment? s_dalAssignment; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static readonly Random s_rand = new();
    private static void createConfig() { }
    private static void createAssignment() { }
    private static void createCall() { }
    private static void createVolunteers()
    {
        string[] VolunteerNames =
            { "Amit Levi", "Noa Cohen", "Yossi Ben-David", "Tamar Shapiro", "Eitan Goldstein", "Lior Katz",
            "Rivka Avraham", "Shira Mizrahi", "Dan Alon", "Yael Rosenberg", "Ariel Bar-Zvi", "Maya Feldman",
            "David (Dudi) Peretz", "Orly Harel", "Yonatan Stein", "Alma Sela", "Gal Barak", "Michal Shaked",
            "Itai Blum", "Hila Golan", "Ronen Neuman", "Oren Eliav", "Nadav Segal", "Tali Ohayon", "Elior Friedman" };

        string[] VolunteerEmails =
        {
            "amitlevi@gmail.com", "noacohen@gmail.com", "yossiben-david@gmail.com", "tamarshapiro@gmail.com",
            "eitangoldstein@gmail.com", "liorkatz@gmail.com", "rivkaavraham@gmail.com", "shiramizrahi@gmail.com",
            "danalon@gmail.com", "yaelrosenberg@gmail.com", "arielbar-zvi@gmail.com", "mayafeldman@gmail.com",
            "davidperetz@gmail.com", "orlyharel@gmail.com", "yonatanstein@gmail.com", "almasela@gmail.com",
            "galbarak@gmail.com", "michalshaked@gmail.com", "itaiblum@gmail.com", "hilagolan@gmail.com",
            "ronenneuman@gmail.com", "oreneliav@gmail.com", "nadavsegal@gmail.com", "taliohayon@gmail.com",
            "eliorfriedman@gmail.com" };
        foreach (var name in VolunteerNames)
        {
            int MIN_ID = 200000000, MAX_ID = 400000000, id;
            do
                id = s_rand.Next(MIN_ID, MAX_ID);
            while (s_dalVolunteer!.Read(id) != null);

            string phoneNumber = GeneratePhoneNumber();
            string password = GenerateRandomPassword();
            bool? even = (id % 2) == 0 ? true : false;
            string? alias = even ? name + "ALIAS" : null;
            DateTime start = new DateTime(1995, 1, 1);
            DateTime bdt = start.AddDays(s_rand.Next((s_dalConfig.Clock - start).Days));

            s_dalVolunteer!.Create(new(id, name, alias, even, bdt));
        }



    }
    static string GeneratePhoneNumber()
    {
        Random random = new Random();

        // First 2 digits are "05"
        string prefix = "05";

        // Third digit is either 0, 2, 3, or 4
        int[] validNextDigits = { 0, 2, 3, 4 };
        int nextDigit = validNextDigits[random.Next(validNextDigits.Length)];

        // Generate the last 7 random digits
        string lastSevenDigits = "";
        for (int i = 0; i < 7; i++)
        {
            lastSevenDigits += random.Next(10); // Random digit from 0 to 9
        }

        // Combine all parts to form the phone number
        return $"{prefix}{nextDigit}{lastSevenDigits}";
    }
    static string GenerateRandomPassword()
    {
        Random random = new Random();

        // Generate a random password length between 8 and 16
        int length = random.Next(8, 17); // 17 is exclusive, so it generates 8 to 16

        // Character pools
        string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        string digits = "0123456789";
        string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        // Combine all pools
        string allChars = upperCase + lowerCase + digits + specialChars;

        StringBuilder password = new StringBuilder();

        // Ensure at least one character from each group
        password.Append(upperCase[random.Next(upperCase.Length)]);
        password.Append(lowerCase[random.Next(lowerCase.Length)]);
        password.Append(digits[random.Next(digits.Length)]);
        password.Append(specialChars[random.Next(specialChars.Length)]);

        // Fill the rest of the password length with random characters from all groups
        for (int i = 4; i < length; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }

        // Shuffle the characters to randomize the order
        char[] passwordArray = password.ToString().ToCharArray();
        for (int i = passwordArray.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (passwordArray[i], passwordArray[j]) = (passwordArray[j], passwordArray[i]);
        }

        return new string(passwordArray);
    }
}