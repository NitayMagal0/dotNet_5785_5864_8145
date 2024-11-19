namespace DalTest;

using System.Text;
using System.Xml.Linq;
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
            "David Peretz", "Orly Harel", "Yonatan Stein", "Alma Sela", "Gal Barak", "Michal Shaked",
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

        string[] volunteerAddresses =
        {
            // Tel Aviv
            "Herzl St 23, Tel Aviv",
            "Dizengoff St 99, Tel Aviv",
            "Allenby St 47, Tel Aviv",
            "Rothschild Blvd 35, Tel Aviv",
            "King George St 21, Tel Aviv",
            "Ben Yehuda St 102, Tel Aviv",
            "Ibn Gabirol St 58, Tel Aviv",
            "Florentin St 5, Tel Aviv",
            "Bugrashov St 15, Tel Aviv",
            "Arlozorov St 25, Tel Aviv",

            // Jerusalem
            "Jaffa St 55, Jerusalem",
            "King David St 11, Jerusalem",
            "Ben Yehuda St 30, Jerusalem",
            "Emek Refaim St 12, Jerusalem",
            "Yafo St 40, Jerusalem",
            "HaPalmach St 20, Jerusalem",
            "Derech Hebron St 78, Jerusalem",
            "Keren Hayesod St 6, Jerusalem",
            "HaNevi'im St 39, Jerusalem",
            "Agron St 8, Jerusalem",

            // Haifa
            "HaNassi Blvd 100, Haifa",
            "Ben Gurion Blvd 44, Haifa",
            "Herzl St 25, Haifa",
            "Hatzionut Blvd 15, Haifa",
            "Shivat Zion St 13, Haifa",
        };

        float[] latitudeArray =
        { 32.0608944f, 32.0794167f, 32.0703077f, 32.0638841f,
          32.0711762f, 32.0826139f, 32.0789016f, 32.0562879f,
          32.0773501f, 32.0870401f, 31.7832166f, 31.7766337f,
          31.781114f, 31.7657359f, 31.7818882f, 31.7675627f,
          31.7553434f, 31.7740197f, 31.7838441f, 31.7755135f,
          32.8080666f, 32.8188199f, 32.8113992f, 32.8176865f, 32.7903562f };

        float[] longitudeArray =
        {34.7705256f, 34.7737204f, 34.7703935f, 34.7735413f,
         34.7722718f, 34.7714952f, 34.7815665f, 34.7677873f,
         34.7691619f, 34.7753243f, 35.2176534f, 35.2222924f,
         35.21595f, 35.2217105f, 35.22011570000001f, 35.2114197f,
         35.2217262f, 35.2189366f, 35.220514f, 35.2184101f, 34.9858606f,
         34.9897136f, 34.9979308f, 34.99322f, 35.0150586f };

        foreach (var name in VolunteerNames)
        {
            int MIN_ID = 200000000, MAX_ID = 400000000, id;
            do
                id = s_rand.Next(MIN_ID, MAX_ID);
            while (s_dalVolunteer!.Read(id) is not null);

            string phoneNumber = GeneratePhoneNumber();
            string password = GenerateRandomPassword();
            bool even = (id % 2) == 0 ? true : false;
            string? alias = even ? name + "ALIAS" : null;
            DateTime start = new DateTime(1995, 1, 1);
            DateTime bdt = start.AddDays(s_rand.Next((s_dalConfig.Clock - start).Days));

            s_dalVolunteer!.Create(new Volunteer(id, name, phoneNumber, ));
        }

        for (int i = 0; i < VolunteerNames.Length; i++) {
            int MIN_ID = 200000000, MAX_ID = 400000000, id;
            do
                id = s_rand.Next(MIN_ID, MAX_ID);
            while (s_dalVolunteer!.Read(id) is not null);

            string phoneNumber = GeneratePhoneNumber();
            string password = GenerateRandomPassword();
            DateTime start = new DateTime(1995, 1, 1);
            DateTime bdt = start.AddDays(s_rand.Next((s_dalConfig.Clock - start).Days));

            s_dalVolunteer!.Create(new Volunteer(
               id,
               VolunteerNames[i],
               phoneNumber,
               VolunteerEmails[i],
               password,
               volunteerAddresses[i],
               latitudeArray[i],
               longitudeArray[i],
               Role.Volunteer,
               true
            ));
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