namespace DalTest;
using System.Text;
using Dal;
using DalApi;
using DO;


/// <summary>
/// A class that initializes values ​​in DataSource lists so we can perform tests
/// </summary>
public static class Initialization
{
    private static IDal s_dal;
    private static readonly Random s_rand = new();

    static List<int> VolunteerId = new List<int>();
    /// <summary>
    /// The function connects the 50 calls to the 25 volunteers by assigning two calls to each volunteer
    /// </summary>
    private static void createAssignment()
    {
        if (VolunteerId == null) return;
        int i = 0;                               //The above variable is responsible for advancing the identification number of the calls
        TreatmentEndType? status = null;
        foreach (var id in VolunteerId)
        {
            //In order to have variety in the ending state of the calls, I made these conditional sentences that for each ID will give a different state
            if (id % 4 == 0)
            {
                status = TreatmentEndType.CancelledByAdmin;
            }
            else if (id % 3 == 0)
            {
                status = TreatmentEndType.CancelledByUser;
            }
            else if (id % 2 == 0)
            {
                status = TreatmentEndType.Completed;
            }
            else
            {
                status = TreatmentEndType.Expired;
            }

            //Since there are 50 calls and only 25 volunteers this loop assigns each volunteer two calls
            for (int j = 0; j < 2; j++, i++)
            {
                s_dal.Assignment!.Create(new Assignment(
                    0,
                    1000 + i,
                    id,
                    DateTime.Now,
                    DateTime.Now,
                    status
                ));
                i++;
            }
        }
    }


    /// <summary>
    /// The function generates 50 different calls
    /// </summary>
    private static void createCall()
    {
        //An array that holds all the addresses of the calls
        string[] Addresses =
        {
            // Rishon LeTsiyon
            "Rothschild St 22, Rishon LeTsiyon",
            "HaPalmach St 10, Rishon LeTsiyon",
            "Herzl St 56, Rishon LeTsiyon",
            "HaPalmach St 10, Rishon LeTsiyon",
            "HaPalmach St 14, Rishon LeTsiyon",
            "HaAlon St 15, Rishon LeTsiyon",
            "Masada St 11, Rishon LeTsiyon",
            "Bialik St 14, Rishon LeTsiyon",
            "Weizmann St 17, Rishon LeTsiyon",
            "HaShikma St 24, Rishon LeTsiyon",

            // Petah Tikva
            "Bar Ilan St 6, Petah Tikva",
            "Kaplan St 1, Petah Tikva",
            "HaHistadrut St 40, Petah Tikva",
            "Kaplan St 3, Petah Tikva",
            "Emek Dotan St 5, Petah Tikva",
            "Hadar St 7, Petah Tikva",
            "HaRav Kook St 5, Petah Tikva",
            "Weizmann St 12, Petah Tikva",
            "Em HaMoshavot St 10, Petah Tikva",
            "Weizmann St 17, Petah Tikva",

            // Be'er Sheva
            "HaNasi St 2, Be'er Sheva",
            "Rothschild St 6, Be'er Sheva",
            "Eli Cohen St 12, Be'er Sheva",
            "Ben Gurion Blvd 3, Be'er Sheva",
            "Shazar St 20, Be'er Sheva",
            "Avraham Avinu St 14, Be'er Sheva",
            "Avraham Avinu St 17, Be'er Sheva",
            "HaAtzmaut St 18, Be'er Sheva",
            "HaPalmach St 5, Be'er Sheva",
            "Eli Cohen St 11, Be'er Sheva",

            // Holon
            "Sokolov St 22, Holon",
            "Weizmann St 16, Holon",
            "HaRav Kook St 10, Holon",
            "Dov Hoz St 7, Holon",
            "Weizmann St 16, Holon",
            "Bialik St 3, Holon",
            "Pinsker St 6, Holon",
            "Shenkar St 19, Holon",
            "Weizmann St 16, Holon",
            "Weizmann St 16, Holon",

            // Netanya
            "HaYarden St 5, Netanya",
            "Bialik St 18, Netanya",
            "Ussishkin St 14, Netanya",
            "Ussishkin St 11, Netanya",
            "Sokolov St 4, Netanya",
            "Ussishkin St 15, Netanya",
            "Eli Cohen St 8, Netanya",
            "Ussishkin St 15, Netanya",
            "Eli Cohen St 8, Netanya",
            "Bialik St 18, Netanya"
};



        //These two arrays hold all the longitude and latitude of the addresses
        float[] latitudeArray =
        { 31.9643154f, 31.955518f, 31.9676173f, 31.9622751f, 32.0715704f, 32.0181548f, 31.9589889f,
         31.9653336f, 31.9713531f, 31.9630978f, 32.0853361f, 32.0929381f, 32.0921131f, 32.0860497f,
         32.0905002f, 32.0917919f, 32.1595358f, 32.0824978f, 32.0805623f, 32.102801f, 31.2521018f,
         31.2521018f, 31.24182549999999f, 31.2472712f, 31.2617444f, 31.2521018f, 31.2641094f,
         31.269819f, 31.2521018f, 31.2370494f, 32.0228929f, 32.0620845f, 32.0210558f,
         32.015833f, 32.0206689f, 32.0215594f, 32.0205318f, 32.0130381f, 32.0260154f,
         32.0751249f, 32.3276846f, 32.3155879f, 32.321458f, 32.3315165f, 32.3319194f,
         32.284403f, 32.341958f, 32.3280852f, 32.321458f, 32.32686f  };

        float[] longitudeArray =
        {34.8044968f, 34.8115675f, 34.8034651f, 34.810258f, 34.7806364f, 34.7759021f,
         34.8995623f, 34.802021f, 34.8043477f, 34.7991253f, 34.8862855f, 34.9014356f,
         34.860923f, 34.8867304f, 34.869086f, 34.9023234f, 34.9727815f, 34.882186f,
         34.8731057f, 34.8648183f, 34.7867691f, 34.7867691f, 34.8022122f, 34.80614449999999f,
         34.8007625f, 34.7867691f, 34.7938402f, 34.798157f, 34.7867691f, 34.7914045f, 34.7722195f,
         34.77030269999999f, 34.7795762f, 34.787384f, 34.7538539f, 34.7724563f, 34.7777558f, 34.7873238f,
         34.7768943f, 34.8156124f, 34.8602827f, 34.8482565f, 34.853196f, 34.8536459f, 34.8515711f, 34.8573248f,
         34.857991f, 34.8516656f, 34.853196f, 34.8643746f };


        //The date range
        DateTime start = new DateTime(2024, 1, 1);          // Start date
        DateTime today = DateTime.Today;                    // Today's date
        int range = (today - start).Days;                   // Total days between start and today


        //A loop that creates 10 calls of type "HelpForFamiliesInNeed"
        for (int i = 0; i < 10; i++)
        {
            DateTime OpeningTime = start.AddDays(s_rand.Next(range));  // Generate random date within the range
            s_dal.Call!.Create(new Call(
                0,
                CallType.HelpForFamiliesInNeed,
                "Assistance provided to families lacking essential resources, such as food, shelter, or financial aid.",
                Addresses[i],
                latitudeArray[i],
                longitudeArray[i],
                OpeningTime,
                DateTime.Today.AddDays(2)           //End time
                ));
        }


        //A loop that creates 10 calls of type "FoodPackagingForNeedyFamilies"
        for (int i = 10; i < 20; i++)
        {
            DateTime OpeningTime = start.AddDays(s_rand.Next(range));  // Generate random date within the range
            s_dal.Call!.Create(new Call(
                0,
                CallType.FoodPackagingForNeedyFamilies,
                "Preparing and packaging food items for distribution to families in need.",
                Addresses[i],
                latitudeArray[i],
                longitudeArray[i],
                OpeningTime,
                DateTime.Today.AddDays(s_rand.Next(15, 60))         //End time
                ));
        }


        //A loop that creates 10 calls of type "CleaningShelters"
        for (int i = 20; i < 30; i++)
        {
            DateTime OpeningTime = start.AddDays(s_rand.Next(range));  // Generate random date within the range
            s_dal.Call!.Create(new Call(
                0,
                CallType.CleaningShelters,
                "Task focused on cleaning and maintaining shelters for displaced or at-risk individuals.",
                Addresses[i],
                latitudeArray[i],
                longitudeArray[i],
                OpeningTime,
                DateTime.Today.AddDays(s_rand.Next(15, 60))     //End time
                ));
        }


        //A loop that creates 20 calls of type "HospitalVisitsForMoraleBoost"
        for (int i = 30; i < 50; i++)
        {
            DateTime OpeningTime = start.AddDays(s_rand.Next(range));  // Generate random date within the range
            s_dal.Call!.Create(new Call(
                0,
                CallType.HospitalVisitsForMoraleBoost,
                "Visiting hospitals to support and uplift the spirits of patients and healthcare workers.",
                Addresses[i],
                latitudeArray[i],
                longitudeArray[i],
                OpeningTime,
                DateTime.Today.AddDays(s_rand.Next(15, 60))     //End time
                ));
        }
    }


    /// <summary>
    /// Creates 25 volunteers
    /// </summary>
    private static void createVolunteers()
    {
        //An array that saves the names of the volunteers
        string[] VolunteerNames =
        { "Amit Levi", "Noa Cohen", "Yossi Ben-David", "Tamar Shapiro", "Eitan Goldstein", "Lior Katz",
            "Rivka Avraham", "Shira Mizrahi", "Dan Alon", "Yael Rosenberg", "Ariel Bar-Zvi", "Maya Feldman",
            "David Peretz", "Orly Harel", "Yonatan Stein", "Alma Sela", "Gal Barak", "Michal Shaked",
            "Itai Blum", "Hila Golan", "Ronen Neuman", "Oren Eliav", "Nadav Segal", "Tali Ohayon", "Elior Friedman" };


        //An array that saves the emails of the volunteers
        string[] VolunteerEmails =
        {
            "amitlevi@gmail.com", "noacohen@gmail.com", "yossiben-david@gmail.com", "tamarshapiro@gmail.com",
            "eitangoldstein@gmail.com", "liorkatz@gmail.com", "rivkaavraham@gmail.com", "shiramizrahi@gmail.com",
            "danalon@gmail.com", "yaelrosenberg@gmail.com", "arielbar-zvi@gmail.com", "mayafeldman@gmail.com",
            "davidperetz@gmail.com", "orlyharel@gmail.com", "yonatanstein@gmail.com", "almasela@gmail.com",
            "galbarak@gmail.com", "michalshaked@gmail.com", "itaiblum@gmail.com", "hilagolan@gmail.com",
            "ronenneuman@gmail.com", "oreneliav@gmail.com", "nadavsegal@gmail.com", "taliohayon@gmail.com",
            "eliorfriedman@gmail.com" };


        //A system that keeps the addresses of the volunteers
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


        //These arrays store the longitude and latitude of the addresses
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


        
        for (int i = 0; i < VolunteerNames.Length; i++)
        {
            int MIN_ID = 200000000, MAX_ID = 400000000, id;
            do
                id = s_rand.Next(MIN_ID, MAX_ID);
            while (s_dal.Volunteer!.Read(id) is not null);  //here its calling for read
            VolunteerId.Add(id);                            
            string phoneNumber = GeneratePhoneNumber();
            string password = GenerateRandomPassword();
            DateTime start = new DateTime(1995, 1, 1);
            DateTime bdt = start.AddDays(s_rand.Next((s_dal.Config.Clock - start).Days));

           

            s_dal.Volunteer!.Create(new Volunteer(
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


    /// <summary>
    /// The function creates an invented phone number for the volunteer
    /// </summary>
    /// <returns>phone number</returns>
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


    /// <summary>
    /// The function creates an invented password for the volunteer
    /// </summary>
    /// <returns>password</returns>
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
 
    public static void Do() 

    {
        s_dal = DalApi.Factory.Get;
        Console.WriteLine("Reset Configuration values and List values:");
        s_dal.ResetDB();

        Console.WriteLine("Initializing Volunteers list:");
        createVolunteers();
        Console.WriteLine("Initializing Calls list:");
        createCall();
        Console.WriteLine("Initializing Assignments list:");
        createAssignment();
    }

   
}

