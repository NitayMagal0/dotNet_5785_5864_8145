using System;
using Dal;
using DalApi;
using DO;

namespace DalTest

    //NEED TO ADD EXEPTIONS
{
    internal class Program
    {
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
        private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
        private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        private static ICall? s_dalCall = new CallImplementation(); //stage 1
        enum MenuOptions
        {
            Exit = 0,                 
            VolunteerSubMenu = 1,             
            CallSubMenu = 2, 
            AssignmentSubMenu = 3,
            Initialize = 5,
            ShowAll = 6,
            SubMenuFor = 7,
            Reset = 8
        }
        enum SubMenuOptions
        {
            Exit = 0,
            AddObject = 1,
            ShowObject = 2,
            ShowList = 3,
            Update = 4,
            DeleteObject = 5,
            DeleteAll = 6
        }

        static void Main(string[] args)
        {
        int conditon;
            do
            {
                Console.WriteLine("Enter a number: \n" +
                "0 - exit the menu \n" +
                "1 - enter volunteer sub menu \n" +
                "2 - enter call sub menu \n" +
                "3 - enter assignment sub menu");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)MenuOptions.VolunteerSubMenu:
                           { 
                                volunteerMenu();
                                break;
                           }
                        case (int)MenuOptions.CallSubMenu:
                            {
                                assignmentMenu();
                                break;
                            }
                        case (int)MenuOptions.Initialize:
                            { break; }
                        case (int)MenuOptions.ShowAll:
                            { break; }
                        case (int)MenuOptions.SubMenuFor:
                            { break; }
                        case (int)MenuOptions.Reset:
                            { break; }
                        case (int)MenuOptions.Exit:
                            //just break and the condition below will exit the loop
                            break;

                        default:
                            break;
                    }

                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            while (conditon != 0);

        }

        private static void volunteerMenu()
        {
            int conditon;
            do
            {
                Console.WriteLine("Enter a number:");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)SubMenuOptions.AddObject:
                            {
                                addVolunteer();
                                break;
                            }
                        case (int)SubMenuOptions.ShowObject:
                            {
                                int id = int.Parse(Console.ReadLine());
                                Volunteer temp = s_dalVolunteer.Read(id);
                                printVolunteer(temp);
                                break;
                            }
                        case (int)SubMenuOptions.ShowList:
                            {
                                List<Volunteer> temp = s_dalVolunteer.ReadAll();
                                foreach (var vol in temp)
                                {
                                    printVolunteer(vol);
                                }
                                break;
                            }
                        case (int)SubMenuOptions.Update:
                            {

                                break;
                            }
                        case (int)SubMenuOptions.DeleteObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteAll:
                            {
                                break;
                            }



                        case (int)SubMenuOptions.Exit:
                            //just break and the condition below will exit the loop
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            while (conditon != 0);

            return;
        }

        private static void addVolunteer()
        {
            int id;
            string fullName,phoneNumber, email, password, fullAddress;
            double latitude, longitude;
            bool isActive;

            // Receive the volunteer's details into variables
            Console.WriteLine("Enter the Volunteer ID:");
            id = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the Volunteer Full Name:");
            fullName = Console.ReadLine();

            Console.WriteLine("Enter the Volunteer Phone number:");
            phoneNumber = Console.ReadLine();

            Console.WriteLine("Enter the Volunteer Email:");
            email = Console.ReadLine();

            Console.WriteLine("Enter the Volunteer Password:");
            password = Console.ReadLine();

            Console.WriteLine("Enter the Volunteer Full Address:");
            fullAddress = Console.ReadLine();

            Console.WriteLine("Enter the Volunteer Latitude:");
            latitude = double.Parse(Console.ReadLine());

            Console.WriteLine("Enter the Volunteer Longitude:");
            longitude = double.Parse(Console.ReadLine());

            Console.WriteLine("Is the volunteer active? (true/false):");
            isActive = bool.Parse(Console.ReadLine());


            Volunteer temp = new Volunteer(id, fullName, phoneNumber, email, password, fullAddress, latitude, longitude, Role.Volunteer, isActive);   
            try
            {
                s_dalVolunteer.Create(temp);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");
                
            }
        }
        private static void printVolunteer(Volunteer vol)
        {
            Console.WriteLine($"ID: {vol.Id}");
            Console.WriteLine($"Full Name: {vol.FullName}");
            Console.WriteLine($"Phone Number: {vol.MobilePhone}");
            Console.WriteLine($"Email: {vol.Email}");
            Console.WriteLine($"Password: {vol.Password}");
            Console.WriteLine($"Address: {vol.FullAddress}");
            Console.WriteLine($"Latitude: {vol.Latitude}");
            Console.WriteLine($"Longitude: {vol.Longitude}");
            Console.WriteLine($"Role: {vol.Role}");
            if (vol.IsActive)
                Console.WriteLine("Is Active");
            else
                Console.WriteLine("Is Not Active");


        }
        private static void assignmentMenu()
        {

            int conditon;
            do
            {
                Console.WriteLine("Enter a number:");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)SubMenuOptions.AddObject:
                            {

                                break;
                            }
                        case (int)SubMenuOptions.ShowObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.ShowList:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.Update:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteAll:
                            {
                                break;
                            }



                        case (int)SubMenuOptions.Exit:
                            //just break and the condition below will exit the loop
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            while (conditon != 0);

            return;
        }
        private static void callMenu()
        {

            int conditon;
            do
            {
                Console.WriteLine("Enter a number:");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)SubMenuOptions.AddObject:
                            {

                                break;
                            }
                        case (int)SubMenuOptions.ShowObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.ShowList:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.Update:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteAll:
                            {
                                break;
                            }



                        case (int)SubMenuOptions.Exit:
                            //just break and the condition below will exit the loop
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            while (conditon != 0);

            return;
        }
    }
}
